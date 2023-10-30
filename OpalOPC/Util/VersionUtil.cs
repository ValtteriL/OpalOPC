using System.Reflection;

namespace Util
{
    public class VersionUtil
    {
        public static readonly Version? AppAssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
    }
}
