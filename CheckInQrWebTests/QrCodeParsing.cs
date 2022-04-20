using System.IO;
using CheckInQrWeb.Core;
using Xunit;

namespace CheckIn.Shared.Tests;

public class QrCodeParsing
{
    [InlineData("Bobby.JPG")]
    [InlineData("Bob_Bouwer_1-1-1960_3_3.JPG")]
    [InlineData("Erikea Musermann 12-8-1964.png")]
    [InlineData("bobby_bouwer is ouwer - fails AF.png")]
    [InlineData("bobby_bouwer is ouwer - cropped.png")]
    [InlineData("Something dodgy VAC_DE.png")]
    [Theory]
    public void Parse(string name)
    {
        var decoder = new QrCodeService();
        var decoded = decoder.Decode(File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(NCrunch.Framework.NCrunchEnvironment.GetOriginalProjectPath()), name)));
        Assert.True(decoded.StartsWith("HC1"));
    }
}