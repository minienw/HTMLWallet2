using System.IO;
using CheckInQrWeb.Core;
using Xunit;

namespace CheckIn.Shared.Tests;

public class QrCodeParsing
{

    [Fact]
    public void Bob()
    {
        var decoder = new QrCodeService();
        var decoded = decoder.Decode(File.ReadAllBytes(Paths.Bob));
        Assert.True(decoded.StartsWith("HC1"));
    }

    [Fact]
    public void NotBob()
    {
        var decoder = new QrCodeService();
        var decoded = decoder.Decode(File.ReadAllBytes(Paths.NotBob));
        Assert.True(decoded.StartsWith("HC1"));

    }

    [Fact]
    public void BobFailsWithNull()
    {
        var decoder = new QrCodeService();
        var decoded = decoder.Decode(File.ReadAllBytes(Paths.BobFailsWithNull));
        Assert.True(decoded.StartsWith("HC1"));
    }

    [Fact]
    public void CroppedBobFailsWithNull()
    {
        var decoder = new QrCodeService();
        var decoded = decoder.Decode(File.ReadAllBytes(Paths.BobCroppedFailsWithNull));
        Assert.True(decoded.StartsWith("HC1"));
    }
}