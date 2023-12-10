using Opc.Ua;

namespace Model
{
    public class SessionCredential
    {
        public UserIdentity identity { get; private set; }
        public CertificateIdentifier applicationCertificate { get; private set; }
        public bool selfSignedAppCert { get; private set; }


        public SessionCredential(UserIdentity userIdentity, CertificateIdentifier appCertificateIdentifier, bool selfSigned = false)
        {
            identity = userIdentity;
            applicationCertificate = appCertificateIdentifier;
            selfSignedAppCert = selfSigned;
        }
    }
}
