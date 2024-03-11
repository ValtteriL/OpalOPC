namespace Model
{
    public class LicenseValidationResponse(string licenseId, string code)
    {
        public readonly string LicenseId = licenseId;
        public readonly string Code = code;

        public bool IsValid => Code is "VALID";
        public bool ShouldActivateMachine => Code is "NO_MACHINE" or "NO_MACHINES" or "FINGERPRINT_SCOPE_MISMATCH";
        public bool IsMachineLimitExceeded => Code is "TOO_MANY_MACHINES";
        public bool IsInvalid => Code is "NOT_FOUND" or "SUSPENDED" or "OVERDUE" or "EXPIRED" or "BANNED";
    }
}
