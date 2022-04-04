using System.Security.Cryptography;
using CheckInQrWeb.Core.Models;

namespace CheckInQrWeb.Core;


public class HttpPostCallbackCommandArgs
{
    public string ResultToken { get; set; }
    public string EndpointUri { get; set; }
    public string ValidationAccessToken { get; set; }
}