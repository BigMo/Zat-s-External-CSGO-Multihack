using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.Data.AimMethods
{
    class RageMode : AimMethodImplementation
    {
        #region METHODS
        protected override AimMethodResult OnGetAimPoint(CSGOImplementation csgo, Target target, AimBone bone, Vector2 screenMid, float radius)
        {
            Vector2 least = Vector2.Zero;
            int index = -1;
            float leastDist = float.MaxValue, dist = 0f;
            Vector3 currentAngles = csgo.ViewAngles;
            Vector3 aimAngles = Vector3.Zero;

            for (int i = 0; i < csgo.Players.Length; i++)
            {
                if (csgo.Players[i] == null)
                    continue;
                if (!csgo.LocalPlayer.SeenBy(csgo.Players[i]) && !csgo.Players[i].SeenBy(csgo.LocalPlayer))
                    continue;
                aimAngles = Geometry.CalcAngle(csgo.LocalPlayer.Vector3 + csgo.ViewOffset, csgo.Players[i].Skeleton.GetBone(bone));
                aimAngles -= currentAngles;
                dist = aimAngles.Length();
                if(dist < leastDist)
                {
                    leastDist = dist;
                    index = csgo.Players[i].Index;
                }
            }
            return new AimMethodResult(index, least);
        }
        #endregion
    }
}
