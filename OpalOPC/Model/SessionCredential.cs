using Opc.Ua;

namespace Model
{
    public class SessionCredential(UserIdentity userIdentity, CertificateIdentifier appCertificateIdentifier, bool selfSigned = false)
    {
        public UserIdentity identity { get; private set; } = userIdentity;
        public CertificateIdentifier applicationCertificate { get; private set; } = appCertificateIdentifier;
        public bool selfSignedAppCert { get; private set; } = selfSigned;
    }
}
