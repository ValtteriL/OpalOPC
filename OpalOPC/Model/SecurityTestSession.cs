using Opc.Ua.Client;

namespace Model
{
    public interface ISecurityTestSession
    {
        ISession Session { get; }
        SessionCredential Credential { get; }

        string EndpointUrl { get; }
        public void Dispose();
    }

    public class SecurityTestSession(ISession Session, SessionCredential Credential) : ISecurityTestSession, IDisposable
    {
        public ISession Session { get; private set; } = Session;
        public SessionCredential Credential { get; private set; } = Credential;

        public string EndpointUrl => Session.Endpoint.EndpointUrl;

        public void Dispose()
        {
            Session.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
