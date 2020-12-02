using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Server.TimerTool
{
    public class TimerManager
    {
        private static object objLock = new object();
        
        private static TimerManager instance = null;


        public static TimerManager Instance
        {
            get
            {
                lock (objLock)
                {
                    if (instance == null)
                    {
                        instance = new TimerManager();
                    }
                    return instance;
                }
            }
        }

        private Timer timer;
        /// <summary>
        /// id to timerModel
        /// </summary>
        private ConcurrentDictionary<int, TimerModel> idModelDic = new ConcurrentDictionary<int, TimerModel>();

        private ThreadSafeInt id = new ThreadSafeInt(-1);

        public TimerManager()
        {
            //1000ms
            timer = new Timer(1000);
            //calls every 1000 ms
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //iterate tasks
            foreach (var item in idModelDic.Values)
            {
                //if dateTime is beyond the time we set before
                if(DateTime.Now.Ticks >= item.time)
                {
                    //delegate
                    item.Run();
                }
            }
        }

        /// <summary>
        /// 添加计时任务
        /// </summary>
        /// <param name="delayTime"></param>
        /// <param name="td"></param>
        public void AddTimerEvent(float delayTime, TimerDelegate td)
        {
            if (delayTime <= 0) return;
            TimerModel model = new TimerModel(id.AddGetValue(), DateTime.Now.Ticks + (long)(delayTime * 10000000),td);
            idModelDic.TryAdd(model.id, model);
        }

        /// <summary>
        /// 清空任务字典
        /// </summary>
        public void Clear()
        {
            idModelDic.Clear();
        }
    }
}
