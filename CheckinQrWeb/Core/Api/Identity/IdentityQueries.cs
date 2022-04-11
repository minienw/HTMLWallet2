using CheckInQrWeb.Core.Models;
using CheckInQrWeb.Core.Models.api.Identity;

namespace CheckInQrWeb.Core;

public static class IdentityQueries
{
    public static string GetServiceUrl(this IdentityResponse identityResponse, string serviceName)
        => identityResponse.service.SingleOrDefault(x => x.id.EndsWith(serviceName))?.serviceEndpoint
           ?? throw new InvalidOperationException($"Cannot retrieve {serviceName} url");
}