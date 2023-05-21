namespace Util
{
    public class Credentials
    {
        public static List<(string username, string password)> CommonCredentials = new List<(string username, string password)> {
                // from https://github.com/COMSYS/msf-opcua/blob/master/credentials/opcua_credentials_sources.txt
                (username: "HTTPS", password: "password"), // Turck TBEN-L4/5
                (username: "User", password: "Siemens.1"), // SIMATIC S7-1500
                (username: "opcuauser", password: "password"), // Ignition
                (username: "username", password: "password"), // Traeger .NET SDK
                (username: "OpcUaClient", password: "OpcUaClient"), // Siemens Sinumerik Driver
                (username: "john", password: "password1"), // Unified Automation
                (username: "root", password: "secret"), // Unified Automation
                (username: "RD81OPC96", password: "MITSUBISHI"), // MELSEC iQ-R OPC UA
                (username: "simatic", password: "100simatic"), // SIMATIC HMI S7-1500
                (username: "User1", password: "1"), // Beckhof TC3
                (username: "userName", password: "password"), // process-informatik .NET
                (username: "username1", password: "password1"),
                (username: "username2", password: "password2"),
                (username: "username3", password: "password3"),
                (username: "admin", password: "admin"), // IBH Link
                (username: "admin", password: "Admin"), // HEIDENHAIN StateMonitor
                (username: "MyUser", password: "MyPassword"),
                (username: "Administrator", password: "Administrator"), // Bosch Building Integration System
                (username: "user1", password: "password"), // open62541
                (username: "user2", password: "password1"),
                (username: "appuser", password: "demo"), // UA-.NETStandard
                (username: "appadmin", password: "demo"),
                (username: "sysadmin", password: "demo"),
                (username: "user1", password: "password1"), // node-opcua
                (username: "user2", password: "password2"),
                (username: "admin", password: "pass"), // python-opcua
            };

    }
}