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

            var parameters = usvfsCreateParameters();
            usvfsSetInstanceName(parameters, "lol6");
            usvfsSetDebugMode(parameters, false);
            usvfsSetLogLevel(parameters, LogLevel.Warning);
            usvfsSetCrashDumpType(parameters, CrashDumpsType.None);
            usvfsSetCrashDumpPath(parameters, "");
            usvfsSetProcessDelay(parameters, 200);

            usvfsInitLogging(false);
            usvfsCreateVFS(parameters);

            string source = "J:\\BG3Profiles";
            string destination = "C:\\Tools";

            usvfsWrapVirtualLinkDirectoryStatic(source, destination, LINKFLAG_RECURSIVE);
            usvfsWrapVirtualLinkDirectoryStatic(destination, source, LINKFLAG_MONITORCHANGES);
            usvfsWrapVirtualLinkDirectoryStatic(source, destination, LINKFLAG_CREATETARGET);
            usvfsWrapCreateVFSDump();
            bool running = true;
            void threadMethod()
            {
                running = true;
                usvfsWrapCreateProcessHooked("C:\\Tools\\Notepad++\\notepad++.exe", "");
                running = false;
            }
            Thread exeThread = new Thread(new ThreadStart(threadMethod));
            exeThread.Start();
            while (running) {
                Console.WriteLine("Main process still running. Hooked processes: "+usvfsWrapGetHookedCount());
                Thread.Sleep(3000);
            }
            Console.WriteLine("Main process terminated.");
            int hookCnt = usvfsWrapGetHookedCount();
            while (hookCnt > 0)
            usvfsDisconnectVFS();
            //Application.Run(new Form1());
        }
    }
}