using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CheckInQrWeb.Core.Helpers;
using CheckInQrWeb.Core.Models;
using CheckInQrWeb.Core.Models.api.Identity;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
//using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;


namespace CheckInQrWeb.Core
{
    //Scoped
    public class VerificationWorkflow
    {
        //Error or finished
        public bool Exiting { get; private set; }
        public string WorkflowMessage { get; private set; } //Error message or try different DCC.

        //After initialize success
        public bool ShowDccUpload { get; private set; }

        //After dcc upload/verification attempt
        public bool AfterDccValidation { get; private set; }
        public DccExtract DccExtract { get; private set; }
        public FailureItem[] FailureMessages { get; private set; }
        public List<string> DebugMessages { get; } = new();

        private readonly HttpGetIdentityCommand _HttpGetIdentityCommand;
        private readonly HttpPostTokenCommand _HttpPostTokenCommand;
        private readonly HttpPostValidateCommand _HttpPostValidateCommand;
        private readonly HttpPostCallbackCommand _HttpPostCallbackCommand;

        public InitiatingQrPayload InitiatingQrPayload { get; private set; }
        public JwtSecurityToken ResultToken { get; private set; }
        public bool ShowNotify { get; private set; }

        private JwtSecurityToken _InitiatingQrPayloadToken;
        private string _ResultTokenServiceUrl;
        private HttpPostTokenCommandResult _PostTokenResult;
        private AsymmetricCipherKeyPair _WalletSigningKeyPair;

        private readonly ILogger _Logger;
        private string _AccessTokenServiceUrl;
        private HttpPostValidateResult _HttpPostValidateResult;
        private string _ResultClaimValue;

        private readonly IDialogService _DialogService;
        public VerificationWorkflow(HttpPostTokenCommand httpPostTokenCommand, HttpPostValidateCommand httpPostValidateCommand, HttpGetIdentityCommand httpGetIdentityCommand, HttpPostCallbackCommand httpPostCallbackCommand, ILogger<VerificationWorkflow> logger, IDialogService dialogService)
        {
            _HttpGetIdentityCommand = httpGetIdentityCommand;
            _HttpPostTokenCommand = httpPostTokenCommand;
            _HttpPostValidateCommand = httpPostValidateCommand;
            _HttpPostCallbackCommand = httpPostCallbackCommand;
            _DialogService = dialogService;
            _Logger = logger;
        }

        public async Task OnInitializedAsync(string qrBytesBase64)
        {
            _Logger.LogDebug($"Starting.");
            _Logger.BeginScope(nameof(OnInitializedAsync));
            try
            {
                try
                {
                    var valueBytes = Convert.FromBase64String(qrBytesBase64);
                    var jsonData = Encoding.UTF8.GetString(valueBytes);
                    InitiatingQrPayload = JsonConvert.DeserializeObject<InitiatingQrPayload>(jsonData);
                    _InitiatingQrPayloadToken = InitiatingQrPayload?.Token.ToJwtSecurityToken();
                    _Logger.LogInformation($"Initiating QR Code: {jsonData}");
                }
                catch (Exception e)
                {
                    DebugMessages.Add($"Error reading Initiating QR Code: {e}");
                    _Logger.LogError($"Error reading Initiating QR Code: {e}");
                    WorkflowMessage = "Could not understand QR code. Unable to continue.";
                    Exiting = true;
                    return;
                }

                IdentityResponse airlineIdentity;
                try
                {
                    airlineIdentity = await _HttpGetIdentityCommand.ExecuteAsync(InitiatingQrPayload.ServiceIdentity);
                }
                catch (Exception e)
                {
                    DebugMessages.Add($"Error contacting service provider: {e}");
                    _Logger.LogError($"Error contacting service provider: {e}");
                    WorkflowMessage = "Error contacting service provider. Unable to continue.";
                    Exiting = true;
                    return;
                }

                try
                {
                    _AccessTokenServiceUrl = airlineIdentity.GetServiceUrl("AccessTokenService");
                    _ResultTokenServiceUrl = airlineIdentity.GetServiceUrl("ResultTokenService");
                    _Logger.LogInformation($"AccessTokenServiceUrl :{_AccessTokenServiceUrl}");
                    _Logger.LogInformation($"ResultTokenServiceUrl :{_ResultTokenServiceUrl}");
                    DebugMessages.Add($"AccessTokenServiceUrl :{_AccessTokenServiceUrl}");
                    DebugMessages.Add($"ResultTokenServiceUrl :{_ResultTokenServiceUrl}");
                    ShowDccUpload = true;
                }
                catch (Exception e)
                {
                    var m = $"Error reading identity document of service provider: {e}";
                    DebugMessages.Add(m);
                    _Logger.LogError(m);
                    Exiting = true;
                }

            }
            finally
            {
                _Logger.LogDebug("End initialise.");
            }
        }


        /// <summary>
        /// After DCC is extracted from provided file.
        /// </summary>
        /// <param name="dccQrData"></param>
        /// <returns></returns>
        public async Task ValidateDccAsync(string dccQrData)
        {
            _Logger.LogDebug("Start validation.");
            var args = new HttpPostValidateArgs();

            var consented = await _DialogService.ShowMessageBox("Continue?", InitiatingQrPayload.Consent.Split(";")[0], yesText: "Yes", noText: "No");

            if (!consented ?? false)
            {
                WorkflowMessage = "You have chosen not to consent to send your DCC for verification at this time.";
                Exiting = true;
                return;
            }


            try
            {
                DebugMessages.Add($"Uploaded QR Data starts with: {dccQrData?.Substring(0, 10)}");

                if (!dccQrData.IsInternationalDccString())
                {
                    _Logger.LogInformation("Invalid DCC QR code (did not start with 'HC1:').");
                    DebugMessages.Add("Invalid DCC QR code (did not start with 'HC1:').");
                    WorkflowMessage = "Could not find an international DCC QR code in the file provided.";
                    return;
                }

                try
                {
                    _WalletSigningKeyPair = Crypto.GenerateEcKeyPair();
                    _Logger.LogInformation("Wallet key pair generated.");
                }
                catch (Exception e) //TODO narrower...
                {
                    var m = $"Did not generate wallet key pair: {e}";
                    _Logger.LogError(m);
                    DebugMessages.Add(m);
                    WorkflowMessage = "Found a QR code in the file provided but it was not the international version.";
                    Exiting = true;
                    return;
                }

                try
                {
                    _PostTokenResult = await _HttpPostTokenCommand.ExecuteAsync(new HttpPostTokenCommandArgs
                    {
                        AccessTokenServiceUrl = _AccessTokenServiceUrl,
                        InitiatingToken = InitiatingQrPayload.Token,
                        WalletPublicKey = _WalletSigningKeyPair.Public,
                    });
                    var m = "POST token completed.";
                    _Logger.LogInformation(m);
                    DebugMessages.Add(m);
                }
                catch (Exception e) //TODO narrower...
                {
                    var m = $"POST token failed: {e}";
                    _Logger.LogError(m);
                    DebugMessages.Add(m);
                    WorkflowMessage = "Could not complete initialisation of validation. Please try again later.";
                    Exiting = true;
                    return;
                }

                try
                {
                    var encKeyJwk = JsonConvert.DeserializeObject<PublicKeyJwk>(Encoding.UTF8.GetString(Convert.FromBase64String(_PostTokenResult.EncKeyBase64)));
                    args.InitiatingQrPayloadToken = _InitiatingQrPayloadToken;
                    args.DccQrCode = dccQrData;
                    args.PublicKeyJwk = encKeyJwk;
                    args.IV = _PostTokenResult.Nonce;
                    args.WalletPrivateKey = _WalletSigningKeyPair.Private;
                    args.ValidatorAccessTokenObject = _PostTokenResult.ValidationAccessTokenPayload;
                    args.ValidatorAccessToken = _PostTokenResult.ValidationAccessToken;
                    _HttpPostValidateResult = await _HttpPostValidateCommand.Execute(args);
                    var m = "POST validate completed.";
                    _Logger.LogInformation(m);
                    DebugMessages.Add(m);
                }
                catch (Exception e) //TODO narrower...
                {
                    var m = $"POST validate failed: {e}";
                    _Logger.LogError(m);
                    DebugMessages.Add(m);
                    WorkflowMessage = "Could not complete validation. Please try again later.";
                    Exiting = true;
                    return;
                }

                DebugMessages.Add($"Validation response: {JsonConvert.SerializeObject(_HttpPostValidateResult)}");
                ResultToken = (JwtSecurityToken)(new JwtSecurityTokenHandler().ReadToken(_HttpPostValidateResult.Content));

                _ResultClaimValue = ResultToken.Claims.FirstOrDefault(x => x.Type == "result")?.Value;
                if (_ResultClaimValue == null)
                {
                    var m = $"Result is missing entry for 'result'.";
                    _Logger.LogError(m);
                    DebugMessages.Add(m);
                    WorkflowMessage = "Unexpected response from validation service. Please try again later.";
                    Exiting = true;
                    return;
                }

                var validationSucceeded = _ResultClaimValue!.Equals("OK", StringComparison.InvariantCultureIgnoreCase);
                if (!validationSucceeded)
                {
                    try
                    {
                        FailureMessages = ResultToken.Claims
                            .Where(x => x.Type == "results") //Flat and repeated.
                            .Select(x => JsonConvert.DeserializeObject<FailureItem>(x?.Value))
                            .ToArray();

                        if (FailureMessages.Length == 0)
                        {
                            FailureMessages = new[] { new FailureItem { type = "-", ruleIdentifier = "-", customMessage = "DCC Health Certificate QR Code could not be verified." } };
                        }

                        AfterDccValidation = true;
                        ShowNotify = false;
                        Exiting = true;
                        return;
                    }
                    catch (Exception ex) //TODO narrower!
                    {
                        var m = $"Result could not parse entry for Validation Failures: {ex}";
                        _Logger.LogError(m);
                        DebugMessages.Add(m);
                        WorkflowMessage = "Unexpected response from validation service. Please try again later.";
                        return;
                    }
                }

                //Validation succeeded - parse results
                try
                {
                    var dccExtractJson = ResultToken.Claims.FirstOrDefault(x => x.Type == "personalinfodccextract")?.Value;
                    if (dccExtractJson == null)
                    {
                        var m = $"Result is missing entry for 'personalInfoExtract'.";
                        _Logger.LogError(m);
                        DebugMessages.Add(m);
                        WorkflowMessage = "Unexpected response from validation service. Please try again later.";
                        Exiting = true;
                    }
                    DccExtract = JsonConvert.DeserializeObject<DccExtract>(dccExtractJson);
                    AfterDccValidation = true;
                    ShowNotify = true;
                }
                catch (Exception ex) //TODO narrower!
                {
                    var m = $"Result could not parse entry for DCC Extract: {ex}";
                    _Logger.LogError(m);
                    DebugMessages.Add(m);
                    WorkflowMessage = "Unexpected response from validation service. Please try again later.";
                }
            }
            catch (Exception e) //TODO narrower!
            {
                var m = $"Unexpected error {e}.";
                _Logger.LogError(m);
                DebugMessages.Add(m);
                WorkflowMessage = "There was an unexpected error while validating. Please try again later.";
                Exiting = true;
            }
            finally
            {
                //Hint to memory manager
                args = null;
                dccQrData = null;
                _Logger.LogDebug("End validation.");
            }
        }

        public async Task NotifyServiceProvider()
        {
            _Logger.LogDebug("Start notification.");

            var consented = await _DialogService.ShowMessageBox("Continue?", InitiatingQrPayload.Consent.Split(";")[^1], yesText: "Yes", noText: "No");
            if (!consented ?? false)
            {
                WorkflowMessage = "You have chosen not to consent to send your result to the service provider at this time.";
                Exiting = true;
                return;
            }

            try
            {
                var args = new HttpPostCallbackCommandArgs
                {
                    EndpointUri = _ResultTokenServiceUrl,
                    ResultToken = ResultToken.Claims.First(x => x.Type == "confirmation").Value,
                    ValidationAccessToken = _PostTokenResult.ValidationAccessToken
                };

                if (!await _HttpPostCallbackCommand.ExecuteAsync(args))
                {
                    var m = $"Could not complete validation. Please try again later.";
                    _Logger.LogError(m);
                    DebugMessages.Add(m);
                    WorkflowMessage = "Unexpected response notifying service provider. Please try again later.";
                    Exiting = true;
                }
                else
                {
                    var m = $"Service provider notified.";
                    _Logger.LogInformation(m);
                    DebugMessages.Add(m);
                    WorkflowMessage = "You have completed verification and notified the service provider.";
                    Exiting = true;
                }
            }
            catch (Exception e)
            {
                var m = $"Unexpected error {e}.";
                _Logger.LogError(m);
                DebugMessages.Add(m);
                WorkflowMessage = "There was an unexpected error while notifying the service provider. Please try again later.";
                Exiting = true;
            }
            finally
            {
                _Logger.LogDebug("End notification.");
            }
        }
    }
}
