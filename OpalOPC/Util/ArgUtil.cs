using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public class ArgUtil
    {
        public static string DefaultReportName() => $"opalopc-report-{DateTime.Now.ToString("yyyyMMddHHmmssffff")}.html";
    }
}
