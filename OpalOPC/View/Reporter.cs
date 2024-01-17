using System.Reflection;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using Model;

namespace View
{
    public interface IReporter
    {
        public void PrintXHTMLReport(Report report, Stream outputStream);
    }

    public class Reporter : IReporter
    {
        public void PrintXHTMLReport(Report report, Stream outputStream)
        {
            // register helpers
            IHandlebars handlebarsContext = Handlebars.Create();
            HandlebarsHelpers.Register(handlebarsContext);

            HandlebarsTemplate<object, object> template = handlebarsContext.Compile(getHtmlTemplate());

            string html = template(report);

#if !DEBUG
            // Replace debug paths in XSLT
            html = html.Replace(Util.XmlResources.DebugResourcePath, Util.XmlResources.ProdResourcePath);
#endif

            // Write to output stream
            StreamWriter writer = new(outputStream)
            {
                AutoFlush = true
            };
            writer.Write(html);
        }

        private static string getHtmlTemplate()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using Stream? stream = assembly.GetManifestResourceStream(Util.XmlResources.HtmlTemplateLocation) ?? throw new FileNotFoundException($"Cannot find HTML template {Util.XmlResources.HtmlTemplateLocation}");
            StreamReader finalReader = new(stream);
            return finalReader.ReadToEnd();
        }


    }
}
