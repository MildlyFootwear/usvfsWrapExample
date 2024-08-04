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
            usvfsSetInstanceName(parameters, "lol");
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

            usvfsWrapCreateProcessHooked("C:\\Tools\\Notepad++\\notepad++.exe", "");

            Application.Run(new Form1());
        }
    }
}