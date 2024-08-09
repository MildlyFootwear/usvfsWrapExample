using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace usvfstest
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        unsafe static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            [DllImport("kernel32.dll")] static extern bool AllocConsole();
            AllocConsole();

            // Set up parameters for VFS.

            var parameters = usvfsCreateParameters();
            usvfsSetInstanceName(parameters, "test");
            usvfsSetDebugMode(parameters, false);
            usvfsSetLogLevel(parameters, LogLevel.Warning);
            usvfsSetCrashDumpType(parameters, CrashDumpsType.None);
            usvfsSetCrashDumpPath(parameters, "");
            usvfsSetProcessDelay(parameters, 200); // delays launch of executable by 200ms to make sure the VFS has time to set up.

            usvfsInitLogging(false);
            usvfsCreateVFS(parameters);

            usvfsWrapSetDebug(true); // Enables console printout for usvfsWrap.

            string source = "J:\\BG3Profiles";
            string destination = "C:\\Tools";

            // LINKFLAG_RECURSIVE for linking the source and all its subdirectories.
            usvfsWrapVirtualLinkDirectoryStatic(source, destination, LINKFLAG_RECURSIVE);

            // LINKFLAG_CREATETARGET will make the source directory the target for all file creation/modification operations that happen in destination.
            usvfsWrapVirtualLinkDirectoryStatic(source, destination, LINKFLAG_CREATETARGET);

            // LINKFLAG_MONITORCHANGES will update the VFS when files are created/etc.
            usvfsWrapVirtualLinkDirectoryStatic(destination, source, LINKFLAG_MONITORCHANGES);

            // Convert the character array pointer provided by usvfsWrapCreateVFSDump to a proper string format for C#.
            string s = Marshal.PtrToStringAnsi(usvfsWrapCreateVFSDump());

            Console.WriteLine(s);

            // Setting up a thread to launch and hook the executable so it doesn't hang the main application.

            bool running = true;

            void threadMethod()
            {
                running = true;
                usvfsWrapCreateProcessHooked("C:\\Tools\\Notepad++\\notepad++.exe", "");
                running = false;
            }
            Thread exeThread = new Thread(new ThreadStart(threadMethod));
            exeThread.Start();

            // while loop to keep the application from proceeding past this point while the hooked executable is running.

            while (running) {
                Console.WriteLine("Main process still running. Hooked processes: "+usvfsWrapGetHookedCount()+". "+usvfsWrapGetLastHookedID());
                Thread.Sleep(5000);
            }
            Console.WriteLine("Main process terminated.");

            // another while loop to keep the VFS available until all the child executables have closed.

            int hookCnt = usvfsWrapGetHookedCount();
            while (hookCnt > 0)
            {
                hookCnt = usvfsWrapGetHookedCount();
                Console.WriteLine("Main process has ended. Hooked processes: ");
                Thread.Sleep(5000);
            }

            // finally disconnceting and freeing the VFS

            Console.WriteLine("Disconnecting VFS");
            usvfsDisconnectVFS();
            usvfsFreeParameters(parameters);

            //Application.Run(new Form1());
        }
    }
}