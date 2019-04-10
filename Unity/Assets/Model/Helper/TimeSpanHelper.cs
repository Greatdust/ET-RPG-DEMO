using System;
using System.Collections.Generic;

namespace ETModel
{
    public static class TimeSpanHelper
    {
        public class Timer
        {
            public long interval;
            public long timing;
        }
        private static Dictionary<int, Timer> timeDatas = new Dictionary<int, Timer>();

        private static Queue<Timer> caches = new Queue<Timer>();
       


        public static Timer GetTimer(int hash)
        {
            Timer timer = null;
            if (!timeDatas.TryGetValue(hash, out timer))
            {
                if (caches.Count > 0)
                {
                    timer = caches.Dequeue();
                    timer.interval = 0;
                    timer.timing = 0;
                }
                else
                {
                    timer = new Timer();
                }
                timeDatas[hash] = timer;
            }
            return timer;
        }

        public static async void Timing(Timer timer, long timeSpan)
        {
            timer.interval = timeSpan;
            await TimerComponent.Instance.WaitAsync(timeSpan);
            timer.interval = 0;
        }

        public static void Remove(int hash)
        {
            if (timeDatas.ContainsKey(hash))
            {
                caches.Enqueue(timeDatas[hash]);
                timeDatas.Remove(hash);
            }
        }
    }
}