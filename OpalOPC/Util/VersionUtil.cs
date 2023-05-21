using System.Reflection;

namespace Util
{
    public class VersionUtil
    {
        public static Version? AppAssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
    }
}