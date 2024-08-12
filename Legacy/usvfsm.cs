using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace usvfstest
{
    public class usvfsM
    {
        static public uint LINKFLAG_FAILIFEXISTS = 0x00000001; // if set, linking fails in case of an error
        static public uint LINKFLAG_MONITORCHANGES = 0x00000002; // if set, changes to the source directory after the link operation
                                                                        // will be updated in the virtual fs. only relevant in static
                                                                        // link directory operations
        static public uint LINKFLAG_CREATETARGET = 0x00000004; // if set, file creation (including move or copy) operations to
                                                                      // destination will be redirected to the source. Only one createtarget
                                                                      // can be set for a destination folder so this flag will replace
                                                                      // the previous create target.
                                                                      // If there different create-target have been set for an element and one of its
                                                                      // ancestors, the inner-most create-target is used
        static public uint LINKFLAG_RECURSIVE = 0x00000008; // if set, directories are linked recursively
        static public uint LINKFLAG_FAILIFSKIPPED = 0x00000010; // if set, linking fails if the file or directory is skipped
                                                                // files or directories are skipped depending on whats been added to 
                                                                // the skip file suffixes or skip directories list in
                                                                // the sharedparameters class, those lists are checked during virtual linking


        /**
         * removes all virtual mappings
         */
        [DllImport("usvfs_x64.dll")] public static unsafe extern void  usvfsClearVirtualMappings();

        /**
         * link a file virtually
         * @note: the directory the destination file resides in has to exist - at least virtually.
         */
        [DllImport("usvfs_x64.dll")] public static unsafe extern bool  usvfsVirtualLinkFile([MarshalAs(UnmanagedType.LPWStr)] string source, [MarshalAs(UnmanagedType.LPWStr)] string destination, uint flags);

        /**
         * link a directory virtually. This static variant recursively links all files individually, change notifications
         * are used to update the information.
         * @param failIfExists if true, this call fails if the destination directory exists (virtually or physically)
         */
        [DllImport("usvfs_x64.dll")] public static unsafe extern bool  usvfsVirtualLinkDirectoryStatic([MarshalAs(UnmanagedType.LPWStr)] string source, [MarshalAs(UnmanagedType.LPWStr)] string destination, uint flags);

        /**
         * connect to a virtual filesystem as a controller, without hooking the calling process. Please note that
         * you can only be connected to one vfs, so this will silently disconnect from a previous vfs.
         */
        [DllImport("usvfs_x64.dll")] public static unsafe extern bool  usvfsConnectVFS(usvfsParameters* p);

        /**
         * @brief create a new VFS. This is similar to ConnectVFS except it guarantees
         *   the vfs is reset before use.
         */
        [DllImport("usvfs_x64.dll")] public static unsafe extern bool  usvfsCreateVFS(usvfsParameters* p);

        /**
         * disconnect from a virtual filesystem. This removes hooks if necessary
         */
        [DllImport("usvfs_x64.dll")] public static unsafe extern void  usvfsDisconnectVFS();

        /**
         * adds an executable to the blacklist so it doesn't get exposed to the virtual
         * file system
         * @param executableName  name of the executable
         */
        [DllImport("usvfs_x64.dll")] public static unsafe extern void  usvfsBlacklistExecutable([MarshalAs(UnmanagedType.LPWStr)] string executableName);

        /**
         * clears the executable blacklist
         */
        [DllImport("usvfs_x64.dll")] public static unsafe extern void  usvfsClearExecutableBlacklist();

        /**
         * adds a file suffix to a list to skip during file linking
         * .txt and some_file.txt are both valid file suffixes,
         * not to be confused with file extensions
         * @param fileSuffix  a valid file suffix
         */
        [DllImport("usvfs_x64.dll")] public static extern void  usvfsAddSkipFileSuffix([MarshalAs(UnmanagedType.LPWStr)] string fileSuffix);

        /**
         * clears the file suffix skip-list
         */
        [DllImport("usvfs_x64.dll")] public static extern void  usvfsClearSkipFileSuffixes();

        /**
         * adds a directory name that will be skipped during directory linking.
         * Not a path. Any directory matching the name will be skipped,
         * regardless of it's path, for example if .git is added,
         * any sub-path or root-path containing a .git directory
         * will have the .git directory skipped during directory linking
         * @param directory  name of the directory
         */
        [DllImport("usvfs_x64.dll")] public static extern void  usvfsAddSkipDirectory([MarshalAs(UnmanagedType.LPWStr)] string directory);

        /**
         * clears the directory skip-list
         */
        [DllImport("usvfs_x64.dll")] public static extern void  usvfsClearSkipDirectories();

        /**
         * adds a library to be force loaded when the given process is injected
         * @param
         */
        [DllImport("usvfs_x64.dll")] public static unsafe extern void  usvfsForceLoadLibrary([MarshalAs(UnmanagedType.LPWStr)] string processName, [MarshalAs(UnmanagedType.LPWStr)] string libraryPath);

        /**
         * clears all previous calls to ForceLoadLibrary
         */
        [DllImport("usvfs_x64.dll")] public static extern void  usvfsClearLibraryForceLoads();

        /**
         * print debugging info about the vfs. The format is currently not fixed and may
         * change between usvfs versions
         */
        [DllImport("usvfs_x64.dll")] public static extern void  usvfsPrintDebugInfo();

        //#if defined(UNITTEST) || defined(_WINDLL)
        [DllImport("usvfs_x64.dll")] public static extern void  usvfsInitLogging(bool toLocal = false);
        //#endif


        // the instance and shm names are not updated
        //
        [DllImport("usvfs_x64.dll")] public static unsafe extern void  usvfsUpdateParameters(usvfsParameters* p);

        [DllImport("usvfs_x64.dll")] public static unsafe extern int  usvfsCreateMiniDump(void* exceptionPtrs, CrashDumpsType type, [MarshalAs(UnmanagedType.LPWStr)] string dumpPath);

        [DllImport("usvfs_x64.dll")] public static unsafe extern char*  usvfsVersionString();
    }

}