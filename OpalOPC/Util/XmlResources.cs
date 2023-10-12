namespace Util
{
    public static class XmlResources
    {
        public static string StylesheetLocation = "OpalOPC.report_resources.stylesheet.xsl";
#if DEBUG
        public static string DtdLocation = "OpalOPC/report-resources/report.dtd";
#else
        public static string DtdLocation = $"https://opalopc.com/report-resources/{Util.VersionUtil.AppAssemblyVersion!.ToString()}/report.dtd";
#endif
    }
}