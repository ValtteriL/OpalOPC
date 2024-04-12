using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis.Sarif;
using Model;
using Plugin;

namespace View
{
    public interface ISarifReporter : IReporter
    {
    }
    public class SarifReporter(IPluginRepository pluginRepository) : ISarifReporter
    {
        public const string ErrorRuleId = "-1";
        private readonly IList<ReportingDescriptor> _rules = BuildRules(pluginRepository);
        private readonly Guid _guid = new("9ff60239-60d4-4715-afa8-bfe993f72824");
        private static readonly ReportingDescriptor s_errorRule = new()
        {
            Id = ErrorRuleId,
            Name = "Error",
            DefaultConfiguration = new ReportingConfiguration
            {
                Level = FailureLevel.Warning,
                Rank = 0
            }
        };

        public void WriteReportToStream(Report report, Stream outputStream)
        {
            SarifLog log = BuildSarifLog(report);
            log.Save(outputStream);
        }

        private SarifLog BuildSarifLog(Report report)
        {
            SarifLog log = new()
            {
                Runs = [
                    new Run
                    {
                        Tool = new Tool
                        {
                            Driver = new ToolComponent
                            {
                                Name = "OpalOPC",
                                InformationUri = new Uri("https://opalopc.com/"),
                                Guid = _guid,
                                Version = report.Version,
                                Rules = _rules,
                                ShortDescription = new MultiformatMessageString
                                {
                                    Text = "OpalOPC is a security scanner for OPC UA applications"
                                }
                            }
                        },
                        Results = BuildResults(report),
                        Invocations = [
                            new Invocation
                            {
                                CommandLine = report.Command,
                                StartTimeUtc = report.StartTime.ToUniversalTime(),
                                EndTimeUtc = report.EndTime.ToUniversalTime(),
                                ExitCode = 0, // Assuming success if got this far
                                ExecutionSuccessful = true, // Assuming success if got this far
                            }
                        ],
                        Artifacts = BuildArtifacts(report),
                    }
                ]
            };
            return log;
        }

        private static IList<Artifact> BuildArtifacts(Report report)
        {
            IList<Artifact> artifacts = [];
            foreach (Target target in report.Targets)
            {
                foreach (Server server in target.Servers)
                {
                    artifacts.Add(BuildArtifact(server, target.ApplicationName));
                }
            }
            return artifacts;
        }

        private static Artifact BuildArtifact(Server server, string applicationName)
        {
            return new Artifact
            {
                Description = new Message
                {
                    Text = $"Endpoint of {applicationName}"
                },
                Roles = ArtifactRoles.AnalysisTarget,
                Location = new ArtifactLocation
                {
                    Uri = new Uri(server.DiscoveryUrl)
                }
            };
        }


        private IList<Result> BuildResults(Report report)
        {
            IList<Result> results = [];
            foreach (Target target in report.Targets)
            {
                foreach (Server server in target.Servers)
                {
                    foreach (Issue issue in server.Issues)
                    {
                        results.Add(BuildResult(issue, server));
                    }
                    foreach (Error error in server.Errors)
                    {
                        results.Add(BuildErrorResult(error, server));
                    }
                }
            }
            return results;
        }

        private Result BuildErrorResult(Error error, Server server)
        {
            return new Result
            {
                RuleId = s_errorRule.Id,
                RuleIndex = _rules.IndexOf(s_errorRule), // RuleIndex is used to connect the result to the rule
                Message = new Message
                {
                    Text = error.Message,
                },
                Locations = [
                    new Location
                    {
                        PhysicalLocation = new PhysicalLocation
                        {
                            ArtifactLocation = new ArtifactLocation
                            {
                                Uri = new Uri(server.DiscoveryUrl)
                            }
                        }
                    }
                ],
                Level = FailureLevel.Warning,
                Rank = 0, // No CVSS for errors
                PartialFingerprints = new Dictionary<string, string>
                {
                    ["1"] = GeneratePartialFingerPrint(s_errorRule.Id, server.DiscoveryUrl, error.Message)
                }
            };
        }

        private Result BuildResult(Issue issue, Server server)
        {
            ReportingDescriptor correspondingRule = _rules.First(r => r.Id == $"{issue.PluginIdInt}");
            int ruleIndex = _rules.IndexOf(correspondingRule);

            return new Result
            {
                RuleId = $"{issue.PluginIdInt}",
                RuleIndex = ruleIndex, // RuleIndex is used to connect the result to the rule
                Message = new Message
                {
                    Text = issue.Name,
                },
                Locations = [
                    new Location
                    {
                        PhysicalLocation = new PhysicalLocation
                        {
                            ArtifactLocation = new ArtifactLocation
                            {
                                Uri = new Uri(server.DiscoveryUrl)
                            }
                        }
                    }
                ],
                Level = issue.Severity > 0 ? FailureLevel.Error : FailureLevel.Note,
                Rank = issue.Severity * 10, // CVSS * 10
                PartialFingerprints = new Dictionary<string, string>
                {
                    ["1"] = GeneratePartialFingerPrint(issue.PluginIdInt.ToString(), server.DiscoveryUrl, issue.Name)
                }
            };
        }

        private string GeneratePartialFingerPrint(string ruleId, string discoveryUrl, string message)
        {
            // generate a determenistic hash of the issue/error to be used as a partial fingerprint

            string unhashedPartialFingerPrint = $"{_guid}{ruleId}{discoveryUrl}{message}";

            return BitConverter.ToString(MD5.HashData(Encoding.ASCII.GetBytes(unhashedPartialFingerPrint)))
                .Replace("-", string.Empty)
                .ToLower();
        }

        private static IList<ReportingDescriptor> BuildRules(IPluginRepository pluginRepository)
        {
            IList<ReportingDescriptor> rules = [];
            foreach (IPlugin plugin in pluginRepository.GetAll())
            {
                rules.Add(BuildRule(plugin));
            }
            rules.Add(s_errorRule);
            return rules;
        }

        private static ReportingDescriptor BuildRule(IPlugin plugin)
        {
            return new ReportingDescriptor
            {
                Id = $"{(int)plugin.Id}",
                Name = plugin.Name,
                HelpUri = new Uri($"https://opalopc.com/docs/plugin-{(int)plugin.Id}/"),
                DefaultConfiguration = new ReportingConfiguration
                {
                    // if severity is 0, it's a note, otherwise its an error
                    Level = plugin.Severity > 0 ? FailureLevel.Error : FailureLevel.Note,
                    Rank = plugin.Severity * 10 // CVSS * 10
                }
            };
        }
    }
}
