using CheckInQrWeb.Core.Models;
using CheckInQrWeb.Core.Models.api.Identity;

namespace CheckInQrWeb.Core;

public static class IdentityQueries
{
    public static string GetServiceUrl(this IdentityResponse identityResponse, string serviceName)
        => identityResponse.service.SingleOrDefault(x => x.id.EndsWith(serviceName))?.serviceEndpoint
           ?? throw new InvalidOperationException($"Cannot retrieve {serviceName} url");
    public static PublicKeyJwk GetRsaEncryptionPublicKey(this IdentityResponse identityResponse)
        => identityResponse.verificationMethod.SingleOrDefault(x => x.id.EndsWith("ValidationServiceEncKey-1"))?.publicKeyJwk
           ?? throw new InvalidOperationException($"Cannot retrieve validation service encryption key.");
}