using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.TimerTool
{
    /// <summary>
    /// task when time is up
    /// </summary>
    public delegate void TimerDelegate();
    public class TimerModel
    {
        public int id;

        /// <summary>
        /// duration
        /// </summary>
        public long time;

        private TimerDelegate timerDelegate;

        public TimerModel(int id, long time, TimerDelegate timerDelegate)
        {
            this.id = id;
            this.time = time;
            this.timerDelegate = timerDelegate;
        }

        public void Run()
        {
            timerDelegate();
        }
    }
}
