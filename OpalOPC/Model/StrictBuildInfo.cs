using Opc.Ua;

namespace Model
{
    public class StrictBuildInfo(BuildInfo buildInfo)
    {
        public string ProductName { get; set; } = buildInfo.ProductName;
        public string Manufacturer { get; set; } = buildInfo.ManufacturerName;
        public string SoftwareVersion { get; set; } = buildInfo.SoftwareVersion;
    }
}
