using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Updaters
{
    public interface IUpdater
    {
        #region METHODS
        void StartUpdater();
        void StopUpdater();
        void OnUpdaterTick();
        long GetLastTick();
        void CalculateFPS();
        long GetFrameRate();
        #endregion
    }
}
