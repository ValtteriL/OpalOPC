using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using Model;
using HandlebarsDotNet;
using System.Text.Json;
using HandlebarsDotNet.Helpers;

namespace View
{
    public interface IReporter
    {
        public void PrintXHTMLReport(Report report);
    }

    public class Reporter : IReporter
    {
        private readonly Stream _outputStream;

        public Reporter(Stream outputStream)
        {
            _outputStream = outputStream;
        }

        public void PrintXHTMLReport(Report report)
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
            StreamWriter writer = new(_outputStream)
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
