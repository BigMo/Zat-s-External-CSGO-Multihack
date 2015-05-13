using laExternalMulti.Objects.Implementation.CSGO.Data;
using laExternalMulti.Objects.Implementation.CSGO.Data.Enums;
using laExternalMulti.Objects.UI;
using SharpDX;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.UI
{
    class PlayerInformation : CSGOControl
    {
        #region VARIABLES
        private CSGOImplementation csgo;
        private ProgressBar 
            barSpeed, barRecoil, barSpread, barKD,
            barMem, barInp, barAim, barTrg,
            barRcs, barCPU, barDrw, barTick;
        private Label lblWeaponInfo, lblServerData, lblMapName, lblState, lblServerIP, lblDataRead, lblDataWritten;
        private const int PERFBARS = 4;
        private static string[] spinners = new string[] { "", ".", "..", "..."};
        private static int spinnerCnt;
        private static long spinTick;
        #endregion

        #region CONSTRUCTOR
        public PlayerInformation(CSGOTheme theme, ProgressBarTheme pbthemeGreen, ProgressBarTheme pbthemeRedToGreen, ProgressBarTheme pbthemeGreenToRed, TextFormat font, TextFormat fontPerfIndicators, float x, float y, float width, float height)
            : base(theme, font, x, y, width, height)
        {
            //Theme themeRedToGreen = new Objects.UI.Theme(themeGreenToRed.ShadowColor, themeGreenToRed.BackColor, themeGreenToRed.BorderColor, themeGreenToRed.ForeColor, themeGreenToRed.ShadowOffsetX, themeGreenToRed.ShadowOffsetY);
           
            barSpeed = new ProgressBar(pbthemeGreen, this.Font, this.X, this.Y, this.Width, 20f, 0f, 100f, false);
            barRecoil = new ProgressBar(pbthemeGreenToRed, this.Font, this.X, this.Y + 22f, this.Width, 20f, 0f, 100f);
            barSpread = new ProgressBar(pbthemeGreenToRed, this.Font, this.X, this.Y + 44f, this.Width, 20f, 0f, 100f);
            barKD = new ProgressBar(pbthemeGreenToRed, this.Font, this.X, this.Y + 66f, this.Width, 20f, 0f, 10f);

            barMem = new ProgressBar(pbthemeRedToGreen, fontPerfIndicators, this.X, this.Y + 88f, this.Width / PERFBARS, 20f, 0, 60);
            barInp = new ProgressBar(pbthemeRedToGreen, fontPerfIndicators, this.X + this.Width / PERFBARS * 1, this.Y + 88f, this.Width / PERFBARS, 20f, 0, 60);
            barAim = new ProgressBar(pbthemeRedToGreen, fontPerfIndicators, this.X + this.Width / PERFBARS * 2, this.Y + 88f, this.Width / PERFBARS, 20f, 0, 60);
            barTrg = new ProgressBar(pbthemeRedToGreen, fontPerfIndicators, this.X + this.Width / PERFBARS * 3, this.Y + 88f, this.Width / PERFBARS, 20f, 0, 60);
            
            barRcs = new ProgressBar(pbthemeRedToGreen, fontPerfIndicators, this.X, this.Y + 110f, this.Width / PERFBARS, 20f, 0, 60);
            barCPU = new ProgressBar(pbthemeGreenToRed, fontPerfIndicators, this.X + this.Width / PERFBARS * 1, this.Y + 110f, this.Width / PERFBARS, 20f, 0, 100);
            barDrw = new ProgressBar(pbthemeRedToGreen, fontPerfIndicators, this.X + this.Width / PERFBARS * 2, this.Y + 110f, this.Width / PERFBARS, 20f, 0, 60);
            barTick = new ProgressBar(pbthemeRedToGreen, fontPerfIndicators, this.X + this.Width / PERFBARS * 3, this.Y + 110f, this.Width / PERFBARS, 20f, 0, 60);

            lblWeaponInfo = new Label(theme, font, this.X + 4, this.Y + 132f, this.Width, 20f, "");
            lblServerData = new Label(theme, font, this.X + 4, this.Y + 154f, this.Width, 20f, "");
            lblServerIP = new Label(theme, font, this.X + 4, this.Y + 176f, this.Width, 20f, "");
            lblMapName = new Label(theme, font, this.X + 4, this.Y + 198f, this.Width, 20f, "");
            lblState = new Label(theme, font, this.X + 4, this.Y + 220f, this.Width, 20f, "");
            lblDataRead = new Label(theme, font, this.X + 4, this.Y + 242f, this.Width / 2f, 20f, "");
            lblDataWritten = new Label(theme, font, this.X + 4 + this.Width / 2f, this.Y + 242f, this.Width / 2f, 20f, "");
            
            //1
            this.AddChildControl(barSpeed);
            //2
            this.AddChildControl(barRecoil);
            //3
            this.AddChildControl(barSpread);
            //4
            this.AddChildControl(barKD);

            //5
            this.AddChildControl(barCPU);
            this.AddChildControl(barMem);
            this.AddChildControl(barInp);
            this.AddChildControl(barAim);
            //6
            this.AddChildControl(barTrg);
            this.AddChildControl(barDrw);
            this.AddChildControl(barTick);
            this.AddChildControl(barRcs);

            //7
            this.AddChildControl(lblWeaponInfo);
            //8
            this.AddChildControl(lblServerData);
            //9
            this.AddChildControl(lblServerIP);
            //10
            this.AddChildControl(lblMapName);
            //11
            this.AddChildControl(lblState);
            //12
            this.AddChildControl(lblDataRead);
            this.AddChildControl(lblDataWritten);

            this.Height = 22f * 12f;
        }
        #endregion

        #region METHODS

        public override float X
        {
            get { return base.X; }
            set
            {
                float offset = value - this.X;
                foreach (Control child in ChildControls)
                    child.X += offset;
                base.X = value;
            }
        }
        protected override void OnDraw(SharpDX.Direct2D1.WindowRenderTarget device)
        {
            csgo = (CSGOImplementation)Program.GameImplementation;

            FillRectangle(device, Theme.BackColor, this.X, this.Y, this.Width, this.Height);
            lblDataRead.Text = string.Format("Data read: {0}", GetSize(WinAPI.BytesRead));
            lblDataWritten.Text = string.Format("Data written: {0}", GetSize(WinAPI.BytesWritten));
            
            //Update performance-bars
            /*                    ((CSGOGameController)Program.GameController).MemoryUpdater.GetFrameRate().ToString(),
                    ((CSGOGameController)Program.GameController).InputUpdater.GetFrameRate().ToString(),
                    ((CSGOGameController)Program.GameController).TriggerBot.GetFrameRate().ToString(),
                    ((CSGOGameController)Program.GameController).AimBot.GetFrameRate().ToString()*/
            barMem.Value = ((CSGOGameController)Program.GameController).MemoryUpdater.GetFrameRate();
            barMem.Text = String.Format("MEM {0}", barMem.Value);
            
            barInp.Value = ((CSGOGameController)Program.GameController).InputUpdater.GetFrameRate();
            barInp.Text = String.Format("INP {0}", barInp.Value);
            
            barAim.Value = ((CSGOGameController)Program.GameController).AimBot.GetFrameRate();
            barAim.Text = String.Format("AIM {0}", barAim.Value);

            barTrg.Value = ((CSGOGameController)Program.GameController).TriggerBot.GetFrameRate();
            barTrg.Text = String.Format("TRG {0}", barTrg.Value);

            barDrw.Value = ((CSGOGameController)Program.GameController).Form.DrawUpdater.GetFrameRate();
            barDrw.Text = String.Format("DRW {0}", barDrw.Value);

            barTick.Value = ((CSGOGameController)Program.GameController).Form.TickUpdater.GetFrameRate();
            barTick.Text = String.Format("TCK {0}", barTick.Value);

            barRcs.Value = ((CSGOGameController)Program.GameController).RecoilControl.GetFrameRate();
            barRcs.Text = String.Format("RCS {0}", barRcs.Value);

            barCPU.Value = ((CSGOGameController)Program.GameController).PerformanceUpdater.CurrentValue;
            barCPU.Text = String.Format("CPU {0}", barCPU.Value);
            //Update playerinfo
            //if (currentPlayer == null)
            //    return;

            Player currentPlayer = csgo.GetCurrentPlayer();

            bool valid = currentPlayer != null;
            lblState.Text = String.Format("State: {0}", GetSignOnState(csgo.SignOnState));
            lblServerData.Text = String.Format("Server: {0}", csgo.ServerName);
            lblServerIP.Text = String.Format("IP: {0}", csgo.ServerIP);
            lblMapName.Text = String.Format("Current map: {0}", csgo.ServerMap);
            if (valid)
                valid = csgo.SignOnState == SignOnState.SIGNONSTATE_FULL;
            barSpeed.Enabled = valid;
            barRecoil.Enabled = valid;
            barSpread.Enabled = valid;
            barKD.Enabled = valid;

            if (valid)
            {
                //Velocity
                Vector2 velXY = new Vector2(currentPlayer.Velocity.X, currentPlayer.Velocity.Y);
                float length = velXY.Length();
                float speedPercent = 100f / 450f * (length % 450f);
                float speedMeters = length * 0.01905f;
                float speedKiloMetersPerHour = speedMeters * 60f * 60f / 1000f;

                barSpeed.Value = (int)speedPercent;
                barSpeed.Text = String.Format("{0} km/h", Math.Round(speedKiloMetersPerHour, 2));

                //Gun info
                if (Environment.TickCount - spinTick > 300)
                {
                    spinnerCnt++;
                    spinnerCnt %= spinners.Length;
                    spinTick = Environment.TickCount;
                }

                lblWeaponInfo.Text = String.Format(
                        "{0} [{1}] [{2}/{3}] {5}{4}",
                        csgo.WeaponName,
                        csgo.WeaponType,
                        csgo.WeaponClip1 > 0 && csgo.WeaponClip1 <= 200 && !csgo.IsReloading ?
                            csgo.WeaponClip1.ToString() :
                            (csgo.IsReloading ? "RELOADING" + spinners[spinnerCnt].ToString() : "-"),
                        csgo.WeaponClip2 > 0 && csgo.WeaponClip2 <= 500 ? csgo.WeaponClip2.ToString() : "-",
                        (csgo.IsShooting ? "[x]" : ""),
                        (csgo.WeaponShotsFired > 0 ? string.Format("[{0}] ", csgo.WeaponShotsFired.ToString()) : "")
                    );
                //lblShotsFired.Text = String.Format("Shots fired: {0}", csgo.ShotsFired);

                //Gun punch
                float percentage = 100f / 9f * currentPlayer.PunchVector.Length();

                barRecoil.Value = percentage;
                barRecoil.Text = String.Format("Recoil: {0}%", Math.Round(percentage, 0));

                //Gun spread
                percentage = 100f / 0.16f * csgo.AccuracyPenality;
                percentage = (float)Math.Min(percentage, 100f);

                barSpread.Value = percentage;
                barSpread.Text = String.Format("Spread: {0}%", Math.Round(percentage, 0));

                //KD
                if (currentPlayer.Deaths > 0)
                {
                    float kd = (float)currentPlayer.Kills / (float)Math.Max(1, currentPlayer.Deaths);
                    barKD.Value = kd;
                    barKD.Text = String.Format("k/d ratio: {0}", Math.Round(kd, 2));
                }
                else
                {
                    barKD.Value = 0;
                    barKD.Text = "k/d ratio: ∞";
                }
            }
            else
            {
                barSpeed.Value = 0f;
                barRecoil.Value = 0f;
                barSpread.Value = 0f;
                barKD.Value = 0f;

                barSpeed.Text = "";
                barRecoil.Text = "";
                barSpread.Text = "";
                barKD.Text = "";

                lblWeaponInfo.Text = "";
                lblServerData.Text = "";
                lblMapName.Text = "";
            }
        }

        private string GetSignOnState(SignOnState state)
        {
            switch (state)
            {
                case SignOnState.SIGNONSTATE_CHALLENGE:
                    return "Connection";
                case SignOnState.SIGNONSTATE_CHANGELEVEL:
                    return "Mapchange";
                case SignOnState.SIGNONSTATE_CONNECTED:
                    return "Connected";
                case SignOnState.SIGNONSTATE_FULL:
                    return "Playing";
                case SignOnState.SIGNONSTATE_NEW:
                    return "New";
                case SignOnState.SIGNONSTATE_NONE:
                    return "Menu";
                case SignOnState.SIGNONSTATE_PRESPAWN:
                    return "Prespawn";
                case SignOnState.SIGNONSTATE_SPAWN:
                    return "Spawn";
                default:
                    return "none";
            }
        }
        private static string GetSize(double size)
        {
            string[] sizes = new string[] { "B", "KB", "MB", "GB", "TB" };
            int index = 0;
            while (size >= 1024)
            {
                size /= 1024;
                index++;
            }
            return string.Format("{0}{1}", Math.Round(size, 2).ToString(), sizes[index]);
        }
        #endregion
    }
}
