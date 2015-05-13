using laExternalMulti.Objects.Implementation.CSGO.Data;
using laExternalMulti.Objects.Updaters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace laExternalMulti.Objects.Implementation.CSGO.Updaters
{
    class SoundESP : ThreadedUpdater
    {
        #region VARIABLES
        long lastBeep;
        #endregion

        #region PROPERTIES
        public float LastPercent { get; private set; }
        #endregion

        #region CONSTRUCTOR
        public SoundESP()
            : base(60)
        {
            lastBeep = DateTime.Now.Ticks;
        }
        #endregion

        #region METHODS
        public override void OnUpdaterTick()
        {
            CSGOImplementation csgo = (CSGOImplementation)Program.GameImplementation;
            Player localPlayer;

            if (csgo == null)
                return;
            localPlayer = csgo.LocalPlayer;
            if (csgo.LocalPlayer == null)
                return;
            if (csgo.Players == null)
                return;
            if (csgo.GetValue<YesNo>("soundEspEnabled") != YesNo.Yes)
                return;

            float maxSpan = csgo.GetValue<float>("soundEspInterval");
            float maxRange = csgo.GetValue<float>("soundEspRange");
            Program.SoundManager.SetVolume(csgo.GetValue<float>("soundEspVolume") / 100f);

            TimeSpan span = new TimeSpan(DateTime.Now.Ticks - lastBeep);
            if (span.TotalMilliseconds > maxSpan)
            {
                lastBeep = DateTime.Now.Ticks;
                return;
            }
            float minRange = maxRange / maxSpan * (float)span.TotalMilliseconds;
            LastPercent = 100f / maxSpan * (float)span.TotalMilliseconds;

            float leastDist = float.MaxValue;
            foreach (Player player in csgo.Players)
            {
                if (player == null)
                    continue;
                if (player.Health == 0)
                    continue;
                if (player.InTeam == localPlayer.InTeam)
                    continue;
                float dist = localPlayer.DistanceToOtherEntityInMetres(player);
                if (dist <= minRange)
                {
                    leastDist = dist;
                    break;
                }
            }

            if(leastDist != float.MaxValue)
            {
                Program.SoundManager.Play((int)csgo.GetValue<float>("soundEspSound") - 1);
                Thread.Sleep(50);
                lastBeep = DateTime.Now.Ticks;
            }
        }
        #endregion
    }
}
