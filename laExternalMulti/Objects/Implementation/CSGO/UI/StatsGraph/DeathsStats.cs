using laExternalMulti.Objects.Implementation.CSGO.Data;
using laExternalMulti.Objects.UI;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.UI.StatsGraph
{
    class DeathsStats : StatsGraph
    {
        #region CONSTRUCTOR
        public DeathsStats(Theme theme, TextFormat font, float x, float y, float width, float height)
            : base(theme, font, x, y, width, height, "Deaths") { }
        #endregion
        protected override void DrawPlayer(SharpDX.Direct2D1.WindowRenderTarget device, Player player, int index, int numPlayers)
        {
            float
                rectWidth = ElementWidth,
                rectX = this.X + rectWidth * (index + 1),
                rectHeight = this.Height / MaxValue * player.Deaths,
                rectY = this.Y + Height - rectHeight;
            FillRectangle(device,
                (player.InTeam == Team.CounterTerrorists ? colorCT : colorT) * (player.Index == CSGO.LocalPlayer.Index ? (0.75f + 0.25f * GetColorMultiplier()) : 1),
                rectX,
                rectY,
                rectWidth,
                rectHeight);
            DrawRectangle(device,
                Theme.BorderColor,
                rectX,
                rectY,
                rectWidth,
                rectHeight);
            float textY = rectY - 20f;
            if (textY < this.Y)
                textY = this.Y;
            else if (textY + 20f > this.Y + this.Height)
                textY = this.Y + this.Height - 20f;
            DrawText(device,
                Theme.ForeColor,
                Theme.ShadowColor,
                rectX,
                textY,
                rectWidth,
                20f,
                Theme.ShadowOffsetX,
                Theme.ShadowOffsetY,
                String.Format("{0} ({1})", player.Name, player.Deaths),
                this.Font);
        }

        protected override void UpdateBoundaries()
        {
            int max = int.MinValue;
            foreach(Player player in CSGO.Players)
            {
                if (player != null)
                {
                    if (player.Deaths > max)
                        max = player.Deaths;
                }
            }
            this.MaxValue = max;
            this.MinValue = 0;
        }
    }
}
