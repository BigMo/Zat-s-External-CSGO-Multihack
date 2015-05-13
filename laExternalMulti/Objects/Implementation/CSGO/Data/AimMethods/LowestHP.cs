using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.Data.AimMethods
{
    class LowestHP : AimMethodImplementation
    {
        #region METHODS
        protected override AimMethodResult OnGetAimPoint(CSGOImplementation csgo, Target target, AimBone bone, Vector2 screenM, float radius)
        {
            Vector2 least = Vector2.Zero;
            Player playerTmp = null;
            int leastHP = int.MaxValue;

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

                    if (player.Health < leastHP)
                    {
                        least = head;
                        leastHP = player.Health;
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
