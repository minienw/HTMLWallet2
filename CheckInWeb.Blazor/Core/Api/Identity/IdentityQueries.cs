using System.Text.RegularExpressions;

namespace CheckInWeb.Blazor.Core.Api.Identity;

public static class IdentityQueries
{
    private const string RegexValue = "^http.*#ConfirmationService(?:[-]{1}[0-9]+)?$";
    

    public static string GetServiceUrl(this IdentityResponse identityResponse, string serviceName)
    {
        return identityResponse.service.SingleOrDefault(x => Hit(x.id, serviceName))?.serviceEndpoint
           ?? throw new InvalidOperationException($"Cannot retrieve {serviceName} url");

    }
    public static bool Hit(string value, string serviceName)
    {
        var r  = new Regex($"^http.*#{serviceName}(?:[-]{{1}}[0-9]+)?$", RegexOptions.IgnoreCase);
        return r.IsMatch(value);
    }
}