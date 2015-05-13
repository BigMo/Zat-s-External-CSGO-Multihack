using laExternalMulti.Objects.Implementation.CSGO.Data;
using laExternalMulti.Objects.UI;
using SharpDX;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.UI
{
    class Crosshair : CSGOControl
    {
        #region VARIABLES
        private Color foreColor = new Color(0.8f, 0.8f, 0.8f, 0.9f);
        private Color backColor = new Color(0.0f, 0.0f, 0.0f, 0.75f);
        private Color aimBackColor = new Color(0.0f, 0.0f, 0.0f, 0.25f);
        private Color colorCT = new Color(0.5f, 0.8f, 0.9f, 0.9f);
        private Color colorT = new Color(0.9f, 0.1f, 0.1f, 0.9f);
        private CSGOImplementation csgo;
        private const float 
            c4BoxWidth = 300f,
            c4BoxHeight = 32f,
            c4BoxPaddingOuter = 16f,
            c4BoxMarginInner = 2f;
        private const float
            xhairWidth = 64f;
        private const float
            specPanelWidth = 260f,
            specPanelPaddingOuter = 16f,
            specPanelMarginNames = 6f,
            specPanelExclMrk = 6f;
        #endregion

        #region CONSTRUCTOR
        public Crosshair(CSGOTheme theme, TextFormat font) : base(theme, font) { }
        #endregion

        #region METHODS
        protected override void OnDraw(SharpDX.Direct2D1.WindowRenderTarget device)
        {
            csgo = (CSGOImplementation)Program.GameImplementation;
            if (csgo == null)
                return;
            CSGOGameController csController = (CSGOGameController)Program.GameController;
            Player currentPlayer = csgo.GetCurrentPlayer();
            if (currentPlayer == null)
                return;

            float screenW = csgo.ScreenSize.Width / 2f, screenH = csgo.ScreenSize.Height / 2f;//, width = 64;
            float multiplier = GetColorMultiplier();

            #region aimbot radius
            if (Program.GameImplementation.GetValue<YesNo>("aimbotEnabled") == YesNo.Yes)
            {
                float aimRadius = Program.GameImplementation.GetValue<float>("aimbotRadius");
                FillEllipse(device,
                    aimBackColor,
                    screenW,
                    screenH,
                    aimRadius * 2,
                    aimRadius * 2,
                    true
                );
                DrawEllipse(device,
                    backColor,
                    screenW,
                    screenH,
                    aimRadius * 2,
                    aimRadius * 2,
                    true
                );
            }
            #endregion
            #region soundesp
            if (Program.GameImplementation.GetValue<YesNo>("crosshairDrawSoundESP") == YesNo.Yes)
            {
                if (csController.SoundESP.LastPercent != 0f)
                {
                    float size = 250f * csController.SoundESP.LastPercent / 100f;
                    DrawEllipse(device, aimBackColor, screenW, screenH, size, size, true, 2f);
                }
            }
            #endregion
            #region recoil
            if (Program.GameImplementation.GetValue<YesNo>("crosshairDrawRecoil") == YesNo.Yes)
            {
                if (csgo.LocalPlayer.PunchVector.Length() != 0f)
                {
                    float x = Program.GameController.Form.Width / 2f;
                    float y = Program.GameController.Form.Height / 2f;
                    float dy = Program.GameController.Form.Height / 90f;
                    float dx = Program.GameController.Form.Width / 90f;
                    x -= (dx * (csgo.LocalPlayer.PunchVector.Y));
                    y += (dy * (csgo.LocalPlayer.PunchVector.X));
                    
                    //float pixelPerDeg = 1680f / 90f;
                    //float distX = csgo.LocalPlayer.PunchVector.Y * 2;
                    //float distY = csgo.LocalPlayer.PunchVector.X * 2 * -1f;
                    //float x = csgo.ScreenSize.Width / 2f - distX * pixelPerDeg;
                    //float y = csgo.ScreenSize.Height / 2f - distY * pixelPerDeg;
                    DrawLine(device, colorT * 0.5f, x - 16f, y, x + 16f, y, 4f);
                    DrawLine(device, colorT * 0.5f, x, y - 16f, x, y + 16f, 4f);
                }
            }
            #endregion
            #region spectator
            if (Program.GameImplementation.GetValue<YesNo>("spectatorDrawWarning") == YesNo.Yes)
            {
                float height = 22f + 20f * csgo.Spectators.Count;
                float specx = screenW - specPanelWidth / 2f, specy = csgo.ScreenSize.Height - 4 - height;

                if (csgo.Spectators.Count > 0)
                {
                    FillRectangle(device,
                        aimBackColor,
                        specx,
                        specy,
                        specPanelWidth,
                        height
                    );
                    FillRectangle(device,
                        aimBackColor,
                        specx,
                        specy,
                        specPanelWidth,
                        height
                    );
                    DrawText(device,
                        colorT,
                        backColor,
                        specx + specPanelMarginNames,
                        specy + 2,
                        100,
                        20,
                        1,
                        1,
                        "Spectators:",
                        FactoryManager.GetFont("largeSegoe"));
                }
                for (int i = 0; i < csgo.Spectators.Count; i++)
                {
                    try
                    {
                        DrawText(device,
                            csgo.Spectators[i].SpectatorView == Data.Enums.SpectatorView.Ego ?
                                colorT * (0.75f + 0.25f * multiplier) :
                                colorT,
                            backColor,
                            specx + specPanelMarginNames,
                            specy + 2f + (i + 1) * 20f,
                            256,
                            20,
                            1,
                            1,
                            String.Format("{0} ({1})", csgo.Spectators[i].Name, csgo.Spectators[i].SpectatorView),
                            FactoryManager.GetFont("smallSegoe"));
                        if (csgo.Spectators[i].SpectatorView == Data.Enums.SpectatorView.Ego)
                        {
                            DrawText(device,
                                colorT * (0.75f + 0.25f * multiplier),
                                backColor,
                                specx - specPanelExclMrk,
                                specy + 2f + (i + 1) * 20f,
                                24f,
                                20,
                                1,
                                1,
                                "!",
                                FactoryManager.GetFont("smallSegoe"));
                        }
                    }
                    catch { }
                }
            }
            #endregion
            #region crosshair
            if (Program.GameImplementation.GetValue<YesNo>("crosshairEnabled") == YesNo.Yes)
            {
                float inaccuracy = 0f;
                Color drawColor = backColor;
                if (csgo.TargetPlayer != null)
                {
                    Entity targetPlayer = (Entity)csgo.TargetPlayer.Clone();
                    drawColor = targetPlayer.InTeam == Data.Team.CounterTerrorists ? colorCT : colorT;
                    DrawText(device,
                        foreColor,
                        backColor,
                        screenW + 2f,
                        screenH + 2f,
                        100f,
                        20f,
                        1f,
                        1f,
                        targetPlayer.Name,
                        FactoryManager.GetFont("smallSegoe"));
                    DrawText(device,
                        foreColor,
                        backColor,
                        screenW + 2f,
                        screenH - 20f,
                        100f,
                        20f,
                        1f,
                        1f,
                        targetPlayer.Health.ToString() + "HP",
                        FactoryManager.GetFont("smallSegoe"));
                }
                if (csgo.AccuracyPenality > 0.0f)
                {
                    inaccuracy = csgo.AccuracyPenality / 0.002f;
                    DrawEllipse(device,
                        drawColor * 0.25f,
                        screenW,
                        screenH,
                        4f * inaccuracy,
                        4f * inaccuracy,
                        true
                    );
                }
                //Left
                DrawLine(device,
                    drawColor,
                    screenW - xhairWidth / 2f,
                    screenH,
                    screenW + xhairWidth / 2f,
                    screenH,
                    1f);
                //Top
                DrawLine(device,
                    drawColor,
                    screenW,
                    screenH - xhairWidth / 2f,
                    screenW,
                    screenH + xhairWidth / 2f,
                    1f);
                if (inaccuracy > 0f)
                {
                    float bar = xhairWidth / 16f * inaccuracy * 0.50f;
                    if (bar > xhairWidth / 2f)
                        bar = xhairWidth / 2f;
                    //Left
                    DrawLine(device,
                        drawColor,
                        screenW - 2f * inaccuracy,
                        screenH - bar / 2f,
                        screenW - 2f * inaccuracy,
                        screenH + bar / 2f,
                        2f);
                    //Right
                    DrawLine(device,
                        drawColor,
                        screenW + 2f * inaccuracy,
                        screenH - bar / 2f,
                        screenW + 2f * inaccuracy,
                        screenH + bar / 2f,
                        2f);
                    //Up
                    DrawLine(device,
                        drawColor,
                        screenW - bar / 2f,
                        screenH - 2f * inaccuracy,
                        screenW + bar / 2f,
                        screenH - 2f * inaccuracy,
                        2f);
                    //Down
                    DrawLine(device,
                        drawColor,
                        screenW - bar / 2f,
                        screenH + 2f * inaccuracy,
                        screenW + bar / 2f,
                        screenH + 2f * inaccuracy,
                        2f);
                }
            }
            #endregion
            #region spotted
            if (currentPlayer != null)
            {
                if (currentPlayer.IsSpotted)
                {
                    DrawEllipse(device,
                        colorT * (0.5f + 0.5f * multiplier),
                        screenW,
                        screenH,
                        xhairWidth,
                        xhairWidth,
                        true,
                        3f);
                }
            }
            #endregion
            }
        #endregion
    }
}
