using Microsoft.VisualBasic.Logging;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace usvfsWrapExample
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            [DllImport("kernel32.dll")] static extern bool AllocConsole();
            AllocConsole();

            usvfsWrapSetDebug(true); // Enables console printout for usvfsWrap.
            usvfsWrapCreateVFS("test", false, LogLevel.Warning, CrashDumpsType.None, "", 200);

            string source = "C:\\Tools";
            string destination = "J:\\BG3Profiles";

            // LINKFLAG_RECURSIVE for linking the source and all its subdirectories.
            usvfsWrapVirtualLinkDirectoryStatic(source, destination, LINKFLAG_RECURSIVE);

            // LINKFLAG_MONITORCHANGES will update the VFS when files are created/etc.
            usvfsWrapVirtualLinkDirectoryStatic(destination, source, LINKFLAG_MONITORCHANGES);

            // LINKFLAG_CREATETARGET will make the source directory the target for all file creation/modification operations that happen in destination.
            usvfsWrapVirtualLinkDirectoryStatic(source, destination, LINKFLAG_CREATETARGET);



            // Convert the character array pointer provided by usvfsWrapCreateVFSDump to a proper string format for C#.
            //string s = Marshal.PtrToStringAnsi(


            bool running = true;

            // Setting up a thread to launch and hook the executable so it doesn't hang the main application.
            string exe = "C:\\Program Files\\paint.net\\paintdotnet.exe";
            void threadMethod()
            {
                running = true;
                try
                {
                    // createFlags set to 0 as it is unneeded here.
                    usvfsWrapCreateProcessHooked(exe, null, 0, null);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                running = false;
            }
            Thread exeThread = new Thread(new ThreadStart(threadMethod));
            exeThread.Start();
            Thread.Sleep(50);
            // While loop to keep the application from proceeding past this point while the hooked executable is running.
            while (running) {
                Console.Write("\r"+DateTime.Now.ToString("HH:mm:ss")+" Main process still running. Hooked processes: " + usvfsWrapGetHookedCount() + ". " + usvfsWrapGetLastHookedID());
                Thread.Sleep(1000);
            }
            Console.WriteLine("\nMain process terminated.");

            // Another while loop to keep the VFS available until all the child executables have closed.
            int hookCnt = usvfsWrapGetHookedCount();
            while (hookCnt > 0)
            {
                Console.Write("\r" + DateTime.Now.ToString("HH:mm:ss") + " Hooked processes: " + usvfsWrapGetHookedCount() + ".");
                hookCnt = usvfsWrapGetHookedCount();
                Thread.Sleep(1000);
            }

            usvfsWrapCreateVFSDump("dump.txt");

            // finally disconnceting and freeing the VFS
            Console.WriteLine("Disconnecting VFS");
            usvfsWrapFree();

            Thread.Sleep(5000);
        }
    }
}