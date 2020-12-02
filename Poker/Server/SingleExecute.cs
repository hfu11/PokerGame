using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public delegate void ExecuteDelegate();

    public class SingleExecute
    {
        private static object obj = new object();
        private static SingleExecute instance = null;
        /// <summary>
        /// in case multiple threads access instance 
        /// </summary>
        public static SingleExecute Instance
        {
            get
            {
                lock (obj)
                {
                    if (instance == null)
                    {
                        instance = new SingleExecute();
                    }
                    return instance;
                }

            }
        }

        private object objLock = new object();
        private Mutex mutex = new Mutex();
        /// <summary>
        /// lock and mutex for double safety
        /// </summary>
        /// <param name="executeDelegate"></param>
        public void Execute(ExecuteDelegate executeDelegate)
        {
            lock (objLock)
            {
                mutex.WaitOne();
                executeDelegate();
                mutex.ReleaseMutex();
            }
        }
    }
}
