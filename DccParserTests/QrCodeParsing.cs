using System.IO;
using CheckInQrWeb.Core;
using NCrunch.Framework;
using Xunit;

namespace CheckInQrWebTests;

public class QrCodeParsing
{
    [ExclusivelyUses("files")]
    [InlineData("Bobby.JPG")]
    [InlineData("Bob_Bouwer_1-1-1960_3_3.JPG")]
    [InlineData("Erikea Musermann 12-8-1964.png")]
    //[InlineData("bobby_bouwer is ouwer - fails AF.png")] Dodgy!
    //[InlineData("bobby_bouwer is ouwer - cropped.png")] Dodgy!
    [InlineData("Something dodgy VAC_DE.png")]
    [InlineData("bobby_bower_2021_minor.png")]
    [InlineData("ron de boer.png")]
    [Theory]
    public void Parse(string name)
    {
        using var f =
            File.OpenRead(Path.Combine(Path.GetDirectoryName(NCrunchEnvironment.GetOriginalProjectPath()), name));
        var decoder = new ImageParser();
        Assert.True(decoder.TryParse(f, 1024L, out var result));
        Assert.StartsWith("HC1:", result);
    }
}