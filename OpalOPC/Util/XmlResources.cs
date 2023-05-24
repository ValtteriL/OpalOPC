namespace Util
{
    public static class XmlResources
    {
#if DEBUG
        public static string StylesheetLocation = "http://localhost:8000/OpalOPC/report-resources/stylesheet.xsl";
#else
        public static string StylesheetLocation = $"https://opalopc.com/report-resources/{Util.VersionUtil.AppAssemblyVersion!.ToString()}/stylesheet.xsl";
#endif
    }
}