using System.IO;

namespace CheckIn.Shared.Tests;

public static class Paths
{
    public static string Bob => Path.Combine(Path.GetDirectoryName(NCrunch.Framework.NCrunchEnvironment.GetOriginalProjectPath())
        , @"Bob_Bouwer_1-1-1960_3_3.JPG");
    public static string NotBob => Path.Combine(Path.GetDirectoryName(NCrunch.Framework.NCrunchEnvironment.GetOriginalProjectPath())
        , @"Erikea Musermann 12-8-1964.png");

    public static string SourceDocument1 => Path.Combine(Path.GetDirectoryName(NCrunch.Framework.NCrunchEnvironment.GetOriginalProjectPath())
        , @"Test Cert SK 2022-01.pdf");
    public static string SourceDocument2 => Path.Combine(Path.GetDirectoryName(NCrunch.Framework.NCrunchEnvironment.GetOriginalProjectPath())
        , @"Vacc Cert SK 2022-01.pdf");
}