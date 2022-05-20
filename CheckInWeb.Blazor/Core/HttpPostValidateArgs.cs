using System.IdentityModel.Tokens.Jwt;
using CheckInWeb.Blazor.Core.Api.Identity;
using Org.BouncyCastle.Crypto;

namespace CheckInWeb.Blazor.Core
{
    public class HttpPostValidateArgs
    {
        public string DccQrCode { get; set; }
        public AsymmetricKeyParameter WalletPrivateKey { get; set; }
        public JwtSecurityToken InitiatingQrPayloadToken { get; set; }
        public byte[] IV { get; set; }
        public PublicKeyJwk PublicKeyJwk { get; set; }
        public JwtSecurityToken ValidatorAccessTokenObject { get; set; }
        public string ValidatorAccessToken { get; set; }
    }
}
