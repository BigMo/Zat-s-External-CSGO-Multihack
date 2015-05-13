using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.Data.AimMethods
{
    class NearestToPlayer : AimMethodImplementation
    {
        #region METHODS
        protected override AimMethodResult OnGetAimPoint(CSGOImplementation csgo, Target target, AimBone bone, Vector2 screenM, float radius)
        {
            Vector2 least = Vector2.Zero;
            Player playerTmp = null;
            float leastDist = float.MaxValue;

            try
            {
                foreach (Player player in csgo.Players)
                {
                    if (!PlayerValid(csgo.LocalPlayer, player, target))
                        continue;
                    if (csgo.GetValue<YesNo>("aimSpottedOnly") == YesNo.Yes && !player.SeenBy(csgo.LocalPlayer))
                        continue;

                    float multiplicator = this.GetFloatMultiplicator();
                    Vector2 head = AimAt(csgo, bone, player);
                    if (!Geometry.PointInCircle(head, screenM, radius))
                        continue;

                    float playerDist = Geometry.GetDistanceToPoint(player.Vector3, csgo.LocalPlayer.Vector3);

                    if (playerDist < leastDist)
                    {
                        least = head;
                        leastDist = playerDist;
                        playerTmp = player;
                    }
                }
            }
            catch { }
            return new AimMethodResult(playerTmp != null ? playerTmp.Index : 0, least);
        }
        #endregion
    }
}
