using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using CheckInQrWeb.Core.Models;
using Newtonsoft.Json;

namespace CheckInQrWeb.Core;

public class HttpPostValidateCommand 
{
    private readonly IHttpClientFactory _HttpClientFactory;

    public HttpPostValidateCommand(IHttpClientFactory httpClientFactory)
    {
        _HttpClientFactory = httpClientFactory;
    }

    public async Task<HttpPostValidateResult> Execute(HttpPostValidateArgs args)
    {
        var secretKey = RandomNumberGenerator.GetBytes(32);
        var encryptedDcc = Crypto.EncryptAesCbc(Encoding.UTF8.GetBytes(args.DccQrCode), secretKey, args.IV);

        var publicKey = Crypto.GetRsaPublicKey(args.PublicKeyJwk.x5c[0]);
        var encryptedSecretKey = publicKey.Encrypt(secretKey, RSAEncryptionPadding.OaepSHA256);

        var digest = Crypto.Digest(encryptedDcc, args.WalletPrivateKey);

        var body = new ValidationRequestBody
        {
            kid = args.PublicKeyJwk.kid,
            dcc = Convert.ToBase64String(encryptedDcc),
            sig = Convert.ToBase64String(digest),
            sigAlg = "SHA256withECDSA",
            encKey = Convert.ToBase64String(encryptedSecretKey),
            encScheme = "RsaOaepWithSha256AesCbcScheme",
        };

        var endpoint = args.ValidatorAccessTokenObject.Payload["validationUrl"].ToString(); //TODO should be properly decoded and WTF is not aud? :-/ Also, why is this from the airline?

        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Headers =
            {
                { "authorization", new AuthenticationHeaderValue("Bearer", args.ValidatorAccessToken).ToString() },
                {"X-Version", "2.00"}
            },
            Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")
        };

        using var httpClient = _HttpClientFactory.CreateClient(nameof(HttpPostValidateCommand));
        var httpResponse = await httpClient.SendAsync(httpRequestMessage);
        var content = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
            throw new Exception($"Http response code: {httpResponse.StatusCode}, message: {content}");

        return new()
        {
            Content = content
        };
    }
}