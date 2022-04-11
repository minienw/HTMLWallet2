using System.IO;

namespace CheckIn.Shared.Tests;

public static class Paths
{
    public static string SourceDocument1 => Path.Combine(Path.GetDirectoryName(NCrunch.Framework.NCrunchEnvironment.GetOriginalProjectPath())
        , @"Test Cert SK 2022-01.pdf");
    public static string SourceDocument2 => Path.Combine(Path.GetDirectoryName(NCrunch.Framework.NCrunchEnvironment.GetOriginalProjectPath())
        , @"Vacc Cert SK 2022-01.pdf");
}