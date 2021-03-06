namespace CheckInQrWeb.Core.Models.api.Identity;

public class VerificationMethod
{
    public string id { get; set; }
    public string type { get; set; }
    public string controller { get; set; }
    public PublicKeyJwk publicKeyJwk { get; set; }
    public string[] verificationMethods { get; set; }
}