using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Plagiarism.Worker
{
    public abstract class WorkerInstance
    {
        protected ISleeper Sleeper;

        private CancellationTokenSource TokenSource;
        private Thread MainThread;

        public WorkerInstance(Configuration config)
        {
            Logger.Info(Logger.Separator);
            Sleeper = new FakeSleeper();

            TokenSource = new CancellationTokenSource();
            MainThread = new Thread(() => this.Run(TokenSource.Token));
            MainThread.IsBackground = false;
        }

        public virtual void Run(CancellationToken token)
        {
            try
            {
                DoRun(token);
            }
            catch (OperationCanceledException)
            {
                Logger.Info("Operation cancelled, terminating the worker...");
            }
            catch (Exception ex)
            {
                Logger.Error("Unhandled exception: {0}\n{1}", ex.GetType().Name, ex.ToString());
                throw;
            }
        }

        public void Start()
        {
            MainThread.Start();
        }

        public void Join()
        {
            TokenSource.Cancel();

            if (MainThread.ThreadState != ThreadState.Unstarted)
            {
                MainThread.Join();
            }
        }

        protected abstract void DoRun(CancellationToken token);
    }
}
