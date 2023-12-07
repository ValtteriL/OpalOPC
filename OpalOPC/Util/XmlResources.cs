namespace Util
{
    public static class XmlResources
    {
        public static readonly string HtmlTemplateLocation = "OpalOPC.report_resources.report-template.html";
        public static readonly string DebugResourcePath = "http://localhost:8000/OpalOPC/report-resources";
        public static readonly string ProdResourcePath = $"https://opalopc.com/report-resources/{VersionUtil.AppAssemblyVersion}";
    }
}
