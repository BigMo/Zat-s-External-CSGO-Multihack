using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace laExternalMulti.Objects.Updaters
{
    public abstract class TimerUpdater : Timer, IUpdater
    {
        #region VARIABLES
        protected long lastTick, oldTick;

        private long lastFrameRate;
        private long frameRate;
        private long fpsTick;
        #endregion

        #region PROPERTIES
        public bool UpdateInIdle { get; set; }
        #endregion

        #region CONSTRUCTOR
        public TimerUpdater(int tickRate)
            : base()
        {
            this.Interval = tickRate;
            this.Tick += TimerUpdater_Tick;
            this.UpdateInIdle = false;
        }

        void TimerUpdater_Tick(object sender, EventArgs e)
        {
            oldTick = lastTick;
            lastTick = DateTime.Now.Ticks;
            CalculateFPS();
            if (Program.IsInGame || !Program.IsInGame && UpdateInIdle)
                this.OnUpdaterTick();
        }
        public TimerUpdater() : this((int)(1000f / 60f)) { }
        #endregion

        public void StartUpdater()
        {
            this.Enabled = true;
        }
        public void StopUpdater()
        {
            this.Enabled = false;
        }
        public abstract void OnUpdaterTick();
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
