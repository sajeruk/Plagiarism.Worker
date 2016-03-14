using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Plagiarism.Worker
{
    public interface ISleeper
    {
        void Sleep();
        void Reset();
    }

    class FakeSleeper : ISleeper
    {
        public void Sleep()
        {
        }
        public void Reset()
        {
        }
    }

    class ConstantSleeper : ISleeper
    {
        private TimeSpan Timeout;

        public ConstantSleeper(int seconds)
        {
            Timeout = new TimeSpan(0, 0, seconds);
        }

        public void Sleep()
        {
            Logger.Debug("Sleeping {0}...", Timeout.TotalSeconds);
            Thread.Sleep(Timeout);
        }

        public void Reset()
        {
        }
    }
}
