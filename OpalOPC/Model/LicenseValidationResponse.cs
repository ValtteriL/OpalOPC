using static Model.LicenseValidationResponse;

namespace Model
{
    public class LicenseValidationResponse(string licenseId, LicenseValidationCode code)
    {
        public enum LicenseValidationCode
        {
            VALID,
            NO_MACHINE,
            NO_MACHINES,
            FINGERPRINT_SCOPE_MISMATCH,
            TOO_MANY_MACHINES,
            NOT_FOUND,
            SUSPENDED,
            OVERDUE,
            EXPIRED,
            BANNED
        }
        public readonly string LicenseId = licenseId;
        public readonly LicenseValidationCode code = code;

        public bool IsValid => code is LicenseValidationCode.VALID;
        public bool ShouldActivateMachine => code is LicenseValidationCode.NO_MACHINE or LicenseValidationCode.NO_MACHINES or LicenseValidationCode.FINGERPRINT_SCOPE_MISMATCH;
        public bool IsMachineLimitExceeded => code is LicenseValidationCode.TOO_MANY_MACHINES;
        public bool IsInvalid => code is LicenseValidationCode.NOT_FOUND or LicenseValidationCode.SUSPENDED or LicenseValidationCode.SUSPENDED or LicenseValidationCode.EXPIRED or LicenseValidationCode.BANNED;
    }
}
