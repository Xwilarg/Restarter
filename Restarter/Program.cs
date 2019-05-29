using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Restarter
{
    class Program
    {
        private static void RestarterThread(string path)
        {
            while (Thread.CurrentThread.IsAlive)
            {
                try
                {
                    FileInfo fi = new FileInfo(path);
                    ProcessStartInfo startInfo = new ProcessStartInfo(path) {
                        WorkingDirectory = fi.Directory.ToString()
                    };
                    Process p = Process.Start(startInfo);
                    p.WaitForExit();
                }
                catch (Win32Exception we)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("We weren't able to access to " + path + Environment.NewLine + "Reason: " + we.Message + Environment.NewLine);
                    break;
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(DateTime.Now.ToString() + ": Restarting " + path);
            }
        }

        private static void Main(string[] args)
        {
            if (!File.Exists("restarter.conf"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You must create a restarter.conf file and fill it with the path of the programs you want to keep open.");
                Console.ReadKey();
                return;
            }
            string[] paths = File.ReadAllLines("restarter.conf");
            foreach (string path in paths)
            {
                new Thread(new ThreadStart(() => RestarterThread(path))).Start();
            }
            Console.ReadKey();
        }
    }
}
