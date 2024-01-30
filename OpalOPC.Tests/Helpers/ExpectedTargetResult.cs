using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;
using Tests.Helpers;
using Xunit;

namespace Tests.Helpers
{
    public class ExpectedTargetResult(int numberOfEndpoints, int numberOfErrors, int numberOfIssues, int numberOfCriticalIssues, int numberOfHighIssues, int numberOfMediumIssues, int numberOfLowIssues, int numberOfInfoIssues, List<int> issueIds)
    {
        public readonly int NumberOfEndpoints = numberOfEndpoints;
        private readonly List<int> _issueIds = issueIds;
        public readonly int NumberOfErrors = numberOfErrors;
        public readonly int NumberOfIssues = numberOfIssues;
        private readonly int _numberOfCriticalIssues = numberOfCriticalIssues;
        private readonly int _numberOfHighIssues = numberOfHighIssues;
        private readonly int _numberOfMediumIssues = numberOfMediumIssues;
        private readonly int _numberOfLowIssues = numberOfLowIssues;
        private readonly int _numberOfInfoIssues = numberOfInfoIssues;

        public void validateWithParsedReport(ParsedReport parsedReport)
        {
            Assert.True(parsedReport.NumberOfEndpoints >= NumberOfEndpoints);
            Assert.True(parsedReport.NumberOfIssues >= NumberOfIssues);
            Assert.True(parsedReport.NumberOfCriticalIssues >= _numberOfCriticalIssues);
            Assert.True(parsedReport.NumberOfHighIssues >= _numberOfHighIssues);
            Assert.True(parsedReport.NumberOfMediumIssues >= _numberOfMediumIssues);
            Assert.True(parsedReport.NumberOfLowIssues >= _numberOfLowIssues);
            Assert.True(parsedReport.NumberOfInfoIssues >= _numberOfInfoIssues);
            Assert.True(parsedReport.NumberOfErrors >= NumberOfErrors);

            foreach (int issueId in _issueIds)
            {
                Assert.Contains(issueId, parsedReport.IssueIds);
            }
        }

        public static readonly ExpectedTargetResult Echo = new(
            numberOfEndpoints: 2,
            numberOfErrors: 1,
            numberOfIssues: 7,
            numberOfCriticalIssues: 0,
            numberOfHighIssues: 1,
            numberOfMediumIssues: 5,
            numberOfLowIssues: 0,
            numberOfInfoIssues: 1,
            issueIds: [10001, 10002, 10004, 10006, 10007, 10008, 10009]
        );

        public static readonly ExpectedTargetResult Golf = new(
            numberOfEndpoints: 1,
            numberOfErrors: 0,
            numberOfIssues: 2,
            numberOfCriticalIssues: 0,
            numberOfHighIssues: 1,
            numberOfMediumIssues: 1,
            numberOfLowIssues: 0,
            numberOfInfoIssues: 0,
            issueIds: [10001, 10007]
            );

        public static readonly ExpectedTargetResult India = new(
            numberOfEndpoints: 1,
            numberOfErrors: 0,
            numberOfIssues: 1,
            numberOfCriticalIssues: 0,
            numberOfHighIssues: 1,
            numberOfMediumIssues: 0,
            numberOfLowIssues: 0,
            numberOfInfoIssues: 0,
            issueIds: [10001]
            );

        public static readonly ExpectedTargetResult Scanme = new(
            numberOfEndpoints: 1,
            numberOfErrors: 0,
            numberOfIssues: 7,
            numberOfCriticalIssues: 0,
            numberOfHighIssues: 1,
            numberOfMediumIssues: 5,
            numberOfLowIssues: 0,
            numberOfInfoIssues: 1,
            issueIds: [10001, 10002, 10004, 10006, 10007, 10008, 10009]
            );

        public static readonly ExpectedTargetResult Invalid = new(
            numberOfEndpoints: 0,
            numberOfErrors: 0,
            numberOfIssues: 0,
            numberOfCriticalIssues: 0,
            numberOfHighIssues: 0,
            numberOfMediumIssues: 0,
            numberOfLowIssues: 0,
            numberOfInfoIssues: 0,
            issueIds: []
            );
    }
}
