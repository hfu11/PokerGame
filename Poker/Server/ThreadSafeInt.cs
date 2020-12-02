using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ThreadSafeInt
    {
        private int value;
        public ThreadSafeInt(int value)
        {
            this.value = value;
        }

        /// <summary>
        /// Increase and return value
        /// </summary>
        /// <returns></returns>
        public int AddGetValue()
        {
            lock (this)
            {
                value++;
                return value;
            }
        }

        /// <summary>
        /// decrease
        /// </summary>
        /// <returns></returns>
        public int SubGetValue()
        {
            lock (this)
            {
                value--;
                return value;
            }
        }

        /// <summary>
        /// return
        /// </summary>
        /// <returns></returns>
        public int GetValue()
        {
            lock (this)
            {
                return value;

            }
        }
    }
}
