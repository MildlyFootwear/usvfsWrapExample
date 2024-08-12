using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace usvfstest
{
    public class usvfsP
    {

        public enum LogLevel : uint
        {
            Debug, Info, Warning, Error
        };

        public enum CrashDumpsType : uint
        {
            None, Mini, Data, Full
        };

        public struct usvfsParameters;

        [DllImport("usvfs_x64.dll")] public static unsafe extern usvfsParameters* usvfsCreateParameters();
        [DllImport("usvfs_x64.dll")] public static unsafe extern usvfsParameters* usvfsDupeParameters(usvfsParameters* p);
        [DllImport("usvfs_x64.dll")] public static unsafe extern void usvfsCopyParameters(usvfsParameters* source, usvfsParameters* dest);
        [DllImport("usvfs_x64.dll")] public static unsafe extern void usvfsFreeParameters(usvfsParameters* p);

        [DllImport("usvfs_x64.dll")] public static unsafe extern void usvfsSetInstanceName(usvfsParameters* p, string name);
        [DllImport("usvfs_x64.dll")] public static unsafe extern void usvfsSetDebugMode(usvfsParameters* p, bool debugMode);
        [DllImport("usvfs_x64.dll")] public static unsafe extern void usvfsSetLogLevel(usvfsParameters* p, LogLevel level);
        [DllImport("usvfs_x64.dll")] public static unsafe extern void usvfsSetCrashDumpType(usvfsParameters* p, CrashDumpsType type);
        [DllImport("usvfs_x64.dll")] public static unsafe extern void usvfsSetCrashDumpPath(usvfsParameters* p, string path);
        [DllImport("usvfs_x64.dll")] public static unsafe extern void usvfsSetProcessDelay(usvfsParameters* p, int milliseconds);

        [DllImport("usvfs_x64.dll")] public static unsafe extern string usvfsLogLevelToString(LogLevel lv);
        [DllImport("usvfs_x64.dll")] public static unsafe extern string usvfsCrashDumpTypeToString(CrashDumpsType t);

    }

}
