using Opc.Ua;
using Opc.Ua.Client;

namespace Model
{
    public interface ISecurityTestSession
    {
        ISession Session { get; }
        SessionCredential Credential { get; }

        string EndpointUrl { get; }
    }

    public class SecurityTestSession : ISecurityTestSession, IDisposable
    {
        public ISession Session { get; private set; }
        public SessionCredential Credential { get; private set; }

        public string EndpointUrl => Session.Endpoint.EndpointUrl;

        public SecurityTestSession(ISession session, SessionCredential credential)
        {
            Session = session;
            Credential = credential;
        }

        public void Dispose()
        {
            Session.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
