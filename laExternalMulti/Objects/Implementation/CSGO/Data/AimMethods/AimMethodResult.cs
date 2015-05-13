using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.Data.AimMethods
{
    public class AimMethodResult
    {
        #region VARIABLES
        private int playerIndex;
        private Vector2 aimPos;
        #endregion

        #region PROPERTIES
        public int PlayerIndex { get { return playerIndex; } }
        public Vector2 AimPosition { get { return aimPos; } }
        #endregion

        #region CONSTRUCTOR
        public AimMethodResult(int playerIndex, Vector2 aimPos)
        {
            this.playerIndex = playerIndex;
            this.aimPos = aimPos;
        }
        #endregion

        #region METHODS
        public bool IsValid()
        {
            Player player = ((CSGOImplementation)Program.GameImplementation).GetPlayerByIndex(playerIndex);
            if (player == null)// || aimPos == Vector2.Zero)
                return false;
            return player.Health > 0;
        }
        #endregion
    }
}
