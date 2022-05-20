using CheckInWeb.Blazor.Core.Api.Identity;
using Newtonsoft.Json;

namespace CheckInWeb.Blazor.Core;

public sealed class HttpGetIdentityCommand
{
    private readonly IHttpClientFactory _HttpClientFactory;

    public HttpGetIdentityCommand(IHttpClientFactory httpClientFactory)
    {
        _HttpClientFactory = httpClientFactory;
    }

    public async Task<IdentityResponse> ExecuteAsync(string url)
    {
        using var httpClient = _HttpClientFactory.CreateClient(nameof(HttpGetIdentityCommand));
        var r = await httpClient.GetAsync(url);
        var jsonData = await r.Content.ReadAsStringAsync();

        //if (string.IsNullOrWhiteSpace(jsonData))
        //    throw new InvalidOperationException($"Getting the validation service description data failed: no valid data received from service with url {url}");

        //var result = JsonConvert.DeserializeObject<IdentityResponse>(jsonData)
        //             ?? throw new InvalidOperationException($"Deserializing the validation service description data failed: no valid data received from service with url {url}");

        return JsonConvert.DeserializeObject<IdentityResponse>(jsonData);
    }
}