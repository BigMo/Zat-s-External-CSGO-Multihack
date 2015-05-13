using laExternalMulti.Objects.Updaters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.Updaters
{
    class CPUUpdater : ThreadedUpdater
    {
        #region VARIABLES
        private PerformanceCounter counter;
        private int lastVal = 0;
        #endregion

        #region PROPERTIES
        public int CurrentValue { get { return lastVal; } }
        #endregion

        public CPUUpdater() : base(500)
        {
            try
            {
                counter = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
            }
            catch(Exception ex) 
            {
                Program.PrintError(" > [ERROR:CPUUpdater]: {0}", ex.Message);
            }
        }

        #region METHODS
        public override void OnUpdaterTick()
        {
            if (counter != null)
                lastVal = (int)(counter.NextValue() / (double)Environment.ProcessorCount);
            else
                lastVal = 0;
        }
        #endregion
    }
}
