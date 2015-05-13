using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.Data.AimMethods
{
    public abstract class AimMethodImplementation
    {
        #region VARIABLES
        private long lastTick;
        #endregion

        #region PROPERTIES
        protected long LastTick { get { return lastTick; } }
        #endregion

        #region METHODS
        protected abstract AimMethodResult OnGetAimPoint(CSGOImplementation csgo, Target target, AimBone bone, Vector2 screenM, float radius);

        public AimMethodResult GetAimTarget(CSGOImplementation csgo, Target target, AimBone bone, Vector2 screenM, float radius)
        {
            if (!CSGOImplementationValid(csgo))
                return null;

            this.CheckTickLength();

            AimMethodResult least = OnGetAimPoint(csgo, target, bone, screenM, radius);

            this.UpdateTick();

            return least;
        }
        public Vector2 AimAt(CSGOImplementation csgo, AimBone bone, Player player)
        {
            if (player == null)
                return Vector2.Zero;
            //float multiplicator = GetFloatMultiplicator();
            return Geometry.WorldToScreen(
                    csgo.ViewMatrix,
                    csgo.ScreenSize,
                    player.Skeleton.GetBone(bone)// + ((player.Velocity + player.BaseVelocity) - (csgo.LocalPlayer.Velocity + csgo.LocalPlayer.BaseVelocity)) * GetFloatMultiplicator() 
                );
        }
        protected bool CSGOImplementationValid(CSGOImplementation csgo)
        {
            if (csgo == null)
                return false;
            if (csgo.LocalPlayer == null)
                return false;
            if (!csgo.LocalPlayer.IsValid())
                return false;
            if (csgo.Players == null)
                return false;
            if (csgo.Players.Length < 2)
                return false;
            return true;
        }

        protected bool PlayerValid(Player localPlayer, Player enemy, Target target)
        {
            if (enemy == null)
                return false;
            if (enemy.Address == localPlayer.Address)
                return false;
            if (enemy.Index == localPlayer.Index)
                return false;
            if (!enemy.IsValid())
                return false;
            if (target == Target.Allies && enemy.InTeam != localPlayer.InTeam)
                return false;
            if (target == Target.Enemies && enemy.InTeam == localPlayer.InTeam)
                return false;
            //if (!Geometry.PointSeesPoint(localPlayer.Vector2, enemy.Vector2, Player.FOV_DEGREE, localPlayer.Yaw))
            //    return false;
            //if (!enemy.IsVisible())
            //    return false;
            CSGOImplementation csgo = (CSGOImplementation)Program.GameImplementation;
            //if (!csgo.CurrentMap.IsVisible(localPlayer.Vector3 + Vector3.UnitZ * (enemy.Skeleton.Head.Z - enemy.Skeleton.LeftFoot.Z), enemy.Skeleton.Head))
            //    return false;
            //if(!enemy.IsSpotted)
            //    return false;
            return true;
        }

        protected void CheckTickLength()
        {
            if (lastTick == 0L || lastTick > TimeSpan.TicksPerSecond)
                lastTick = DateTime.Now.Ticks;
        }

        protected void UpdateTick()
        {
            lastTick = DateTime.Now.Ticks;
        }

        public float GetFloatMultiplicator()
        {
            return (float)((DateTime.Now.Ticks - lastTick) / TimeSpan.TicksPerMillisecond) / 1000f;
        }
        #endregion
    }
}
