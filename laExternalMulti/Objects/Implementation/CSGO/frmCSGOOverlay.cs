using laExternalMulti.Objects.Implementation.CSGO.Data;
using laExternalMulti.Objects.Implementation.CSGO.UI;
using laExternalMulti.Objects.Implementation.CSGO.UI.CSGOMenu;
using laExternalMulti.Objects.Implementation.CSGO.UI.StatsGraph;
using laExternalMulti.Objects.UI;
using laExternalMulti.Objects.UI.Menu;
using SharpDX;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO
{
    public class frmCSGOOverlay : frmOverlay
    {
        #region VARIABLES
        private long lastTick;
        private List<MenuItem> menuItems = new List<MenuItem>();
        private bool once = true;

        private Color panelBackColor = new Color(0.9f, 0.9f, 0.9f, 0.9f);
        private Color panelBackColorDark = new Color(0.6f, 0.6f, 0.6f, 0.9f);
        private Color lifeBarForeground = new Color(0.2f, 0.8f, 0.2f, 0.9f);
        private Color foreColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        private Color backColor = new Color(0.8f, 0.8f, 0.8f, 0.0f);
        private Color colorCrosshair = new Color(1f, 0f, 0f, 1f);
        private Color colorCT = new Color(0.5f, 0.8f, 0.9f, 0.9f);
        private Color colorT = new Color(1f, 0f, 0f, 1f);
        private Color viewColor = new Color(1f, 0.75f, 0.1f, 0.1f);
        private Color viewColorOutline = new Color(0.9f, 0.1f, 0.1f, 0.9f);
        private Color entrySelected = new Color(0.9f, 0.9f, 0.9f, 0.9f);
        private Color entryUnselected = new Color(0.8f, 0.8f, 0.8f, 0.8f);
        private Color lineColor = new Color(0.9f, 0.9f, 0.9f, 0.2f);

        private Theme 
            //themeBasic,
            themeItemSelected,
            themeBasic, 
            themeGreenToRed, 
            themeGreen;
        private CSGOTheme
            themeCSGO;
        private ProgressBarTheme
            pbthemeGreen,
            pbthemeGreenToRed,
            pbthemeRedToGreen;
        private ESP ctrlEsp;
        private Radar ctrlRadar;
        private Crosshair ctrlCrosshair;
        private PlayerInformation ctrlPlayerInformation;
        private KillsStats ctrlStatsKills;
        private DeathsStats ctrlStatsDeaths;
        private ScoreStats ctrlStatsScore;
        private KDStats ctrlStatsKD;
        public Label currentConfig;

        private TextFormat fontMini, fontTiny, fontSmall, fontLarge;

        #region CONTROLS
        public ValueMenuItem
            aimAllowAimJump, aimbotEnabled, aimbotCompensateRecoil, aimbotRagemode, aimbotSmooth, aimSpottedOnly,
            crosshairDrawData, crosshairEnabled, crosshairDrawRecoil, crosshairDrawSoundESP,
            espDrawBox, espDrawCircle, espDrawDetails, espDrawDistance, espDrawHealth, espDrawLines, espDrawName, espDrawSkeleton, espDrawWeapons, espEnabled, espGlowEnabled, espGlowFadingEnabled,
            radarDrawLines, radarDrawView, radarEnabled,
            rcsEnabled,
            spectatorDisableAim, spectatorDisableTrigger, spectatorDrawWarning,
            triggerbotEnabled, triggerbotSnipersOnly,
            miscInfoEnabled, miscAutoPistolEnabled, miscBunnyHopEnabled, miscNoFlash, miscNoSmoke,
            soundEspEnabled;

        public ValueMenuItem
            radarTarget,
            espTarget,
            aimbotTarget,
            triggerbotTarget;
        //public ValueMenuItem
        public TrackbarMenuItem 
            radarZoom,
            aimbotSpeed, aimbotRadius,
            espGlowFadingDistance,
            rcsForce,
            triggerbotDelay, triggerbotRecoilThreshold, triggerbotSpeedThreshold,
            soundEspInterval, soundEspRange, soundEspSound, soundEspVolume;
        public ValueMenuItem
            aimbotMethod, aimbotBone;
        public KeyMenuItem
            aimbotKey,
            triggerKey;
        private BackMenuItem
            radarBack,
            espBack, espGlowBack,
            aimBack,
            triggerBack,
            crosshairBack,
            rcsBack,
            spectatorBack,
            settingsBack,
            statsBack,
            miscBack,
            highlightPlayersBack,
            soundEspBack;
        private ValueMenuItem[]
            highlightPlayers;

        private Menu
            mainMenu,
            aimMenu,
            triggerMenu,
            radarMenu,
            espMenu, espGlowMenu,
            crosshairMenu,
            rcsMenu,
            spectatorMenu,
            soundEspMenu,
            settingsMenu,
            statsMenu,
            miscMenu,
            wireframeMenu;
        private Menu[]
            settingsFilesMenus;
        private Menu
            highlightPlayerMenu;
        private LoadActionMenuItem[] settingsLoadButtons;
        private SaveActionMenuItem[] settingsSaveButtons;
        #endregion

        #region OPTIONS
        private static object[] switchOnOff = new object[] { OnOff.On, OnOff.Off };
        private static object[] switchYesNo = new object[] { YesNo.Yes, YesNo.No };
        private static object[] switchTargets = new object[] { Target.Enemies, Target.Allies, Target.Everyone };
        private static object[] switchAimMethods = new object[] { AimMethod.NearestToCrosshair, AimMethod.NearestToPlayer, AimMethod.LowestHP };
        private static object[] switchAimBones = new object[] { AimBone.Head, AimBone.Neck, AimBone.Torso, AimBone.Hip, AimBone.Knees, AimBone.Feet, AimBone.Random, AimBone.RandomLethal, AimBone.RandomBody };
        #endregion

        #region CONSTANTS
        private const float
            trackBarRadarDistMin = 0f, trackBarRadarDistMax = 100f, trackBarRadarDistStep = 1f,
            trackBarAimbotSpeedMin = 20f, trackBarAimbotSpeedMax = 200f, trackBarAimbotSpeedStep = 5f,
            trackBarAimbotRadiusMin = 5f, trackBarAimbotRadiusMax = 1000f, trackBarAimbotRadiusStep = 5f,
            trackBarTriggerDelayMin = 0f, trackBarTriggerDelayMax = 1000f, trackBarTriggerDelayStep = 5f,
            trackBarTriggerRecoilTHMin = 0f, trackBarTriggerRecoilTHMax = 25f, trackBarTriggerRecoilTHStep = 0.25f,
            trackBarTriggerSpeedTHMin = 0f, trackBarTriggerSpeedTHMax = 30f, trackBarTriggerSpeedTHStep = 0.5f;
        #endregion
        #endregion
        protected override void OnInitDevice()
        {
            lastTick = DateTime.Now.Ticks;
            //Fonts
            FactoryManager.CreateFont("miniSegoe", "Segoe UI", 8.0f);
            FactoryManager.CreateFont("tinySegoe", "Segoe UI", 10.0f);
            FactoryManager.CreateFont("smallSegoe", "Segoe UI", 12.0f);
            FactoryManager.CreateFont("largeSegoe", "Segoe UI", 18.0f);
            fontMini = FactoryManager.GetFont("miniSegoe");
            fontTiny = FactoryManager.GetFont("tinySegoe");
            fontSmall = FactoryManager.GetFont("smallSegoe");
            fontLarge = FactoryManager.GetFont("largeSegoe");

            //Themes
            themeCSGO = new CSGOTheme(foreColor, panelBackColorDark, foreColor, Color.Transparent, 2f, 2f, colorCT, colorT, lifeBarForeground, panelBackColorDark, viewColor, viewColorOutline, lineColor);
            pbthemeGreenToRed = new ProgressBarTheme(foreColor, panelBackColorDark, foreColor, Color.Transparent, 2f, 2f, lifeBarForeground, colorT);
            pbthemeRedToGreen = new ProgressBarTheme(foreColor, panelBackColorDark, foreColor, Color.Transparent, 2f, 2f, colorT, lifeBarForeground);
            pbthemeGreen = new ProgressBarTheme(foreColor, panelBackColorDark, foreColor, Color.Transparent, 2f, 2f, lifeBarForeground);

            themeBasic = new Theme(foreColor, panelBackColorDark, foreColor, Color.Transparent, 0f, 0f);
            //themeBasic = new Theme(foreColor * 0.9f, panelBackColorDark * 0.9f, foreColor * 0.9f, Color.Transparent, 0f, 0f);
            themeItemSelected = new Theme(foreColor * 1.1f, panelBackColorDark * 1.1f, foreColor * 1.1f, Color.Transparent, 0f, 0f);
            /* 
             * ForeColor: BarLow
             * BackColor: Background
             * BorderColor: Border & Font
             * ShadowColor: BarHigh
             */

            //Controls
            #region CUSTOM CONTROLS
            themeGreenToRed = new Theme(lifeBarForeground, panelBackColorDark, foreColor, colorT, 0f, 0f);
            themeGreen = new Theme(lifeBarForeground, panelBackColorDark, foreColor, lifeBarForeground, 0f, 0f);

            ctrlRadar = new Radar(themeCSGO, fontSmall, 20f);
            ctrlRadar.SetPosition(4f, 4f);
            ctrlRadar.Width = 128f;
            ctrlRadar.Height = 128f;
            ctrlEsp = new ESP(themeCSGO, fontSmall);
            ctrlCrosshair = new Crosshair(themeCSGO, fontSmall);
            ctrlPlayerInformation = new PlayerInformation(
                themeCSGO,
                pbthemeGreen,
                pbthemeRedToGreen,
                pbthemeGreenToRed,
                fontSmall,
                fontTiny,
                ctrlRadar.X + ctrlRadar.Width + 4f, 
                4f, 
                256f, 
                22f
            );
            ctrlStatsKills = new KillsStats(themeBasic, fontTiny, 0, 0, 256f, 128);
            ctrlStatsDeaths = new DeathsStats(themeBasic, fontTiny, 0, 0, 256f, 128);
            ctrlStatsScore = new ScoreStats(themeBasic, fontTiny, 0, 0, 256f, 128);
            ctrlStatsKD = new KDStats(themeBasic, fontTiny, 0, 0, 256f, 128);
            #endregion

            #region MENUS
            //Menus
            aimMenu = new Menu(themeBasic, themeItemSelected, themeBasic, fontSmall, fontLarge, 0, 0, 256, 256, "[Aimbot]", 4f, 4f, 4f);
            crosshairMenu = new Menu(themeBasic, themeItemSelected, themeBasic, fontSmall, fontLarge, 0, 0, 256, 256, "[Crosshair]", 4f, 4f, 4f);
            currentConfig = new Label(themeBasic, fontSmall, 0, 0, 256, 12, "Current config: -");
            espMenu = new Menu(themeBasic, themeItemSelected, themeBasic, fontSmall, fontLarge, 0, 0, 256, 256, "[ESP]", 4f, 4f, 4f);
            espGlowMenu = new Menu(themeBasic, themeItemSelected, themeBasic, fontSmall, fontLarge, 0, 0, 256, 256, "[Outline-glow]", 4f, 4f, 4f);
            highlightPlayerMenu = new Menu(themeBasic, themeItemSelected, themeBasic, fontSmall, fontLarge, 0, 0, 256, 256, "[Highlight players]", 4f, 4f, 4f);
            mainMenu = new Menu(themeBasic, themeItemSelected, themeBasic, fontSmall, fontLarge, 4, ctrlRadar.Y + ctrlRadar.Height + 4f, 256, 100, @"[¯\_(ツ)_/¯] Zat's leaked", 4f, 4f, 4f, true);
            miscMenu = new Menu(themeBasic, themeItemSelected, themeBasic, fontSmall, fontLarge, 0, 0, 256, 256, "[Misc]", 4f, 4f, 4f);
            radarMenu = new Menu(themeBasic, themeItemSelected, themeBasic, fontSmall, fontLarge, 0, 0, 256, 256, "[Radar]", 4f, 4f, 4f);
            rcsMenu = new Menu(themeBasic, themeItemSelected, themeBasic, fontSmall, fontLarge, 0, 0, 256, 256, "[Recoil-Control]", 4f, 4f, 4f);
            settingsMenu = new Menu(themeBasic, themeItemSelected, themeBasic, fontSmall, fontLarge, 0, 0, 256, 256, "[Settings]", 4f, 4f, 4f);
            soundEspMenu = new Menu(themeBasic, themeItemSelected, themeBasic, fontSmall, fontLarge, 0, 0, 256, 256, "[SoundESP]", 4f, 4f, 4f);
            spectatorMenu = new Menu(themeBasic, themeItemSelected, themeBasic, fontSmall, fontLarge, 0, 0, 256, 256, "[Spectator]", 4f, 4f, 4f);
            statsMenu = new Menu(themeBasic, themeItemSelected, themeBasic, fontSmall, fontLarge, 0, 0, 512, 256, "[Statistics]", 4f, 4f, 4f);
            triggerMenu = new Menu(themeBasic, themeItemSelected, themeBasic, fontSmall, fontLarge, 0, 0, 256, 256, "[Triggerbot]", 4f, 4f, 4f);
            wireframeMenu = new Menu(themeBasic, themeItemSelected, themeBasic, fontSmall, fontLarge, 0, 0, 256, 256, "[Wireframe]", 4f, 4f, 4f);
            #endregion

            mainMenu.AddChildControl(currentConfig);

            #region MENU ITEMS - INIT
            //Init menuitems
            aimAllowAimJump = new ValueMenuItem(themeBasic, fontSmall, aimMenu, "Allow \"jumping\" between targets", switchYesNo, "aimAllowAimJump");
            aimAllowAimJump.OptionChanged += yesNoValChanged;
            aimbotBone = new ValueMenuItem(themeBasic, fontSmall, aimMenu, "Bone", switchAimBones, "aimbotBone");
            aimbotBone.OptionChanged += aimBoneValChanged;
            aimbotCompensateRecoil = new ValueMenuItem(themeBasic, fontSmall, aimMenu, "Compensate recoil (override)", switchOnOff, "aimbotCompensateRecoil");
            aimbotCompensateRecoil.OptionChanged += onOffValChanged;
            aimbotEnabled = new ValueMenuItem(themeBasic, fontSmall, aimMenu, "Enabled", switchYesNo, "aimbotEnabled");
            aimbotEnabled.OptionChanged += yesNoValChanged;
            aimbotKey = new KeyMenuItem(themeBasic, fontSmall, aimMenu, "Key", System.Windows.Forms.Keys.MButton, "aimbotKey");
            aimbotKey.OptionChanged += keyValChanged;
            aimbotMethod = new ValueMenuItem(themeBasic, fontSmall, aimMenu, "Method", switchAimMethods, "aimbotMethod");
            aimbotMethod.OptionChanged += aimMethodValChanged;
            aimbotRadius = new TrackbarMenuItem(themeBasic, fontSmall, aimMenu, "Radius (px)", trackBarAimbotRadiusMin, trackBarAimbotRadiusMax, trackBarAimbotRadiusStep, "aimbotRadius");
            aimbotRadius.ValueChanged += trackBar_ValueChanged;
            aimbotRagemode = new ValueMenuItem(themeBasic, fontSmall, aimMenu, "Rage mode", switchOnOff, "aimbotRagemode");
            aimbotRagemode.OptionChanged += onOffValChanged;
            aimbotSmooth = new ValueMenuItem(themeBasic, fontSmall, aimMenu, "Smooth aim", switchOnOff, "aimbotSmooth");
            aimbotSmooth.OptionChanged += onOffValChanged;
            aimbotSpeed = new TrackbarMenuItem(themeBasic, fontSmall, aimMenu, "Speed (%)", trackBarAimbotSpeedMin, trackBarAimbotSpeedMax, trackBarAimbotSpeedStep, "aimbotSpeed");
            aimbotSpeed.ValueChanged += trackBar_ValueChanged;
            aimbotTarget = new ValueMenuItem(themeBasic, fontSmall, aimMenu, "Target", switchTargets, "aimbotTarget");
            aimbotTarget.OptionChanged += targetValChanged;
            aimSpottedOnly = new ValueMenuItem(themeBasic, fontSmall, aimMenu, "Only aim at spotted players", switchYesNo, "aimSpottedOnly");
            aimSpottedOnly.OptionChanged += yesNoValChanged;

            crosshairDrawData = new ValueMenuItem(themeBasic, fontSmall, crosshairMenu, "Draw target-data", switchOnOff, "crosshairDrawData");
            crosshairDrawData.OptionChanged += onOffValChanged;
            crosshairDrawRecoil = new ValueMenuItem(themeBasic, fontSmall, crosshairMenu, "Draw recoil", switchYesNo, "crosshairDrawRecoil");
            crosshairDrawRecoil.OptionChanged += onOffValChanged;
            crosshairDrawSoundESP = new ValueMenuItem(themeBasic, fontSmall, crosshairMenu, "Draw SoundESP", switchYesNo, "crosshairDrawSoundESP");
            crosshairDrawSoundESP.OptionChanged += yesNoValChanged;
            crosshairEnabled = new ValueMenuItem(themeBasic, fontSmall, crosshairMenu, "Enabled", switchYesNo, "crosshairEnabled");
            crosshairEnabled.OptionChanged += yesNoValChanged;

            espDrawBox = new ValueMenuItem(themeBasic, fontSmall, espMenu, "Draw box", switchOnOff, "espDrawBox");
            espDrawBox.OptionChanged += onOffValChanged;
            espDrawCircle = new ValueMenuItem(themeBasic, fontSmall, espMenu, "Draw circle", switchOnOff, "espDrawCircle");
            espDrawCircle.OptionChanged += onOffValChanged;
            espDrawDetails = new ValueMenuItem(themeBasic, fontSmall, espMenu, "Draw stats", switchOnOff, "espDrawDetails");
            espDrawDetails.OptionChanged += onOffValChanged;
            espDrawDistance = new ValueMenuItem(themeBasic, fontSmall, espMenu, "Draw distance", switchOnOff, "espDrawDistance");
            espDrawDistance.OptionChanged += onOffValChanged;
            espDrawHealth = new ValueMenuItem(themeBasic, fontSmall, espMenu, "Draw health", switchOnOff, "espDrawHealth");
            espDrawHealth.OptionChanged += onOffValChanged;
            espDrawLines = new ValueMenuItem(themeBasic, fontSmall, espMenu, "Draw lines", switchOnOff, "espDrawLines");
            espDrawLines.OptionChanged += onOffValChanged;
            espDrawName = new ValueMenuItem(themeBasic, fontSmall, espMenu, "Draw name", switchOnOff, "espDrawName");
            espDrawName.OptionChanged += onOffValChanged;
            espDrawSkeleton = new ValueMenuItem(themeBasic, fontSmall, espMenu, "Draw skeleton", switchOnOff, "espDrawSkeleton");
            espDrawSkeleton.OptionChanged += onOffValChanged;
            espDrawWeapons = new ValueMenuItem(themeBasic, fontSmall, espMenu, "Draw weapons", switchOnOff, "espDrawWeapons");
            espDrawWeapons.OptionChanged += onOffValChanged;
            espEnabled = new ValueMenuItem(themeBasic, fontSmall, espMenu, "Enabled", switchYesNo, "espEnabled");
            espEnabled.OptionChanged += yesNoValChanged;
            espTarget = new ValueMenuItem(themeBasic, fontSmall, espMenu, "Draw", switchTargets, "espDrawTarget");
            espTarget.OptionChanged += targetValChanged;

            espGlowEnabled = new ValueMenuItem(themeBasic, fontSmall, espGlowMenu, "Enabled", switchYesNo, "espGlowEnabled");
            espGlowEnabled.OptionChanged += yesNoValChanged;
            espGlowFadingEnabled = new ValueMenuItem(themeBasic, fontSmall, espGlowMenu, "Fading enabled", switchYesNo, "espGlowFadingEnabled");
            espGlowFadingEnabled.OptionChanged += yesNoValChanged;
            espGlowFadingDistance = new TrackbarMenuItem(themeBasic, fontSmall, espGlowMenu, "Fading-distance", 0f, 500f, 5f, 50f, "espGlowFadingDistance");
            espGlowFadingDistance.ValueChanged += trackBar_ValueChanged;

            miscAutoPistolEnabled = new ValueMenuItem(themeBasic, fontSmall, miscMenu, "Auto-pistol enabled", switchYesNo, "miscAutoPistolEnabled");
            miscAutoPistolEnabled.OptionChanged += yesNoValChanged;
            miscBunnyHopEnabled = new ValueMenuItem(themeBasic, fontSmall, miscMenu, "Bunnyhop enabled", switchYesNo, "miscBunnyHopEnabled");
            miscBunnyHopEnabled.OptionChanged += yesNoValChanged;
            miscInfoEnabled = new ValueMenuItem(themeBasic, fontSmall, miscMenu, "Information-panel enabled", switchYesNo, "miscInfoEnabled");
            miscInfoEnabled.OptionChanged += yesNoValChanged;
            miscNoFlash = new ValueMenuItem(themeBasic, fontSmall, miscMenu, "NoFlash enabled", switchYesNo, "miscNoFlash");
            miscNoFlash.OptionChanged += yesNoValChanged;
            miscNoSmoke = new ValueMenuItem(themeBasic, fontSmall, miscMenu, "NoSmoke enabled", switchYesNo, "miscNoSmoke");
            miscNoSmoke.OptionChanged += yesNoValChanged;

            radarDrawLines = new ValueMenuItem(themeBasic, fontSmall, radarMenu, "Draw lines", switchOnOff, "radarDrawLines");
            radarDrawLines.OptionChanged += onOffValChanged;
            radarDrawView = new ValueMenuItem(themeBasic, fontSmall, radarMenu, "Draw view", switchOnOff, "radarDrawView");
            radarDrawView.OptionChanged += onOffValChanged;
            radarEnabled = new ValueMenuItem(themeBasic, fontSmall, radarMenu, "Enabled", switchYesNo, "radarEnabled");
            radarEnabled.OptionChanged += yesNoValChanged;
            radarTarget = new ValueMenuItem(themeBasic, fontSmall, radarMenu, "Draw", switchTargets, "radarDrawTarget");
            radarTarget.OptionChanged += targetValChanged;
            radarZoom = new TrackbarMenuItem(themeBasic, fontSmall, radarMenu, "Zoom-level", trackBarRadarDistMin, trackBarRadarDistMax, trackBarRadarDistStep, "radarZoom");
            radarZoom.ValueChanged += trackBar_ValueChanged;

            rcsEnabled = new ValueMenuItem(themeBasic, fontSmall, rcsMenu, "Recoil-Control enabled", switchYesNo, "rcsEnabled");
            rcsEnabled.OptionChanged += yesNoValChanged;
            rcsForce = new TrackbarMenuItem(themeBasic, fontSmall, rcsMenu, "Force", 0f, 3f, 0.1f, 1f, "rcsForce");
            rcsForce.ValueChanged += trackBar_ValueChanged;

            soundEspEnabled = new ValueMenuItem(themeBasic, fontSmall, soundEspMenu, "SoundESP enabled", switchYesNo, "soundEspEnabled");
            soundEspEnabled.OptionChanged += yesNoValChanged;
            soundEspInterval = new TrackbarMenuItem(themeBasic, fontSmall, soundEspMenu, "Interval (ms)", 250, 5000, 50, 1000, "soundEspInterval");
            soundEspInterval.ValueChanged += trackBar_ValueChanged;
            soundEspRange = new TrackbarMenuItem(themeBasic, fontSmall, soundEspMenu, "Range", 10, 500, 10, 50, "soundEspRange");
            soundEspRange.ValueChanged += trackBar_ValueChanged;
            soundEspSound = new TrackbarMenuItem(themeBasic, fontSmall, soundEspMenu, "Sound", 1, 10, 1, 1, "soundEspSound");
            soundEspSound.ValueChanged += trackBar_ValueChanged;
            soundEspVolume = new TrackbarMenuItem(themeBasic, fontSmall, soundEspMenu, "Volume (%)", 0, 100, 5, 5, "soundEspVolume");
            soundEspVolume.ValueChanged += trackBar_ValueChanged;

            spectatorDisableAim = new ValueMenuItem(themeBasic, fontSmall, radarMenu, "Disable aimbot", switchYesNo, "spectatorDisableAim");
            spectatorDisableAim.OptionChanged += yesNoValChanged;
            spectatorDisableTrigger = new ValueMenuItem(themeBasic, fontSmall, radarMenu, "Disable triggerbot", switchYesNo, "spectatorDisableTrigger");
            spectatorDisableTrigger.OptionChanged += yesNoValChanged;
            spectatorDrawWarning = new ValueMenuItem(themeBasic, fontSmall, radarMenu, "Draw warning", switchYesNo, "spectatorDrawWarning");
            spectatorDrawWarning.OptionChanged += yesNoValChanged;

            triggerbotDelay = new TrackbarMenuItem(themeBasic, fontSmall, triggerMenu, "Delay (ms)", trackBarTriggerDelayMin, trackBarTriggerDelayMax, trackBarTriggerDelayStep, "triggerbotDelay");
            triggerbotDelay.ValueChanged += trackBar_ValueChanged;
            triggerbotEnabled = new ValueMenuItem(themeBasic, fontSmall, triggerMenu, "Enabled", switchYesNo, "triggerbotEnabled");
            triggerbotEnabled.OptionChanged += yesNoValChanged;
            triggerbotRecoilThreshold = new TrackbarMenuItem(themeBasic, fontSmall, triggerMenu, "max Recoil (%)", trackBarTriggerRecoilTHMin, trackBarTriggerRecoilTHMax, trackBarTriggerRecoilTHStep, 2.75f, "triggerbotRecoilThreshold");
            triggerbotRecoilThreshold.ValueChanged += trackBar_ValueChanged;
            triggerbotSnipersOnly = new ValueMenuItem(themeBasic, fontSmall, triggerMenu, "Sniper-rifles only", switchYesNo, "triggerbotSnipersOnly");
            triggerbotSnipersOnly.OptionChanged += yesNoValChanged;
            triggerbotSpeedThreshold = new TrackbarMenuItem(themeBasic, fontSmall, triggerMenu, "max speed (km/h)", trackBarTriggerSpeedTHMin, trackBarTriggerSpeedTHMax, trackBarTriggerSpeedTHStep, 5f, "triggerbotSpeedThreshold");
            triggerbotSpeedThreshold.ValueChanged += trackBar_ValueChanged;
            triggerbotTarget = new ValueMenuItem(themeBasic, fontSmall, triggerMenu, "Target", switchTargets, "triggerbotTarget");
            triggerbotTarget.OptionChanged += targetValChanged;
            triggerKey = new KeyMenuItem(themeBasic, fontSmall, aimMenu, "Key", System.Windows.Forms.Keys.LButton, "triggerKey");
            triggerKey.OptionChanged += keyValChanged;
            
            SubMenuItem aimSubMenu = new SubMenuItem(themeBasic, fontSmall, mainMenu, "Aimbot", aimMenu);
            SubMenuItem crosshairSubMenu = new SubMenuItem(themeBasic, fontSmall, mainMenu, "Crosshair", crosshairMenu);
            SubMenuItem espSubMenu = new SubMenuItem(themeBasic, fontSmall, mainMenu, "ESP", espMenu);
            SubMenuItem espGlowSubMenu = new SubMenuItem(themeBasic, fontSmall, espMenu, "Outlineglow", espGlowMenu);
            SubMenuItem highlightPlayerSubMenu = new SubMenuItem(themeBasic, fontSmall, mainMenu, "Hightlight players", highlightPlayerMenu);
            SubMenuItem radarSubMenu = new SubMenuItem(themeBasic, fontSmall, mainMenu, "Radar", radarMenu);
            SubMenuItem rcsSubMenu = new SubMenuItem(themeBasic, fontSmall, mainMenu, "Recoil-Control", rcsMenu);
            SubMenuItem settingsSubMenu = new SubMenuItem(themeBasic, fontSmall, mainMenu, "Settings", settingsMenu);
            SubMenuItem soundEspSubMenu = new SubMenuItem(themeBasic, fontSmall, mainMenu, "SoundESP", soundEspMenu);
            SubMenuItem spectatorSubMenu = new SubMenuItem(themeBasic, fontSmall, mainMenu, "Spectator", spectatorMenu);
            SubMenuItem statsSubMenu = new SubMenuItem(themeBasic, fontSmall, mainMenu, "Statistics", statsMenu);
            SubMenuItem triggerSubMenu = new SubMenuItem(themeBasic, fontSmall, mainMenu, "Triggerbot", triggerMenu);
            SubMenuItem uiSubMenu = new SubMenuItem(themeBasic, fontSmall, mainMenu, "Misc", miscMenu);
            SubMenuItem wireframeSubMenu = new SubMenuItem(themeBasic, fontSmall, mainMenu, "Wireframe", wireframeMenu);

            string[] files = Directory.GetFiles(Environment.CurrentDirectory, "*.csgo.cfg");
            settingsLoadButtons = new LoadActionMenuItem[files.Length];
            settingsSaveButtons = new SaveActionMenuItem[files.Length];
            settingsFilesMenus = new Menu[files.Length];
            SubMenuItem[] settingsFilesSubMenus = new SubMenuItem[files.Length];

            for (int i = 0; i < files.Length; i++ )
            {
                string fileName = new FileInfo(files[i]).Name;
                settingsFilesMenus[i] = new Menu(themeBasic, themeItemSelected, themeBasic, fontSmall, fontLarge, 0, 0, 256, 256, String.Format("[{0}]", fileName), 4f, 4f, 4f);
                settingsLoadButtons[i] = new LoadActionMenuItem(themeBasic, fontSmall, settingsFilesMenus[i], String.Format("Load \"{0}\"", fileName), fileName);
                settingsSaveButtons[i] = new SaveActionMenuItem(themeBasic, fontSmall, settingsFilesMenus[i], String.Format("Save \"{0}\"", fileName), fileName);
                settingsFilesSubMenus[i] = new SubMenuItem(themeBasic, fontSmall, settingsMenu, fileName, settingsFilesMenus[i]);
            }

            highlightPlayers = new ValueMenuItem[64];
            for (int i = 0; i < highlightPlayers.Length; i++)
            {
                highlightPlayers[i] = new ValueMenuItem(themeBasic, fontSmall, highlightPlayerMenu, "Player", switchOnOff, 1);
                highlightPlayers[i].Visible = false;
                highlightPlayers[i].Text = "";
                highlightPlayers[i].OptionChanged += highlightedValChanged;
            }

            aimBack = new BackMenuItem(themeBasic, fontSmall, aimMenu);
            triggerBack = new BackMenuItem(themeBasic, fontSmall, triggerMenu);
            radarBack = new BackMenuItem(themeBasic, fontSmall, radarMenu);
            espBack = new BackMenuItem(themeBasic, fontSmall, espMenu);
            espGlowBack = new BackMenuItem(themeBasic, fontSmall, espGlowMenu);
            crosshairBack = new BackMenuItem(themeBasic, fontSmall, crosshairMenu);
            spectatorBack = new BackMenuItem(themeBasic, fontSmall, spectatorMenu);
            settingsBack = new BackMenuItem(themeBasic, fontSmall, settingsMenu);
            statsBack = new BackMenuItem(themeBasic, fontSmall, statsMenu);
            miscBack = new BackMenuItem(themeBasic, fontSmall, miscMenu);
            rcsBack = new BackMenuItem(themeBasic, fontSmall, rcsMenu);
            highlightPlayersBack = new BackMenuItem(themeBasic, fontSmall, highlightPlayerMenu);
            soundEspBack = new BackMenuItem(themeBasic, fontSmall, soundEspMenu);
            #endregion

            #region MENU ITEMS - ADD
            //Add menuitems
            menuItems = new List<MenuItem>();
            
            menuItems.Add(aimSubMenu);
            menuItems.Add(crosshairSubMenu);
            menuItems.Add(espSubMenu);
            menuItems.Add(espGlowSubMenu);
            menuItems.Add(highlightPlayerSubMenu);
            menuItems.Add(radarSubMenu);
            menuItems.Add(settingsSubMenu);
            menuItems.Add(soundEspSubMenu);
            menuItems.Add(spectatorSubMenu);
            menuItems.Add(statsSubMenu);
            menuItems.Add(uiSubMenu);
            menuItems.Add(rcsSubMenu);
            menuItems.Add(triggerSubMenu);
            menuItems.Add(wireframeSubMenu);

            menuItems.Add(radarBack);
            menuItems.Add(radarDrawLines);
            menuItems.Add(radarDrawView);
            menuItems.Add(radarEnabled);
            menuItems.Add(radarTarget);
            menuItems.Add(radarZoom);

            menuItems.Add(espBack);
            menuItems.Add(espDrawBox);
            //menuItems.Add(espDrawChickens);
            menuItems.Add(espDrawCircle);
            menuItems.Add(espDrawDetails);
            menuItems.Add(espDrawDistance);
            menuItems.Add(espDrawHealth);
            menuItems.Add(espDrawLines);
            menuItems.Add(espDrawName);
            menuItems.Add(espDrawSkeleton);
            menuItems.Add(espDrawWeapons);
            menuItems.Add(espEnabled);
            menuItems.Add(espTarget);

            menuItems.Add(espGlowBack);
            menuItems.Add(espGlowEnabled);
            menuItems.Add(espGlowFadingEnabled);
            menuItems.Add(espGlowFadingDistance);

            menuItems.Add(crosshairBack);
            menuItems.Add(crosshairEnabled);
            menuItems.Add(crosshairDrawData);
            menuItems.Add(crosshairDrawRecoil);
            menuItems.Add(crosshairDrawSoundESP);

            menuItems.Add(aimBack);
            menuItems.Add(aimAllowAimJump);
            menuItems.Add(aimbotBone);
            menuItems.Add(aimbotEnabled);
            menuItems.Add(aimbotCompensateRecoil);
            menuItems.Add(aimbotRagemode);
            menuItems.Add(aimbotKey);
            menuItems.Add(aimbotMethod);
            menuItems.Add(aimbotRadius);
            menuItems.Add(aimbotSmooth);
            menuItems.Add(aimbotSpeed);
            menuItems.Add(aimbotTarget);
            menuItems.Add(aimSpottedOnly);

            menuItems.Add(triggerBack);
            menuItems.Add(triggerbotDelay);
            menuItems.Add(triggerbotEnabled);
            menuItems.Add(triggerbotSnipersOnly);
            menuItems.Add(triggerbotRecoilThreshold);
            menuItems.Add(triggerbotSpeedThreshold);
            menuItems.Add(triggerbotTarget);
            menuItems.Add(triggerKey);

            menuItems.Add(spectatorBack);
            menuItems.Add(spectatorDrawWarning);
            menuItems.Add(spectatorDisableAim);
            menuItems.Add(spectatorDisableTrigger);

            menuItems.Add(statsBack);

            menuItems.Add(miscBack);
            menuItems.Add(miscInfoEnabled);
            menuItems.Add(miscBunnyHopEnabled);
            menuItems.Add(miscAutoPistolEnabled);
            menuItems.Add(miscNoFlash);
            menuItems.Add(miscNoSmoke);

            menuItems.Add(rcsBack);
            menuItems.Add(rcsEnabled);
            menuItems.Add(rcsForce);

            menuItems.Add(soundEspBack);
            menuItems.Add(soundEspEnabled);
            menuItems.Add(soundEspRange);
            menuItems.Add(soundEspInterval);
            menuItems.Add(soundEspSound);
            menuItems.Add(soundEspVolume);

            foreach (LoadActionMenuItem item in settingsLoadButtons)
                menuItems.Add(item);
            foreach (SaveActionMenuItem item in settingsSaveButtons)
                menuItems.Add(item);
            foreach (SubMenuItem item in settingsFilesSubMenus)
                menuItems.Add(item);
            menuItems.Add(settingsBack);

            foreach (ValueMenuItem highlight in highlightPlayers)
                menuItems.Add(highlight);
            menuItems.Add(highlightPlayersBack);

            foreach (MenuItem menuItem in menuItems)
            {
                menuItem.Width = mainMenu.Width - mainMenu.PaddingX * 2f;
                menuItem.Height = 20f;
                //menuItem.Theme = themeBasic;
            }


            highlightPlayerMenu.AddChildControl(highlightPlayersBack);
            foreach (ValueMenuItem highlight in highlightPlayers)
                highlightPlayerMenu.AddChildControl(highlight);

            settingsMenu.AddChildControl(settingsBack);
            for (int i = 0; i < settingsFilesMenus.Length; i++ )
            {
                settingsMenu.AddChildControl(settingsFilesSubMenus[i]);
                settingsFilesMenus[i].AddChildControl(settingsLoadButtons[i]);
                settingsFilesMenus[i].AddChildControl(settingsSaveButtons[i]);
            }

            rcsMenu.AddChildControl(rcsBack);
            rcsMenu.AddChildControl(rcsEnabled);
            rcsMenu.AddChildControl(rcsForce);

            miscMenu.AddChildControl(miscBack);
            miscMenu.AddChildControl(miscInfoEnabled);
            miscMenu.AddChildControl(miscAutoPistolEnabled);
            miscMenu.AddChildControl(miscBunnyHopEnabled);
            miscMenu.AddChildControl(miscNoFlash);
            //miscMenu.AddChildControl(miscNoSmoke);

            statsMenu.AddChildControl(statsBack);
            statsMenu.AddChildControl(ctrlStatsKills);
            statsMenu.AddChildControl(ctrlStatsDeaths);
            statsMenu.AddChildControl(ctrlStatsKD);
            statsMenu.AddChildControl(ctrlStatsScore);

            spectatorMenu.AddChildControl(spectatorBack);
            spectatorMenu.AddChildControl(spectatorDrawWarning);
            spectatorMenu.AddChildControl(spectatorDisableAim);
            spectatorMenu.AddChildControl(spectatorDisableTrigger);

            radarMenu.AddChildControl(radarBack);
            radarMenu.AddChildControl(radarEnabled);
            radarMenu.AddChildControl(radarTarget);
            radarMenu.AddChildControl(radarDrawLines);
            radarMenu.AddChildControl(radarZoom);

            espMenu.AddChildControl(espBack);
            espMenu.AddChildControl(espEnabled);
            espMenu.AddChildControl(espGlowSubMenu);
            espMenu.AddChildControl(espTarget);
            espMenu.AddChildControl(espDrawName);
            espMenu.AddChildControl(espDrawHealth);
            espMenu.AddChildControl(espDrawDetails);
            espMenu.AddChildControl(espDrawDistance);
            espMenu.AddChildControl(espDrawBox);
            espMenu.AddChildControl(espDrawCircle);
            espMenu.AddChildControl(espDrawLines);
            espMenu.AddChildControl(espDrawSkeleton);
            espMenu.AddChildControl(espDrawWeapons);

            espGlowMenu.AddChildControl(espGlowBack);
            espGlowMenu.AddChildControl(espGlowEnabled);
            espGlowMenu.AddChildControl(espGlowFadingEnabled);
            espGlowMenu.AddChildControl(espGlowFadingDistance);

            crosshairMenu.AddChildControl(crosshairBack);
            crosshairMenu.AddChildControl(crosshairEnabled);
            crosshairMenu.AddChildControl(crosshairDrawData);
            crosshairMenu.AddChildControl(crosshairDrawRecoil);
            crosshairMenu.AddChildControl(crosshairDrawSoundESP);

            aimMenu.AddChildControl(aimBack);
            aimMenu.AddChildControl(aimbotEnabled);
            aimMenu.AddChildControl(aimbotRagemode);
            aimMenu.AddChildControl(aimbotSmooth);
            aimMenu.AddChildControl(aimbotCompensateRecoil);
            aimMenu.AddChildControl(aimbotTarget);
            aimMenu.AddChildControl(aimbotBone);
            aimMenu.AddChildControl(aimbotMethod);
            aimMenu.AddChildControl(aimbotRadius);
            aimMenu.AddChildControl(aimbotSpeed);
            aimMenu.AddChildControl(aimbotKey);
            aimMenu.AddChildControl(aimAllowAimJump);
            aimMenu.AddChildControl(aimSpottedOnly);

            triggerMenu.AddChildControl(triggerBack);
            triggerMenu.AddChildControl(triggerbotEnabled);
            triggerMenu.AddChildControl(triggerbotTarget);
            triggerMenu.AddChildControl(triggerbotSnipersOnly);
            triggerMenu.AddChildControl(triggerbotDelay);
            triggerMenu.AddChildControl(triggerbotRecoilThreshold);
            triggerMenu.AddChildControl(triggerbotSpeedThreshold);
            triggerMenu.AddChildControl(triggerKey);

            mainMenu.AddChildControl(aimSubMenu);
            mainMenu.AddChildControl(triggerSubMenu);
            mainMenu.AddChildControl(radarSubMenu);
            mainMenu.AddChildControl(espSubMenu);
            mainMenu.AddChildControl(soundEspSubMenu);
            //mainMenu.AddChildControl(wireframeSubMenu);
            mainMenu.AddChildControl(crosshairSubMenu);
            mainMenu.AddChildControl(spectatorSubMenu);
            mainMenu.AddChildControl(highlightPlayerSubMenu);
            mainMenu.AddChildControl(uiSubMenu);
            mainMenu.AddChildControl(rcsSubMenu);
            mainMenu.AddChildControl(statsSubMenu);
            mainMenu.AddChildControl(settingsSubMenu);

            soundEspMenu.AddChildControl(soundEspBack);
            soundEspMenu.AddChildControl(soundEspEnabled);
            soundEspMenu.AddChildControl(soundEspRange);
            soundEspMenu.AddChildControl(soundEspInterval);
            soundEspMenu.AddChildControl(soundEspSound);
            soundEspMenu.AddChildControl(soundEspVolume);
            #endregion
        }

        #region EVENTS
        void trackBar_ValueChanged(object sender, Objects.UI.Events.ValueChangedEventArgs e)
        {
            Program.GameImplementation.SetValue(((TrackbarMenuItem)sender).ExtraData, e.Value);
        }
        void yesNoValChanged(object sender, Objects.UI.Events.OptionChangedEventArgs e)
        {
            Program.GameImplementation.SetValue(((ValueMenuItem)sender).ExtraData, (YesNo)e.OptionValue);
        }
        void onOffValChanged(object sender, Objects.UI.Events.OptionChangedEventArgs e)
        {
            Program.GameImplementation.SetValue(((ValueMenuItem)sender).ExtraData, (OnOff)e.OptionValue);
        }

        void targetValChanged(object sender, Objects.UI.Events.OptionChangedEventArgs e)
        {
            Program.GameImplementation.SetValue(((ValueMenuItem)sender).ExtraData, (Target)e.OptionValue);
        }

        void floatValChanged(object sender, Objects.UI.Events.OptionChangedEventArgs e)
        {
            Program.GameImplementation.SetValue(((ValueMenuItem)sender).ExtraData, (float)e.OptionValue);
        }

        void aimMethodValChanged(object sender, Objects.UI.Events.OptionChangedEventArgs e)
        {
            Program.GameImplementation.SetValue(((ValueMenuItem)sender).ExtraData, (AimMethod)e.OptionValue);
        }
        void aimBoneValChanged(object sender, Objects.UI.Events.OptionChangedEventArgs e)
        {
            Program.GameImplementation.SetValue(((ValueMenuItem)sender).ExtraData, (AimBone)e.OptionValue);
        }
        void keyValChanged(object sender, Objects.UI.Events.OptionChangedEventArgs e)
        {
            Program.GameImplementation.SetValue(((KeyMenuItem)sender).ExtraData, (System.Windows.Forms.Keys)e.OptionValue);
        }
        void highlightedValChanged(object sender, Objects.UI.Events.OptionChangedEventArgs e)
        {
            CSGOImplementation csgo = (CSGOImplementation)Program.GameImplementation;
            for (int i = 0; i < csgo.Players.Length; i++)
            {
                if (csgo.Players[i] == null)
                    continue;
                if (csgo.Players[i].Name == ((ValueMenuItem)sender).Text)
                {
                    csgo.Highlighted[i] = (OnOff)e.OptionValue == OnOff.On;
                    break;
                }
            }
        }
        #endregion

        protected override void OnDestroyDevice()
        {
            throw new NotImplementedException();
        }

        protected override void OnTick()
        {
            if (Program.GameImplementation == null)
                return;
            if (Program.GameController == null)
                return;
            if (!Program.GameController.IsGameRunning)
                return;
            CSGOImplementation csgo = (CSGOImplementation)Program.GameImplementation;
            if(once)
            {
                once = !once;
                #region Load current values
                //foreach (MenuItem item in menuItems)
                //{
                //    if (csgo.HasKey(item.ExtraData))
                //    {
                //        object val = csgo.GetValue(item.ExtraData);
                //        if (item.ExtraData == "aimbotKey")
                //            Program.PrintError("Aimbot-key: {0}, currently: {1}", val.ToString(), ((KeyMenuItem)item).Key.ToString());
                //        item.SwitchToVal(val);
                //    }
                //}
                #endregion
            }
            #region keys
            if ((DateTime.Now.Ticks - lastTick) / TimeSpan.TicksPerMillisecond >= 100)
            {
                #region realign ui
                mainMenu.SetPosition(mainMenu.X, Program.GameController.Form.ClientSize.Height / 2f - mainMenu.Height / 2f);
                ctrlRadar.SetPosition(Program.GameController.Form.ClientSize.Width - ctrlRadar.Width - 4, Program.GameController.Form.ClientSize.Height / 2f - ctrlRadar.Height / 2f);
                ctrlPlayerInformation.SetPosition(Program.GameController.Form.ClientSize.Width - ctrlPlayerInformation.Width, 4f);
                #endregion
                if (Program.GameImplementation.GetValue<YesNo>("menuEnabled") == YesNo.Yes)
                {
                    foreach (System.Windows.Forms.Keys key in Program.GameController.InputUpdater.KeysThatAreDown())
                        mainMenu.OnKeyUp(key);
                    lastTick = DateTime.Now.Ticks;
                }

                if (Program.GameController.InputUpdater.KeyIsDown(System.Windows.Forms.Keys.F9))
                {
                    if (Program.GameImplementation.GetValue<YesNo>("menuEnabled") == YesNo.Yes)
                    {
                        Program.GameImplementation.SetValue("menuEnabled", YesNo.No);
                        for (int i = mainMenu.ChildControls.Count - 1; i > 0; i--)
                            mainMenu.ChildControls[i].Visible = false;
                    }
                    else
                    {
                        Program.GameImplementation.SetValue("menuEnabled", YesNo.Yes);
                        for (int i = mainMenu.ChildControls.Count - 1; i > 0; i--)
                            mainMenu.ChildControls[i].Visible = true;
                    }
                    lastTick = DateTime.Now.Ticks;
                }
                
                if (Program.GameController.InputUpdater.KeyIsDown(Program.GameImplementation.GetValue<System.Windows.Forms.Keys>("triggerKey")))
                {
                    if (Program.GameImplementation.GetValue<YesNo>("triggerbotEnabled") == YesNo.Yes)
                        Program.GameImplementation.SetValue("triggerbotEnabled", YesNo.No);
                    else
                        Program.GameImplementation.SetValue("triggerbotEnabled", YesNo.Yes);
                }
                //if (Program.GameController.InputUpdater.KeyIsDown(System.Windows.Forms.Keys.F7))
                //    ((CSGOGameController)Program.GameController).MemoryUpdater.SaveIDS();
            }
            #endregion
            #region highlight
            if (csgo.Players != null)
            {
                bool visible;
                for (int i = 0; i < 64; i++)
                {
                    try
                    {
                        visible = highlightPlayers[i].Visible;
                        if (csgo.Players[i] != null)
                        {
                            highlightPlayers[i].Visible = true;
                            highlightPlayers[i].Text = csgo.Players[i].Name;
                        }
                        else
                        {
                            highlightPlayers[i].Visible = false;
                        }
                        if (highlightPlayers[i].Visible != visible)
                            highlightPlayerMenu.InvokeAutoSize();
                    }
                    catch { }
                }
            }
            #endregion
        }

        protected override void OnDraw(SharpDX.Direct2D1.WindowRenderTarget device)
        {
            if (Program.GameImplementation.GetValue<YesNo>("menuEnabled") == YesNo.Yes)
                mainMenu.Draw(device);
            if (Program.GameImplementation.GetValue<YesNo>("radarEnabled") == YesNo.Yes)
                ctrlRadar.Draw(device);
            if (Program.GameImplementation.GetValue<YesNo>("espEnabled") == YesNo.Yes)
                ctrlEsp.Draw(device);
            if (Program.GameImplementation.GetValue<YesNo>("crosshairEnabled") == YesNo.Yes)
                ctrlCrosshair.Draw(device);
            if (Program.GameImplementation.GetValue<YesNo>("miscInfoEnabled") == YesNo.Yes)
                ctrlPlayerInformation.Draw(device);
        }
    }
}
