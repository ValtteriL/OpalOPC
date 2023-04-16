using System.Globalization;
using Microsoft.Extensions.Logging;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace Quickstarts.ConsoleReferenceClient
{
    /// <summary>
    /// The program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main entry point.
        /// </summary>
        public static async Task Main(string[] args)
        {
            TextWriter output = Console.Out;
            output.WriteLine("OPC UA Console Reference Client");

            output.WriteLine("OPC UA library: {0} @ {1} -- {2}",
                Utils.GetAssemblyBuildNumber(),
                Utils.GetAssemblyTimestamp().ToString("G", CultureInfo.InvariantCulture),
                Utils.GetAssemblySoftwareVersion());

            // The application name and config file names
            var applicationName = "ConsoleReferenceClient";
            var configSectionName = "Quickstarts.ReferenceClient";
            var usage = $"Usage: dotnet {applicationName}.dll [OPTIONS]";

            // command line options
            bool showHelp = false;
            bool autoAccept = false;
            string? username = null;
            string? userpassword = null;
            bool logConsole = false;
            bool appLog = false;
            bool renewCertificate = false;
            bool loadTypes = false;
            bool browseall = false;
            bool fetchall = false;
            bool jsonvalues = false;
            bool verbose = false;
            string? password = null;
            int timeout = Timeout.Infinite;
            string? logFile = null;

            Mono.Options.OptionSet options = new Mono.Options.OptionSet {
                usage,
                { "h|help", "show this message and exit", h => showHelp = h != null },
                { "a|autoaccept", "auto accept certificates (for testing only)", a => autoAccept = a != null },
                { "un|username=", "the name of the user identity for the connection", (string u) => username = u },
                { "up|userpassword=", "the password of the user identity for the connection", (string u) => userpassword = u },
                { "c|console", "log to console", c => logConsole = c != null },
                { "l|log", "log app output", c => appLog = c != null },
                { "p|password=", "optional password for private key", (string p) => password = p },
                { "r|renew", "renew application certificate", r => renewCertificate = r != null },
                { "t|timeout=", "timeout in seconds to exit application", (int t) => timeout = t * 1000 },
                { "logfile=", "custom file name for log output", l => { if (l != null) { logFile = l; } } },
                { "lt|loadtypes", "Load custom types", lt => { if (lt != null) loadTypes = true; } },
                { "b|browseall", "Browse all references", b => { if (b != null) browseall = true; } },
                { "f|fetchall", "Fetch all nodes", f => { if (f != null) fetchall = true; } },
                { "j|json", "Output all Values as JSON", j => { if (j != null) jsonvalues = true; } },
                { "v|verbose", "Verbose output", v => { if (v != null) verbose = true; } },
            };

            try
            {
                // parse command line and set options
                var extraArg = ConsoleUtils.ProcessCommandLine(output, args, options, ref showHelp, false);

                // connect Url?
                Uri serverUrl = new Uri("opc.tcp://localhost:62541/Quickstarts/ReferenceServer");
                if (!string.IsNullOrEmpty(extraArg))
                {
                    serverUrl = new Uri(extraArg);
                }

                // log console output to logger
                if (logConsole && appLog)
                {
                    output = new LogWriter();
                }

                // Define the UA Client application
                ApplicationInstance.MessageDlg = new ApplicationMessageDlg(output);
                CertificatePasswordProvider PasswordProvider = new CertificatePasswordProvider(password);
                ApplicationInstance application = new ApplicationInstance {
                    ApplicationName = applicationName,
                    ApplicationType = ApplicationType.Client,
                    ConfigSectionName = configSectionName,
                    CertificatePasswordProvider = PasswordProvider
                };

                // load the application configuration.
                var config = await application.LoadApplicationConfiguration(silent: false);

                // override logfile
                if (logFile != null)
                {
                    var logFilePath = config.TraceConfiguration.OutputFilePath;
                    var filename = Path.GetFileNameWithoutExtension(logFilePath);
                    config.TraceConfiguration.OutputFilePath = logFilePath.Replace(filename, logFile);
                    config.TraceConfiguration.DeleteOnLoad = true;
                    config.TraceConfiguration.ApplySettings();
                }

                // setup the logging
                ConsoleUtils.ConfigureLogging(config, applicationName, logConsole, LogLevel.Information);

                // delete old certificate
                if (renewCertificate)
                {
                    await application.DeleteApplicationInstanceCertificate().ConfigureAwait(false);
                }

                // check the application certificate.
                bool haveAppCertificate = await application.CheckApplicationInstanceCertificate(false, minimumKeySize: 0).ConfigureAwait(false);
                if (!haveAppCertificate)
                {
                    throw new ErrorExitException("Application instance certificate invalid!", ExitCode.ErrorCertificate);
                }

                // wait for timeout or Ctrl-C
                var quitEvent = ConsoleUtils.CtrlCHandler();

                // connect to a server until application stops
                bool quit = false;
                DateTime start = DateTime.UtcNow;
                int waitTime = int.MaxValue;
                do
                {
                    if (timeout > 0)
                    {
                        waitTime = timeout - (int)DateTime.UtcNow.Subtract(start).TotalMilliseconds;
                        if (waitTime <= 0)
                        {
                            break;
                        }
                    }

                    // https://reference.opcfoundation.org/Core/Part4/v105/docs/

                    // DISCOVER (find services, check supported modes, security policies, and user tokens)
                    // https://reference.opcfoundation.org/GDS/v105/docs/4.3
                    // 1. Use FindServers on LDS to get list of application descriptions
                    // 2. Use GetEndpoint on each application's discoveryurls to get list of endpointdescriptions
                    // Endpointdescriptions contain supported security policy, security mode, and all supported user tokens 
                    // If the ApplicationType is Discovery server, it can be used to find other servers with FindServersOnNetwork and iterate through them

                    // Application
                    // - Endpoint..N
                    // - Access privileges..M

                    // https://reference.opcfoundation.org/Core/Part4/v104/docs/5.4.2
                    // ask the server for all servers it knows about
                    DiscoveryClient asd = DiscoveryClient.Create(new Uri("opc.tcp://echo.koti.kontu:53530"));

                    Console.WriteLine("### Discovering servers");
                    ApplicationDescriptionCollection adc = asd.FindServers(null);

                    Console.WriteLine($"Number of servers: {adc.Count}");

                    foreach (ApplicationDescription ad in adc) {
                        Console.WriteLine($"{ad.ApplicationType}: {ad.ApplicationName} + {ad.ApplicationUri} + {ad.ProductUri}");
                        foreach (string s in ad.DiscoveryUrls) {

                            // https://reference.opcfoundation.org/Core/Part4/v104/docs/5.4.4
                            // ask each discoveryUrl for endpoints
                            Console.WriteLine("### Discovering endpoints");

                            Console.WriteLine($"DiscoveryUrl: {s}");

                            DiscoveryClient sss = DiscoveryClient.Create(new Uri(s.Replace("echo", "echo.koti.kontu").Replace("opc.http", "http"))); // TODO: make something smarter up
                            EndpointDescriptionCollection edcc = sss.GetEndpoints(null);

                            Console.WriteLine($"Number of endpoints: {edcc.Count}");
                            foreach (EndpointDescription e in edcc) {
                                Console.WriteLine($"ENDPOINT: {e.EndpointUrl}");
                                Console.WriteLine($"\tSecurity policy: {e.SecurityPolicyUri}");
                                Console.WriteLine($"\tSecurity mode: {e.SecurityMode}");

                                Console.WriteLine($"\tUser token policies:");
                                foreach (UserTokenPolicy utp in e.UserIdentityTokens) {
                                    Console.WriteLine($"\t\t{utp} ({utp.SecurityPolicyUri}, {utp.PolicyId})");
                                }
                            }

                            if (ad.ApplicationType == ApplicationType.DiscoveryServer) {

                                // https://reference.opcfoundation.org/Core/Part4/v104/docs/5.4.3
                                // ask the network servers this server knows about
                                // only works with discoveryservers

                                ServerOnNetworkCollection soncc = sss.FindServersOnNetwork(0, 0, null, out DateTime dtt);
                                foreach (ServerOnNetwork son in soncc) {
                                    Console.WriteLine($"SERVER ON NETWORK: {son.DiscoveryUrl}");
                                    // GetEndpoints could be used to check the endpoint securities of the found servers
                                }
                            }
                        }
                    }

                    return;

                    // TODO: CHECK COMMON CREDENTIALS - if username-pass
                    // TODO: CHECK self signed cert - if certificate
                    // TODO: CHECK FOR READ/WRITE ACCESS


                    // create the UA Client object and connect to configured server.
                    using (UAClient uaClient = new UAClient(
                        application.ApplicationConfiguration, output, ClientBase.ValidateResponse) {
                        AutoAccept = autoAccept
                    })
                    {

                        // set user identity
                        if (!String.IsNullOrEmpty(username))
                        {
                            // identity can be configured to be anonymous, username, certificate, token
                            uaClient.UserIdentity = new UserIdentity(username, userpassword ?? string.Empty);
                        }

                        bool connected = await uaClient.ConnectAsync(serverUrl.ToString(), false);
                        if (connected)
                        {
                            output.WriteLine("Connected! Ctrl-C to quit.");

                            // enable subscription transfer
                            uaClient.Session.TransferSubscriptionsOnReconnect = true;

                            var samples = new ClientSamples(output, ClientBase.ValidateResponse, quitEvent, verbose);
                            if (loadTypes)
                            {
                                await samples.LoadTypeSystem(uaClient.Session).ConfigureAwait(false);
                            }

                            if (browseall || fetchall || jsonvalues)
                            {
                                NodeIdCollection? variableIds = null;
                                ReferenceDescriptionCollection? referenceDescriptions = null;
                                if (browseall)
                                {
                                    referenceDescriptions =
                                        samples.BrowseFullAddressSpace(uaClient, Objects.RootFolder);
                                    variableIds = new NodeIdCollection(referenceDescriptions
                                        .Where(r => r.NodeClass == NodeClass.Variable && r.TypeDefinition.NamespaceIndex != 0)
                                        .Select(r => ExpandedNodeId.ToNodeId(r.NodeId, uaClient.Session.NamespaceUris)));
                                }

                                IList<INode>? allNodes = null;
                                if (fetchall)
                                {
                                    allNodes = samples.FetchAllNodesNodeCache(
                                        uaClient, Objects.RootFolder, true, true, false);
                                    variableIds = new NodeIdCollection(allNodes
                                        .Where(r => r.NodeClass == NodeClass.Variable && ((VariableNode)r).DataType.NamespaceIndex != 0)
                                        .Select(r => ExpandedNodeId.ToNodeId(r.NodeId, uaClient.Session.NamespaceUris)));
                                }

                                if (jsonvalues && variableIds != null)
                                {
                                    await samples.ReadAllValuesAsync(uaClient, variableIds);
                                }

                                quit = true;
                            }
                            else
                            {
                                // Run tests for available methods on reference server.
                                samples.ReadNodes(uaClient.Session);
                                samples.WriteNodes(uaClient.Session);
                                samples.Browse(uaClient.Session);
                                samples.CallMethod(uaClient.Session);
                                samples.SubscribeToDataChanges(uaClient.Session, 120_000);

                                output.WriteLine("Waiting...");

                                // Wait for some DataChange notifications from MonitoredItems
                                quit = quitEvent.WaitOne(timeout > 0 ? waitTime : 30_000);
                            }

                            output.WriteLine("Client disconnected.");

                            uaClient.Disconnect();
                        }
                        else
                        {
                            output.WriteLine("Could not connect to server! Retry in 10 seconds or Ctrl-C to quit.");
                            quit = quitEvent.WaitOne(Math.Min(10_000, waitTime));
                        }
                    }

                } while (!quit);

                output.WriteLine("Client stopped.");
            }
            catch (Exception ex)
            {
                output.WriteLine(ex.Message);
            }
        }
    }
}
