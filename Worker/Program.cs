using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Plagiarism.Worker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static int Main(string[] args)
        {
            SetLowPriority();
            bool runAsService = !Environment.UserInteractive;
            string workingDir = runAsService ? AppDomain.CurrentDomain.BaseDirectory : Directory.GetCurrentDirectory();
            var service = new WorkerService(workingDir);

            Logger.Init(workingDir);

            if (runAsService)
            {
                ServiceBase.Run(service);
                return 0;
            }
            else
            {
                Console.WriteLine("Running Plagiarism service as console app...");
                service.RunAsConsoleApp(args);
                return service.ExitCode;
            }
        }

        private static void SetLowPriority()
        {
            using (Process p = Process.GetCurrentProcess())
            {
                p.PriorityClass = ProcessPriorityClass.BelowNormal;
            }
        }
    }
}
