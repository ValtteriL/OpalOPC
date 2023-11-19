using Controller;
using crypto;
using Microsoft.Extensions.Logging;
using Model;
using Moq;
using Opc.Ua;
using View;
using Xunit;

namespace Tests;
public class ReportControllerTest
{
    private readonly Mock<ILogger> _loggerMock;
    private readonly EndpointDescription _endpointDescription = new()
    {
        UserIdentityTokens = new UserTokenPolicyCollection(new List<UserTokenPolicy> { new(UserTokenType.UserName) }),
        SecurityMode = MessageSecurityMode.None,
        EndpointUrl = "opc.tcp://localhost:4840",
    };
    private readonly Server _server;
    private readonly ApplicationDescription _applicationDescription = new()
    {
        ApplicationType = ApplicationType.Server,
        ApplicationName = "test",
        ApplicationUri = "test",
        ProductUri = "test",
    };

    private readonly Mock<IReporter> _reporterMock;

    public ReportControllerTest()
    {
        _loggerMock = new Mock<ILogger>();
        _reporterMock = new Mock<IReporter>();
        _server = new("opc.tcp://discoveryuri", new EndpointDescriptionCollection() { _endpointDescription });
    }

    // add test to check if targets are sorted by server issue severity
    [Fact]
    public void TestReportingWorksWithEmptyTargets()
    {
        // Arrange
        ReportController reportController = new(_loggerMock.Object, _reporterMock.Object);
        _reporterMock.Setup(r => r.PrintXHTMLReport(It.IsAny<Report>())).Verifiable();


        // Act
        reportController.GenerateReport(new List<Target>(), DateTime.Now, DateTime.Now, string.Empty);
        reportController.WriteReport(string.Empty);

        // Assert
        _reporterMock.Verify(r => r.PrintXHTMLReport(It.IsAny<Report>()), Times.Once);
        Assert.NotNull(reportController.report);
        Assert.Empty(reportController.report.Targets);
    }

    [Fact]
    public void TestReportingWorksWithSingleTarget()
    {
        // Arrange
        ReportController reportController = new(_loggerMock.Object, _reporterMock.Object);
        _reporterMock.Setup(r => r.PrintXHTMLReport(It.IsAny<Report>())).Verifiable();

        // Act
        reportController.GenerateReport(new List<Target>() { new(_applicationDescription) }, DateTime.Now, DateTime.Now, string.Empty);
        reportController.WriteReport(string.Empty);

        // Assert
        _reporterMock.Verify(r => r.PrintXHTMLReport(It.IsAny<Report>()), Times.Once);
        Assert.NotNull(reportController.report);
        Assert.NotEmpty(reportController.report.Targets);
        Assert.True(reportController.report.Targets.Count == 1);
    }

    // add test to check if targets are sorted by server issue severity
    [Fact]
    public void TestTargetServerEndpointSortingWorks()
    {
        // Arrange
        ReportController reportController = new(_loggerMock.Object, _reporterMock.Object);
        _reporterMock.Setup(r => r.PrintXHTMLReport(It.IsAny<Report>())).Verifiable();

        var target1 = new Target(_applicationDescription);
        target1.AddServer(new Server("opc.tcp://discoveryuri", new EndpointDescriptionCollection() { _endpointDescription }));
        target1.Servers.First().SeparatedEndpoints.First().Issues.Add(new Issue(1, "description", 0.1));

        var target2 = new Target(_applicationDescription);
        target2.AddServer(new Server("opc.tcp://discoveryuri", new EndpointDescriptionCollection() { _endpointDescription }));
        target2.Servers.First().SeparatedEndpoints.First().Issues.Add(new Issue(1, "description", 0.2));

        var target3 = new Target(_applicationDescription);
        target3.AddServer(new Server("opc.tcp://discoveryuri", new EndpointDescriptionCollection() { _endpointDescription }));
        target3.Servers.First().SeparatedEndpoints.First().Issues.Add(new Issue(1, "description", 0.3));

        // Act
        reportController.GenerateReport(new List<Target>() { target1, target2, target3 }, DateTime.Now, DateTime.Now, string.Empty);
        reportController.WriteReport(string.Empty);

        // Assert
        _reporterMock.Verify(r => r.PrintXHTMLReport(It.IsAny<Report>()), Times.Once);
        Assert.NotNull(reportController.report);
        Assert.NotEmpty(reportController.report.Targets);
        Assert.True(reportController.report.Targets.Count == 3);
        Assert.True(reportController.report.Targets.First().Servers.First().Endpoints.First().Issues.First().Severity == 0.3);
        Assert.True(reportController.report.Targets.Last().Servers.First().Endpoints.First().Issues.First().Severity == 0.1);
    }

    // test that report is generated correctly when not all targets have servers
    [Fact]
    public void TestReportingWorksWhenNotAllTargetsHaveServers()
    {
        // Arrange
        ReportController reportController = new(_loggerMock.Object, _reporterMock.Object);
        _reporterMock.Setup(r => r.PrintXHTMLReport(It.IsAny<Report>())).Verifiable();

        var target1 = new Target(_applicationDescription);
        target1.AddServer(new Server("opc.tcp://discoveryuri", new EndpointDescriptionCollection() { _endpointDescription }));
        target1.Servers.First().SeparatedEndpoints.First().Issues.Add(new Issue(1, "description", 0.1));

        var target2 = new Target(_applicationDescription);

        var target3 = new Target(_applicationDescription);
        target3.AddServer(new Server("opc.tcp://discoveryuri", new EndpointDescriptionCollection() { _endpointDescription }));
        target3.Servers.First().SeparatedEndpoints.First().Issues.Add(new Issue(1, "description", 0.3));

        // Act
        reportController.GenerateReport(new List<Target>() { target1, target2, target3 }, DateTime.Now, DateTime.Now, string.Empty);
        reportController.WriteReport(string.Empty);

        // Assert
        _reporterMock.Verify(r => r.PrintXHTMLReport(It.IsAny<Report>()), Times.Once);
        Assert.NotNull(reportController.report);
        Assert.NotEmpty(reportController.report.Targets);
        Assert.True(reportController.report.Targets.Count == 3);
        Assert.True(reportController.report.Targets.First().Servers.First().Endpoints.First().Issues.First().Severity == 0.3);
    }

    // test that report is generated correctly when not all servers have endpoints
    [Fact]
    public void TestReportingWorksWhenNotAllServersHaveEndpoints()
    {
        // Arrange
        ReportController reportController = new(_loggerMock.Object, _reporterMock.Object);
        _reporterMock.Setup(r => r.PrintXHTMLReport(It.IsAny<Report>())).Verifiable();

        var target1 = new Target(_applicationDescription);
        target1.AddServer(new Server("opc.tcp://discoveryuri", new EndpointDescriptionCollection() { _endpointDescription }));
        target1.Servers.First().SeparatedEndpoints.First().Issues.Add(new Issue(1, "description", 0.1));

        var target2 = new Target(_applicationDescription);
        target2.AddServer(new Server("opc.tcp://discoveryuri", new EndpointDescriptionCollection() { }));

        var target3 = new Target(_applicationDescription);
        target3.AddServer(new Server("opc.tcp://discoveryuri", new EndpointDescriptionCollection() { _endpointDescription }));
        target3.Servers.First().SeparatedEndpoints.First().Issues.Add(new Issue(1, "description", 0.3));

        // Act
        reportController.GenerateReport(new List<Target>() { target1, target2, target3 }, DateTime.Now, DateTime.Now, string.Empty);
        reportController.WriteReport(string.Empty);

        // Assert
        _reporterMock.Verify(r => r.PrintXHTMLReport(It.IsAny<Report>()), Times.Once);
        Assert.NotNull(reportController.report);
        Assert.NotEmpty(reportController.report.Targets);
        Assert.True(reportController.report.Targets.Count == 3);
        Assert.True(reportController.report.Targets.First().Servers.First().Endpoints.First().Issues.First().Severity == 0.3);
    }

    // test that report is generated correctly when not all endpoints have issues
    [Fact]
    public void TestReportingWorksWhenNotAllEndpointHaveIssues()
    {
        // Arrange
        ReportController reportController = new(_loggerMock.Object, _reporterMock.Object);
        _reporterMock.Setup(r => r.PrintXHTMLReport(It.IsAny<Report>())).Verifiable();

        var target1 = new Target(_applicationDescription);
        target1.AddServer(new Server("opc.tcp://discoveryuri", new EndpointDescriptionCollection() { _endpointDescription }));
        target1.Servers.First().SeparatedEndpoints.First().Issues.Add(new Issue(1, "description", 0.1));
        target1.Servers.First().SeparatedEndpoints.First().Issues.Add(new Issue(2, "description", 0.1));
        target1.Servers.First().SeparatedEndpoints.First().Issues.Add(new Issue(3, "description", 0.1));

        var target2 = new Target(_applicationDescription);
        target2.AddServer(new Server("opc.tcp://discoveryuri", new EndpointDescriptionCollection() { _endpointDescription }));
        target2.AddServer(new Server("opc.tcp://discoveryuri2", new EndpointDescriptionCollection() { _endpointDescription }));
        target2.AddServer(new Server("opc.tcp://discoveryuri3", new EndpointDescriptionCollection() { _endpointDescription }));

        var target3 = new Target(_applicationDescription);
        target3.AddServer(new Server("opc.tcp://discoveryuri", new EndpointDescriptionCollection() { _endpointDescription }));
        target3.Servers.First().SeparatedEndpoints.First().Issues.Add(new Issue(1, "description", 0.3));

        // Act
        reportController.GenerateReport(new List<Target>() { target1, target2, target3 }, DateTime.Now, DateTime.Now, string.Empty);
        reportController.WriteReport(string.Empty);

        // Assert
        _reporterMock.Verify(r => r.PrintXHTMLReport(It.IsAny<Report>()), Times.Once);
        Assert.NotNull(reportController.report);
        Assert.NotEmpty(reportController.report.Targets);
        Assert.True(reportController.report.Targets.Count == 3);
        Assert.True(reportController.report.Targets.First().Servers.First().Endpoints.First().Issues.First().Severity == 0.3);
    }

}