using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Updaters
{
    public class DrawUpdater : TimerUpdater
    {
        #region VARIABLES
        private frmOverlay form;
        private long targetInterval;
        //private bool recalcInterval;
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        public DrawUpdater(frmOverlay form, int interval)
            : base(interval)
        {
            this.form = form;
            this.targetInterval = interval;
            oldTick = Environment.TickCount;
            lastTick = oldTick;
            //recalcInterval = true;
        }
        public DrawUpdater(frmOverlay form)
            : base()
        {
            this.form = form;
            this.targetInterval = this.Interval;
            oldTick = Environment.TickCount;
            lastTick = oldTick;
            //recalcInterval = true;
        }
        #endregion
        public override void OnUpdaterTick()
        {
            form.Draw();
            //if (recalcInterval && lastTick - oldTick >= targetInterval)
            //{
            //    this.Interval = 1 + (int)(1000f / (int)(targetInterval - (lastTick - oldTick)));
            //    Debug.WriteLine("Changed Interval to: {0}", this.Interval);
            //}
        }
    }
}
