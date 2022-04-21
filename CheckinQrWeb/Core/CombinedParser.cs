using CheckInQrWeb.Core.PdfParsing;
using Microsoft.AspNetCore.Components.Forms;

namespace CheckInQrWeb.Core;

public class CombinedParser
{
    private readonly ILogger<CombinedParser> _Logger;

    public CombinedParser(ILogger<CombinedParser> logger)
    {
        _Logger = logger;
    }

    public string Parse(Stream file, string name, long size)
    {
        string result;
        if (name.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
        {
            if (TryParseAsPdf(file, out result))
            {
                _Logger.LogDebug($"Found in pdf: {result}.");
                return result;
            }
        }

        if (TryParseAsImage(file, size, out result))
        {
            _Logger.LogDebug($"Found in image: {result}.");
            return result;
        }

        return "Not a scooby";
    }

    private bool TryParseAsImage(Stream stream, long size, out string result)
    {
        _Logger.LogDebug("Try decoding as image...");
        var p = new ImageParser();
        return p.TryParse(stream, size, out result);
    }

    private bool TryParseAsPdf(Stream stream, out string result)
    {
        _Logger.LogDebug("Try decoding as PDF...");
        var p = new PdfParser(stream);
        return p.TryParse(out result);
    }
}