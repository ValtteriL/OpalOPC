using System.Reflection;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using Model;

namespace View
{
    public interface IHtmlReporter : IReporter
    {
    }

    public class HtmlReporter : IHtmlReporter
    {
        public void WriteReportToStream(Report report, Stream outputStream)
        {
            // register helpers
            IHandlebars handlebarsContext = Handlebars.Create();
            HandlebarsHelpers.Register(handlebarsContext);

            HandlebarsTemplate<object, object> template = handlebarsContext.Compile(getHtmlTemplate());

            string html = template(report);

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
