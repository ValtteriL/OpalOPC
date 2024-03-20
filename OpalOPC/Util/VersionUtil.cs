using System.Reflection;

namespace Util
{
    public class VersionUtil
    {
        private static readonly Version s_appAssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0, 0);
        public static readonly string AppVersion = $"{s_appAssemblyVersion.Major}.{s_appAssemblyVersion.Minor}.{s_appAssemblyVersion.Build}";
    }
}
