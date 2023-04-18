using Opc.Ua;

namespace Util
{
    public static class ConnectionUtil
    {

        // Authenticate with OPC UA server and start a session
        public async static Task<Opc.Ua.Client.Session> StartSession(EndpointDescription endpointDescription, UserIdentity userIdentity)
        {
            ApplicationConfiguration m_configuration = new ApplicationConfiguration();
            m_configuration.ApplicationName = "";
            m_configuration.ApplicationUri = "";
            m_configuration.ApplicationType = ApplicationType.Server;
            m_configuration.ClientConfiguration = new ClientConfiguration();

            EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(m_configuration);
            ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

            var session = await Opc.Ua.Client.Session.Create(
                m_configuration,
                endpoint,
                false,
                false,
                m_configuration.ApplicationName,
                30 * 1000,
                userIdentity,
                null
            ).ConfigureAwait(false);

            if (!session.Connected)
            {
                return null;
            }
            return session;
        }

    }
}