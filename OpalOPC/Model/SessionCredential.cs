using Opc.Ua;

namespace Model
{
    public class SessionCredential(IUserIdentity userIdentity, CertificateIdentifier appCertificateIdentifier, bool selfSigned = false)
    {
        public IUserIdentity identity { get; private set; } = userIdentity;
        public CertificateIdentifier applicationCertificate { get; private set; } = appCertificateIdentifier;
        public bool selfSignedAppCert { get; private set; } = selfSigned;
    }
}
