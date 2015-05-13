using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.Data.AimMethods
{
    class NearestToCrosshair : AimMethodImplementation
    {
        #region METHODS
        protected override AimMethodResult OnGetAimPoint(CSGOImplementation csgo, Target target, AimBone bone, Vector2 screenMid, float radius)
        {
            Vector2 least = Vector2.Zero;
            int index = -1;
            float leastDist = float.MaxValue;
            try
            {
                for (int i = 0; i < csgo.Players.Length; i++)
                {
                    if (!PlayerValid(csgo.LocalPlayer, csgo.Players[i], target))
                        continue;
                    if (csgo.GetValue<YesNo>("aimSpottedOnly") == YesNo.Yes && !csgo.Players[i].SeenBy(csgo.LocalPlayer))
                        continue;

                    Vector2 head = AimAt(csgo, bone, csgo.Players[i]);
                    if (csgo.GetValue<OnOff>("aimbotRagemode") == OnOff.Off)
                        if (!Geometry.PointInCircle(head, screenMid, radius))
                            continue;

                    float dist = (float)Math.Abs((head - screenMid).Length());
                    //Debug.WriteLine("Dist {0}: {1}", csgo.Players[i].Name, Math.Round(dist, 4));
                    if (dist < leastDist)
                    {
                        least = head;
                        leastDist = dist;
                        index = csgo.Players[i].Index;
                    }
                }
            }
            catch { }
            return new AimMethodResult(index, least);
        }
        #endregion
    }
}
