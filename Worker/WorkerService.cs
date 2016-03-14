using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Plagiarism.Worker.Algorithms;

namespace Plagiarism.Worker
{
    public partial class WorkerService : ServiceBase
    {
        private readonly string WorkingDir;
        private WorkerInstance Instance;
        
        public WorkerService(string workingDir)
        {
            WorkingDir = workingDir;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                base.OnStart(args);

                Configuration conf = Configuration.Deserialize(WorkingDir);
                Logger.IsDebugLevelEnabled = conf.Debug;

                switch (conf.Mode)
                {
                    case RunningMode.SelfTesting:
                        Instance = new SelfTestingMode.Instance(conf);
                        break;
                    case RunningMode.Api:
                        Instance = new ApiMode.Instance(conf);
                        break;
                    default:
                        throw new Exception(String.Format("Unsupported mode: {0}", conf.Mode));
                }

                Instance.Start();
                Logger.Info("Successfuly started service");
            }
            catch (Exception ex)
            {
                Logger.Error("Could not start Worker: {0}", ex.ToString());
                ExitCode = Misc.Constants.ErrorExceptionInService;
                Stop();
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            if (Instance != null)
            {
                Instance.Join();
                Instance = null;
            }
            Logger.Info("Service stopped successfully");
        }

        public void RunAsConsoleApp(string[] args)
        {
            OnStart(args);
            Console.WriteLine("Press any key to terminate...");  
            Console.ReadKey();
            Console.WriteLine("Terminating...");
            OnStop();
        }
    }
}
