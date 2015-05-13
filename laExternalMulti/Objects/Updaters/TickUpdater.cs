using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Updaters
{
    public class TickUpdater : TimerUpdater
    {
        #region VARIABLES
        private frmOverlay form;
        #endregion

        #region PROPERTIES
        #endregion

        #region CONSTRUCTOR
        public TickUpdater(frmOverlay form, int interval)
            : base(interval)
        {
            this.form = form;
        }
        public TickUpdater(frmOverlay form)
            : base()
        {
            this.form = form;
        }
        #endregion
        public override void OnUpdaterTick()
        {
            form.OnTimerTick();
        }
    }
}
