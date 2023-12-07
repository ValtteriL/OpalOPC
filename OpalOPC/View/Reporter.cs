using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using Model;
using HandlebarsDotNet;
using System.Text.Json;

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
            RegisterHelpers();
            HandlebarsTemplate<object, object> template = Handlebars.Compile(getHtmlTemplate());

            string html = template(report);

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
            Stream finalstream = stream;

#if !DEBUG
            // Replace debug paths in XSLT
            StreamReader reader = new(stream);
            string xsltString = reader.ReadToEnd();
            xsltString = xsltString.Replace(Util.XmlResources.DebugResourcePath, Util.XmlResources.ProdResourcePath);

            // Write corrected XSLT
            // and adjust stream for reader
            MemoryStream ms = new();
            StreamWriter writer = new(ms)
            {
                AutoFlush = true
            };
            writer.Write(xsltString);

            ms.Seek(0, SeekOrigin.Begin);
            finalstream = ms;
#endif

            StreamReader finalReader = new(finalstream);
            return finalReader.ReadToEnd();
        }

        static void RegisterHelpers()
        {
            HandlebarsDotNet.Handlebars.RegisterHelper("ifCond", (output, options, context, arguments) =>
            {
                if (arguments.Length != 3)
                {
                    throw new HandlebarsException("{{ifCond}} helper must have three arguments");
                }

                string v1 = arguments.At<string>(0);
                string @operator = arguments.At<string>(1);
                string v2 = arguments.At<string>(2);

                switch (@operator)
                {
                    case "==":
                        if (v1 == v2)
                        {
                            options.Template(output, context);
                        }
                        else
                        {
                            options.Inverse(output, context);
                        }
                        break;

                    case "!=":
                        if (v1 != v2)
                        {
                            options.Template(output, context);
                        }
                        else
                        {
                            options.Inverse(output, context);
                        }
                        break;

                    case "<":
                        if (Convert.ToDouble(v1) < Convert.ToDouble(v2))
                        {
                            options.Template(output, context);
                        }
                        else
                        {
                            options.Inverse(output, context);
                        }
                        break;

                    case "<=":
                        if (Convert.ToDouble(v1) <= Convert.ToDouble(v2))
                        {
                            options.Template(output, context);
                        }
                        else
                        {
                            options.Inverse(output, context);
                        }
                        break;

                    case ">":
                        if (Convert.ToDouble(v1) > Convert.ToDouble(v2))
                        {
                            options.Template(output, context);
                        }
                        else
                        {
                            options.Inverse(output, context);
                        }
                        break;

                    case ">=":
                        if (Convert.ToDouble(v1) >= Convert.ToDouble(v2))
                        {
                            options.Template(output, context);
                        }
                        else
                        {
                            options.Inverse(output, context);
                        }
                        break;

                    case "&&":
                        if (Convert.ToBoolean(v1) && Convert.ToBoolean(v2))
                            options.Template(output, context);
                        else
                            options.Inverse(output, context);
                        break;

                    case "||":
                        if (Convert.ToBoolean(v1) || Convert.ToBoolean(v2))
                            options.Template(output, context);
                        else
                            options.Inverse(output, context);
                        break;
                }
            });
        }
    }
}
