namespace Util
{
    public static class XmlResources
    {
        public static string StylesheetLocation = "OpalOPC.report_resources.stylesheet.xsl";
        public static string DebugResourcePath = "http://localhost:8000/OpalOPC/report-resources";
        public static string ProdResourcePath = $"https://opalopc.com/report-resources/{ VersionUtil.AppAssemblyVersion }";
    }
}