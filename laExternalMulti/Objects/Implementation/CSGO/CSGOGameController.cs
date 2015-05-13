using laExternalMulti.Objects.Implementation.CSGO.Updaters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO
{
    class CSGOGameController : GameController
    {
        #region PROPERTIES
        public MemoryUpdater MemoryUpdater { get; private set; }
        public AimBot AimBot { get; private set; }
        public TriggerBot TriggerBot { get; private set; }
        public RecoilControl RecoilControl { get; private set; }
        public CPUUpdater PerformanceUpdater { get; private set; }
        public SoundESP SoundESP { get; private set; }
        #endregion
        public CSGOGameController(frmOverlay form)
            : base((int)(1000f / 60f), "csgo", form)
        {
            MemoryUpdater = new MemoryUpdater();
            MemoryUpdater.StartUpdater();
            AimBot = new AimBot();
            AimBot.StartUpdater();
            TriggerBot = new TriggerBot();
            TriggerBot.StartUpdater();
            RecoilControl = new RecoilControl();
            RecoilControl.StartUpdater();
            PerformanceUpdater = new CPUUpdater();
            PerformanceUpdater.StartUpdater();
            SoundESP = new SoundESP();
            SoundESP.StartUpdater();
        }
    }
}
