using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace usvfstest
{
    public class usvfsWrapM
    {
        [DllImport("usvfsWrap.dll")] public static extern void usvfsWrapVirtualLinkDirectoryStatic(string source, string destination, uint flags);
        [DllImport("usvfsWrap.dll")] public static extern bool usvfsWrapCreateProcessHooked(string lpApplicationName, string lpCommandLine);

    }
}
