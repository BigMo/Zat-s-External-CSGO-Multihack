using laExternalMulti.Objects.Implementation.CSGO.Data;
using laExternalMulti.Objects.UI;
using SharpDX;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.UI.StatsGraph
{
    public abstract class StatsGraph : Control
    {
        #region VARIABLES
        private CSGOImplementation csgo;
        private float min, max, elementWidth;
        protected SharpDX.Color colorCT = new Color(0.5f, 0.8f, 0.9f, 0.9f);
        protected SharpDX.Color colorT = new Color(0.9f, 0.1f, 0.1f, 0.9f);
        #endregion
        #region PROPERTIES
        public float MinValue { get { return min; } protected set { min = value; } }
        public float MaxValue { get { return max; } protected set { max = value; } }
        public float ElementWidth { get { return elementWidth; } protected set { elementWidth = value; } }
        protected CSGOImplementation CSGO { get { return csgo; } }
        #endregion
        #region CONSTRUCTOR
        public StatsGraph(Theme theme, TextFormat font, float x, float y, float width, float height, string text)
            : base(theme, font, x, y, width, height)
        {
            this.Text = text;
            this.elementWidth = 32;
        }
        #endregion
        #region METHODS
        protected override void OnDraw(SharpDX.Direct2D1.WindowRenderTarget device)
        {
            csgo = (CSGOImplementation)Program.GameImplementation;
            if (csgo == null)
                return;
            if (csgo.Players == null)
                return;

            this.UpdateBoundaries();
            int idx = 0, numPlayers = PlayersNum();
            this.Width = (numPlayers + 1) * elementWidth;

            //Base
            FillRectangle(device, Theme.BackColor * 0.5f, this.X, this.Y, this.Width, this.Height);
            DrawRectangle(device, Theme.BorderColor, this.X, this.Y, this.Width, this.Height);
            DrawText(device,
                Theme.ForeColor,
                this.X,
                this.Y + this.Height / 2f,
                100f,
                20f,
                this.Text,
                this.Font);
            //Min/Max
            DrawText(
                device,
                Theme.ForeColor,
                this.X + 2f,
                this.Y + 2f,
                20f,
                20f,
                this.MaxValue.ToString(),
                this.Font);
            DrawText(
                device,
                Theme.ForeColor,
                this.X + 2f,
                this.Y + this.Height - 20f,
                20f,
                20f,
                this.MinValue.ToString(),
                this.Font);
            try
            {
                foreach (Player player in csgo.Players)
                    if (player != null)
                        DrawPlayer(device, player, idx++, numPlayers);
            } catch { }
        }

        protected abstract void DrawPlayer(SharpDX.Direct2D1.WindowRenderTarget device, Player player, int index, int numPlayers);
        protected abstract void UpdateBoundaries();
        private int PlayersNum()
        {
            int num = 0;
            foreach (Player player in csgo.Players)
                if (player != null)
                    num++;
            return num;
        }
        private void EnframeText(SharpDX.Direct2D1.WindowRenderTarget device, float x, float y, float width, float height, string text, bool drawBgColor)
        {
            if(drawBgColor)
                FillRectangle(device, Theme.BackColor, x, y, width, height);
            DrawRectangle(device, Theme.BorderColor, x, y, height, height);
            DrawText(
                device,
                Theme.ForeColor,
                x + 2f,
                y + 2f,
                width - 4f,
                height - 4f,
                text,
                this.Font);
        }
        #endregion
    }
}
