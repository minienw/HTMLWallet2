using System.IdentityModel.Tokens.Jwt;
using System.Text;
using CheckInQrWeb.Core.Helpers;
using CheckInQrWeb.Core.Models;
using CheckInQrWeb.Core.Models.api.Identity;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;

namespace CheckInQrWeb.Core
{
    //Scoped
    public class VerificationWorkflow
    {
        private readonly HttpGetIdentityCommand _HttpGetIdentityCommand;
        private readonly HttpPostTokenCommand _HttpPostTokenCommand;
        private readonly HttpPostValidateCommand _HttpPostValidateCommand;
        private readonly HttpPostCallbackCommand _HttpPostCallbackCommand;

        public InitiatingQrPayload InitiatingQrPayload { get; private set; }
        public JwtSecurityToken ResultToken { get; private set; }

        private JwtSecurityToken _InitiatingQrPayloadToken;
        private string _ResultTokenServiceUrl;
        private HttpPostTokenCommandResult _PostTokenResult;
        private AsymmetricCipherKeyPair _WalletSigningKeyPair;

        private readonly ILogger _Logger;

        public VerificationWorkflow(HttpPostTokenCommand httpPostTokenCommand, HttpPostValidateCommand httpPostValidateCommand, HttpGetIdentityCommand httpGetIdentityCommand, HttpPostCallbackCommand httpPostCallbackCommand, ILogger<VerificationWorkflow> logger)
        {
            _HttpGetIdentityCommand = httpGetIdentityCommand;
            _HttpPostTokenCommand = httpPostTokenCommand;
            _HttpPostValidateCommand = httpPostValidateCommand;
            _HttpPostCallbackCommand = httpPostCallbackCommand;
            _Logger = logger;
        }

        public WorkflowResult<InitiatingQrPayload> OnInitialized(string qrBytesBase64)
        {
            var debugMessages = new List<string>();
            try
            {
                var valueBytes = Convert.FromBase64String(qrBytesBase64);
                //TODO reasonable length check
                var jsonData = Encoding.UTF8.GetString(valueBytes);

                InitiatingQrPayload = JsonConvert.DeserializeObject<InitiatingQrPayload>(jsonData);
                _InitiatingQrPayloadToken = InitiatingQrPayload?.Token.ToJwtSecurityToken();

                _Logger.LogInformation($"Initiating QR Code: {jsonData}");

                return new(InitiatingQrPayload, true, "Token from URL processed successfully.", debugMessages.ToArray());
            }
            catch (Exception e)
            {
                debugMessages.Add($"Error reading QR Code: {e}");
                _Logger.LogError($"Error reading QR Code: {e}");
                return new(null, false, "Could not process token in URL", debugMessages.ToArray());
            }
        }

        public async Task<WorkflowResult<JwtSecurityToken>> ValidateDccAsync(string dccQrData)
        {
            _Logger.LogInformation("Start validation.");
            var args = new HttpPostValidateArgs();
            var debugMessages = new List<string>();
            try
            {
                _WalletSigningKeyPair = Crypto.GenerateEcKeyPair();

                if (!dccQrData.IsValidDccJson())
                {
                    debugMessages.Add("Invalid DCC QR code (did not start with 'HC1:')");
                    return new(null, false, "Please use international QR Code.", debugMessages.ToArray());
                }

                debugMessages.Add($"Uploaded QR Data starts with: {dccQrData.Substring(0, 10)}");

                var airlineIdentity = await _HttpGetIdentityCommand.ExecuteAsync(InitiatingQrPayload.ServiceIdentity);
                var accessTokenServiceUrl = airlineIdentity.GetServiceUrl("AccessTokenService");
                _ResultTokenServiceUrl = airlineIdentity.GetServiceUrl("ResultTokenService");
                _PostTokenResult = await _HttpPostTokenCommand.ExecuteAsync(new HttpPostTokenCommandArgs
                {
                    AccessTokenServiceUrl = accessTokenServiceUrl,
                    InitiatingToken = InitiatingQrPayload.Token,
                    WalletPublicKey = _WalletSigningKeyPair.Public,
                });

                var encKeyJwk = JsonConvert.DeserializeObject<PublicKeyJwk>(Encoding.UTF8.GetString(Convert.FromBase64String(_PostTokenResult.EncKeyBase64)));

                args.InitiatingQrPayloadToken = _InitiatingQrPayloadToken;
                args.DccQrCode = dccQrData;
                args.PublicKeyJwk = encKeyJwk;
                args.IV = _PostTokenResult.Nonce;
                args.WalletPrivateKey = _WalletSigningKeyPair.Private;
                args.ValidatorAccessTokenObject = _PostTokenResult.ValidationAccessTokenPayload;
                args.ValidatorAccessToken = _PostTokenResult.ValidationAccessToken;

                var httpPostValidateResult = await _HttpPostValidateCommand.Execute(args);

                debugMessages.Add($"Process uploaded QR. Display DCC check response: {JsonConvert.SerializeObject(httpPostValidateResult)}");

                ResultToken = (JwtSecurityToken)(new JwtSecurityTokenHandler().ReadToken(httpPostValidateResult.Content));
                debugMessages.Add($"Show QR result: { ResultToken.RawData}");

                //TODO check result return new(null, false, "No valid QR code uploaded. Unexpected response received from validation services.", debugMessages.ToArray());

                // verify end result
                var resultClaim = ResultToken.Claims.FirstOrDefault(x => x.Type == "result");
                if (resultClaim == null)
                {
                    debugMessages.Add("Claim result empty.");
                    return new(null, false, "Unexpected response received from validation services.", debugMessages.ToArray());
                }

                var resultsClaim = ResultToken.Claims.Where(x => x.Type == "results").ToList();
                if (!resultClaim.Value.Equals("OK", StringComparison.InvariantCultureIgnoreCase))
                {
                    return new(ResultToken, false, "Security token received from DCC validation service did not pass. Review the error messages for details.", debugMessages.ToArray());
                }

                return new(ResultToken, true, "Success.", debugMessages.ToArray());

            }
            catch (HttpRequestException e)
            {
                debugMessages.Add($"Exception: {e}");
                _Logger.LogError(String.Join("!!!", debugMessages));
                return new(null, false, "Unable to connect to a verifying service. Please try again later.", debugMessages.ToArray());
            }
            catch (Exception e)
            {
                debugMessages.Add($"Exception: {e}");
                _Logger.LogError(String.Join("!!!", debugMessages));
                return new(null, false, "Unable to process the validation request at this time. Please try again later.", debugMessages.ToArray());
            }
            finally
            {
                //Hint to memory manager
                args = null;
                dccQrData = null;
                _Logger.LogInformation("End validation.");
            }
        }

        public async Task<WorkflowResult<bool>> NotifyServiceProvider()
        {
            var debugMessages = new List<string>();
            try
            {
                var args = new HttpPostCallbackCommandArgs
                {
                    EndpointUri = _ResultTokenServiceUrl,
                    ResultToken = ResultToken.Claims.First(x => x.Type == "result").Value,
                    ValidationAccessToken = _PostTokenResult.ValidationAccessToken
            };
                var r = await _HttpPostCallbackCommand.ExecuteAsync(args);
                return new(r, true, "Process complete.", debugMessages.ToArray());
            }
            catch (Exception e)
            {
                debugMessages.Add($"Exception: {e}");
                _Logger.LogError(String.Join("!!!", debugMessages));
                return new(false, false, "Exception!", debugMessages.ToArray());
            }
        }
    }
}
