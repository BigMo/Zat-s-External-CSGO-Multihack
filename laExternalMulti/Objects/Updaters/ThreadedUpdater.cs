using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace laExternalMulti.Objects.Updaters
{
    public abstract class ThreadedUpdater : IUpdater
    {
        #region VARIABLES
        private Thread thread;
        private bool work;
        private int interval;
        private long lastTick;

        private long lastFrameRate;
        private long frameRate;
        private long fpsTick;
        #endregion

        #region PROPERTIES
        public int Interval { get { return interval; } set { interval = value; } }
        public bool UpdateInIdle { get; set; }
        #endregion

        #region CONSTRUCTOR
        public ThreadedUpdater(int tickRate)
            : base()
        {
            this.Interval = tickRate;
            this.UpdateInIdle = false;
        }
        public ThreadedUpdater() : this((int)(1000f / 60f)) { }
        #endregion

        #region METHODS

        public void StartUpdater()
        {
            if (thread != null)
                StopUpdater();
            work = true; 
            this.thread = new Thread(new ThreadStart(Loop));
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        public void StopUpdater()
        {
            work = false;
            if (thread == null)
                return;
            if (thread.ThreadState == ThreadState.Running)
                thread.Abort();
            thread = null;
        }

        private void Loop()
        {
            lastTick = DateTime.Now.Ticks;
            while (work)
            {
                lastTick = DateTime.Now.Ticks;
                CalculateFPS();
                if(Program.IsInGame || !Program.IsInGame && UpdateInIdle)
                    this.OnUpdaterTick();
                Thread.Sleep(interval);
            }
        }

        private long GetSleepTime()
        {
            long ticks = DateTime.Now.Ticks - GetLastTick();
            long millis = ticks / TimeSpan.TicksPerMillisecond;
            return interval - millis;
        }

        public abstract void OnUpdaterTick();
        #endregion

        public long GetLastTick()
        {
            return lastTick;
        }
        public long GetFrameRate()
        {
            return lastFrameRate;
        }
        public void CalculateFPS()
        {
            if (System.Environment.TickCount - fpsTick >= 1000)
            {
                lastFrameRate = frameRate;
                frameRate = 0;
                fpsTick = System.Environment.TickCount;
            }
            frameRate++;
        }
    }
}
