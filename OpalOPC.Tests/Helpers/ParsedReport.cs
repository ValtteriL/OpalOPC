using System.Text.RegularExpressions;

namespace Tests.Helpers
{
    public partial class ParsedReport
    {

        public int NumberOfTargets { get; private set; }
        public int NumberOfEndpoints { get; private set; }
        public int NumberOfIssues { get; private set; }
        public int NumberOfCriticalIssues { get; private set; }
        public int NumberOfHighIssues { get; private set; }
        public int NumberOfMediumIssues { get; private set; }
        public int NumberOfLowIssues { get; private set; }
        public int NumberOfInfoIssues { get; private set; }
        public int NumberOfErrors { get; private set; }
        public List<int> IssueIds { get; private set; } = [];


        public ParsedReport(string reportContent)
        {
            NumberOfTargets = int.Parse(NumberOfTargetsRegex().Match(reportContent).Groups[1].Value);

            NumberOfEndpoints = NumberOfEndpointsRegex().Matches(reportContent).Count;

            NumberOfCriticalIssues = NumberOfCriticalIssuesRegex().Matches(reportContent).Count;
            NumberOfHighIssues = NumberOfHighIssuesRegex().Matches(reportContent).Count;
            NumberOfMediumIssues = NumberOfMediumIssuesRegex().Matches(reportContent).Count;
            NumberOfLowIssues = NumberOfLowIssuesRegex().Matches(reportContent).Count;
            NumberOfInfoIssues = NumberOfInfoIssuesRegex().Matches(reportContent).Count;

            NumberOfErrors = NumberOfErrorsRegex().Matches(reportContent).Count;

            // number of issues is sum of all severities
            NumberOfIssues = NumberOfCriticalIssues + NumberOfHighIssues + NumberOfMediumIssues + NumberOfLowIssues + NumberOfInfoIssues;

            foreach (Match match in IssueIdRegex().Matches(reportContent).Cast<Match>())
            {
                IssueIds.Add(int.Parse(match.Groups[1].Value));
            }
        }

        [GeneratedRegex(@"\(([0-9]+) applications found\)")]
        private static partial Regex NumberOfTargetsRegex();
        [GeneratedRegex(@"class=""discoveryurl""")]
        private static partial Regex NumberOfEndpointsRegex();
        [GeneratedRegex(@"Critical \(")]
        private static partial Regex NumberOfCriticalIssuesRegex();
        [GeneratedRegex(@"High \(")]
        private static partial Regex NumberOfHighIssuesRegex();
        [GeneratedRegex(@"Medium \(")]
        private static partial Regex NumberOfMediumIssuesRegex();
        [GeneratedRegex(@"Low \(")]
        private static partial Regex NumberOfLowIssuesRegex();
        [GeneratedRegex(@"Info \(")]
        private static partial Regex NumberOfInfoIssuesRegex();
        [GeneratedRegex(@"/docs/plugin-([0-9]+)")]
        private static partial Regex IssueIdRegex();
        [GeneratedRegex(@"class=""error""")]
        private static partial Regex NumberOfErrorsRegex();
    }
}
