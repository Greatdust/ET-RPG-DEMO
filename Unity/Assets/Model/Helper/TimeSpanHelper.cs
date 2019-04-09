using System;
using System.Collections.Generic;

namespace ETModel
{
    public static class TimeSpanHelper
    {
        public class Timer
        {
            public float remainTime;
            public float timing;
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
                    timer.remainTime = 0;
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

        public static async void Timing(Timer timer, float timeSpan)
        {
            timer.remainTime = timeSpan;
            await TimerComponent.Instance.WaitAsync(timeSpan);
            timer.remainTime = 0;
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