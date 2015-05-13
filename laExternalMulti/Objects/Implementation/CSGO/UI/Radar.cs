using laExternalMulti.Objects.Implementation.CSGO.Data;
using laExternalMulti.Objects.Implementation.CSGO.Data.Enums;
using laExternalMulti.Objects.Implementation.CSGO.Updaters;
using laExternalMulti.Objects.UI;
using SharpDX;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.UI
{
    class Radar : CSGOControl
    {
        #region VARIABLES
        //private SharpDX.Color foreColor = new Color(0.8f, 0.8f, 0.8f, 0.9f);
        //private SharpDX.Color backColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        //private SharpDX.Color lifeBarForeground = new Color(0.2f, 0.8f, 0.2f, 0.2f);
        //private SharpDX.Color lifeBarBackground = new Color(0.2f, 0.2f, 0.2f, 0.2f);
        //private SharpDX.Color colorCT = new Color(0.5f, 0.8f, 0.9f, 0.9f);
        //private SharpDX.Color colorT = new Color(0.9f, 0.1f, 0.1f, 0.9f);
        //private SharpDX.Color viewColor = new Color(1f, 0.75f, 0.1f, 0.33f);
        //private SharpDX.Color viewColorOutline = new Color(1f, 0.75f, 0.1f, 0.66f);
        //private SharpDX.Color lineColor = new Color(0.9f, 0.9f, 0.9f, 0.2f);
        private float resolution, dotSize, viewY, viewX;
        private CSGOImplementation csgo;
        #endregion

        #region CONSTRUCTOR
        public Radar(CSGOTheme theme, TextFormat font, float resolution) : base(theme, font) {
            this.Width = 256f;
            this.Height = 256f;
            this.dotSize = 6f;
            this.viewX = 24f;
            this.viewY = 48f;
        }
        public Radar(CSGOTheme theme, TextFormat font) 
            : this(theme, font, 10f) { }
        #endregion

        #region METHODS
        protected override void OnDraw(SharpDX.Direct2D1.WindowRenderTarget device)
        {
            csgo = (CSGOImplementation)Program.GameImplementation;

            //Draw background
            FillRectangle(device, CSGOTheme.BackColor, X, Y, Width, Height);
            if (csgo.SignOnState < SignOnState.SIGNONSTATE_NEW || csgo.SignOnState > SignOnState.SIGNONSTATE_FULL)
            {
                DrawText(device, this.Theme.ForeColor * (0.75f + 0.25f * GetColorMultiplier()), this.X + this.Width / 2f - 50f, this.Y + this.Height / 2f + 8f, 200, 20, "Not connected", this.Font);
                return;
            }

            Player currentPlayer = csgo.GetCurrentPlayer();
            if (csgo.Players == null)
                return;

            //Check validity
            if (csgo.Players == null)
                return;
            if (csgo.Players.Length == 0)
                return;
            if (currentPlayer == null)
                return;
            if (csgo.GetValue<YesNo>("radarEnabled") == YesNo.No)
                return;
            resolution = csgo.GetValue<float>("radarZoom");
            Vector2 screenMid = new Vector2(X + Width / 2f, Y + Height / 2f);
            float scale = 2f / resolution;

            #region SoundESP
            if (csgo.GetValue<YesNo>("soundEspEnabled") == YesNo.Yes)
            {
                //float maxSpan = csgo.GetValue<float>("soundEspInterval");
                float maxRange = csgo.GetValue<float>("soundEspRange");
                maxRange /= 0.01905f;
                if ((maxRange * scale * 2f) <= this.Width)
                    DrawEllipse(device, CSGOTheme.Line, screenMid.X, screenMid.Y, maxRange * scale * 2f, maxRange * scale * 2f, true, 1f);
                SoundESP sEsp = ((CSGOGameController)Program.GameController).SoundESP;
                if (((maxRange * scale * 2f) / 100f * sEsp.LastPercent) <= this.Width)
                DrawEllipse(device, CSGOTheme.Line, screenMid.X, screenMid.Y, (maxRange * scale * 2f) / 100f * sEsp.LastPercent, (maxRange * scale * 2f) / 100f * sEsp.LastPercent, true, 1f);
            }
            #endregion

            //Draw other players
            try
            {
                foreach (Player player in csgo.Players)
                {
                    if (player != null)
                        DrawPlayer(device, currentPlayer, player, screenMid, scale);
                }
            }
            catch { }
            //foreach (Entity entity in csgo.Entities)
            //{
            //    DrawEntity(device, currentPlayer, entity, screenMid, scale);
            //}

            //Draw "view"
            DrawText(device, CSGOTheme.ForeColor, X + 4, Y + 4, 100, 20, "Zoom: x" + Math.Round(1 / resolution, 2), FactoryManager.GetFont("smallSegoe"));
            FillPolygon(device, CSGOTheme.ViewColor, screenMid.X, screenMid.Y, screenMid.X - viewX * scale, screenMid.Y - viewY * scale, screenMid.X + viewX * scale, screenMid.Y - viewY * scale);
            DrawPolygon(device, CSGOTheme.ViewColorOutline, screenMid.X, screenMid.Y, screenMid.X - viewX * scale, screenMid.Y - viewY * scale, screenMid.X + viewX * scale, screenMid.Y - viewY * scale);

            //Draw player
            FillEllipse(device,
                currentPlayer.InTeam == Team.CounterTerrorists ? CSGOTheme.TeamCT : CSGOTheme.TeamT,
                screenMid.X,
                screenMid.Y,
                dotSize * scale,
                dotSize * scale,
                true);
        }

        private void DrawPlayer(SharpDX.Direct2D1.WindowRenderTarget device, Player currentPlayer, Player player, Vector2 screenMid, float scale)
        {
            if (player == null)
                return;
            if (!player.IsValid())
                return;
            if (player.Index == currentPlayer.Index)
                return;
            if (csgo.GetValue<Target>("radarDrawTarget") == Target.Enemies && player.InTeam == currentPlayer.InTeam)
                return;
            if (csgo.GetValue<Target>("radarDrawTarget") == Target.Allies && player.InTeam != currentPlayer.InTeam)
                return;
            Vector2 point = player.Vector2;
            point.X = (currentPlayer.X - player.X) * scale * -1;
            point.Y = (currentPlayer.Y - player.Y) * scale;
            bool highlighted = csgo.Highlighted[player.Index - 1];


            if(point.Length() > this.Width / 2f)
            {
                point.Normalize();
                point *= this.Width / 2f;
            }

            point += screenMid;
            point = Geometry.RotatePoint(point, screenMid, currentPlayer.Yaw - 90);

            //If player is in range
            if (csgo.GetValue<OnOff>("radarDrawLines") == OnOff.On)
            {
                DrawLine(device, CSGOTheme.Line, point.X, point.Y, screenMid.X, screenMid.Y, 1f);
            }

            FillEllipse(device,
                player.InTeam == Team.CounterTerrorists ? CSGOTheme.TeamCT : CSGOTheme.TeamT,
                point.X + dotSize / 2f,
                point.Y + dotSize / 2f,
                dotSize,
                dotSize,
                true);

            if (highlighted)
            {
                DrawEllipse(device,
                    (player.InTeam == Team.Terrorists ? CSGOTheme.TeamT : CSGOTheme.TeamCT) * (DateTime.Now.Millisecond % 1000f / 1000f),
                    point.X + (dotSize / 2f * scale),
                    point.Y + (dotSize / 2f * scale),
                    dotSize * scale * 4f,
                    dotSize * scale * 4f,
                    true);
            }
        }

        private void DrawEntity(SharpDX.Direct2D1.WindowRenderTarget device, Player currentPlayer, Entity entity, Vector2 screenMid, float scale)
        {
            if (entity == null)
                return;
            if (!entity.IsValid())
                return;
            if (entity.Address == currentPlayer.Address)
                return;
            if (entity.ClassID == Data.Enums.ClassID.Weapon)
                if (entity.OwnerEntity != -1)
                    return;
            Vector2 point = entity.Vector2;
            point.X = screenMid.X + (currentPlayer.X - entity.X) * scale * -1;
            point.Y = screenMid.X + (currentPlayer.Y - entity.Y) * scale;

            point = Geometry.RotatePoint(point, screenMid, currentPlayer.Yaw - 90);

            //If player is in range
            //if (point.X >= X && point.X <= X + Width && point.Y >= Y && point.Y <= Y + Height && csgo.GetValue<OnOff>("radarDrawView") == OnOff.On)
            //{
            //    Vector2 view1 = point;
            //    view1.Y -= viewY * scale;
            //    view1.X += viewX * scale;
            //    view1 = Geometry.RotatePoint(view1, point, currentPlayer.Yaw - entity.Yaw);
            //    Vector2 view2 = point;
            //    view2.Y -= viewY * scale;
            //    view2.X -= viewX * scale;
            //    view2 = Geometry.RotatePoint(view2, point, currentPlayer.Yaw - entity.Yaw);
            //    FillPolygon(device, viewColor, point, view1, view2);
            //    DrawPolygon(device, viewColorOutline, point, view1, view2);
            //}

            if (point.X < X)
                point.X = X + dotSize / 2f;
            if (point.Y < Y)
                point.Y = Y + dotSize / 2f;
            if (point.X > X + Width)
                point.X = X + Width - dotSize / 2f;
            if (point.Y > Y + Height)
                point.Y = Y + Height - dotSize / 2f;
           
            //if (csgo.GetValue<OnOff>("radarDrawLines") == OnOff.On)
            //{
            //    DrawLine(device, enemyColor, point.X, point.Y, screenMid.X, screenMid.Y, 1f);
            //}

            FillEllipse(device,
                CSGOTheme.LifebarForeground,
                point.X,
                point.Y,
                dotSize * scale,
                dotSize * scale,
                true);
        }
        #endregion
    }
}
