using laExternalMulti.Objects.Implementation.CSGO.Data;
using laExternalMulti.Objects.Implementation.CSGO.Data.BSP;
using laExternalMulti.Objects.Implementation.CSGO.Data.Enums;
using laExternalMulti.Objects.Implementation.CSGO.Data.WeaponData;
using SharpDX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO
{
    public class CSGOImplementation : GameImplementation
    {
        #region VARIABLES
        private bool c4Planted;
        private bool firstPersonSpec;
        private bool isReloading;
        private bool isShooting;
        private bool[] highlighted;
        private BSP currentMap;
        private Entity[] entities;
        private float accuracyPenality;
        private float c4Timer;
        private float mapTime;
        private int shots;
        private int targetIndex;
        private int weaponClip1, weaponClip2;
        private int weaponFireRate;
        private int weaponShotsFired;
        private int winsCT;
        private int winsT;
        private int[] scrbrdKills, scrbrdDeaths, scrbrdAssists, scrbrdScore, scrbrdArmor, scrbrdHealth, scrbrdRanks, scrbrdWins;
        private List<Damage> damages = new List<Damage>();
        private List<Player> spectators = new List<Player>();
        private long c4PlantTime;
        private Matrix4x4 viewMatrix;
        private object debugData;
        private Player localPlayer;
        private Player targetPlayer;
        private Player[] players;
        private Size2 screenSize;
        private string serverMap = "";
        private string serverIP = "";
        private string serverName = "";
        private string weaponName;
        private WeaponType weaponType;
        private ZoomLevel zoomLevel;
        private SignOnState signOnState;
        private Vector3 viewAngles;
        private Vector3 viewOffset;
        private GlowObjectDefinition[] glowObjects;
        #endregion

        #region PROPERTIES
        public Player[] Players { get { return players; } set { players = value; } }
        public bool[] Highlighted { get { return highlighted; } set { highlighted = value; } }
        public Entity[] Entities { get { return entities; } set { entities = value; } }
        public Player LocalPlayer { get { return localPlayer; } set { localPlayer = value; } }
        public Player TargetPlayer { get { return targetPlayer; } set { targetPlayer = value; } }
        public Matrix4x4 ViewMatrix { get { return viewMatrix; } set { viewMatrix = value; } }
        public Size2 ScreenSize { get { return screenSize; } set { screenSize = value; } }
        public object DebugData { get { return debugData; } set { debugData = value; } }
        public string WeaponName { get { return weaponName; } set { weaponName = value; } }
        public int WeaponFireRate { get { return weaponFireRate; } set { weaponFireRate = value; } }
        public int WeaponClip1 { get { return weaponClip1; } set { weaponClip1 = value; } }
        public int WeaponClip2 { get { return weaponClip2; } set { weaponClip2 = value; } }
        public int WeaponShotsFired { get { return weaponShotsFired; } set { weaponShotsFired = value; } }
        public int TargetIndex { get { return targetIndex; } set { targetIndex = value; } }
        public WeaponType WeaponType { get { return weaponType; } set { weaponType = value; } }
        public List<Player> Spectators { get { return spectators; } set { spectators = value; } }
        public List<Damage> Damages { get { return damages; } set { damages = value; } }
        public bool FirstPersonSpectator { get { return firstPersonSpec; } set { firstPersonSpec = value; } }
        public bool IsShooting { get { return isShooting; } set { isShooting = value; } }
        public float AccuracyPenality { get { return accuracyPenality; } set { accuracyPenality = value; } }
        public float MapTime { get { return mapTime; } set { mapTime = value; } }
        public int[] ScrBrdKills { get { return scrbrdKills; } set { scrbrdKills = value; } }
        public int[] ScrBrdDeaths { get { return scrbrdDeaths; } set { scrbrdDeaths = value; } }
        public int[] ScrBrdAssists { get { return scrbrdAssists; } set { scrbrdAssists = value; } }
        public int[] ScrBrdScore { get { return scrbrdScore; } set { scrbrdScore = value; } }
        public int[] ScrBrdArmor { get { return scrbrdArmor; } set { scrbrdArmor = value; } }
        public int[] ScrBrdHealth { get { return scrbrdHealth; } set { scrbrdHealth = value; } }
        public int[] ScrBrdRanks { get { return scrbrdRanks; } set { scrbrdRanks = value; } }
        public int[] ScrBrdWins { get { return scrbrdWins; } set { scrbrdWins = value; } }
        public bool IsReloading { get { return isReloading; } set { isReloading = value; } }
        public ZoomLevel ZoomLevel { get { return zoomLevel; } set { zoomLevel = value; } }
        public bool C4Planted { get { return c4Planted; } set { c4Planted = value; } }
        public long C4PlantTime { get { return c4PlantTime; } set { c4PlantTime = value; } }
        public float C4Timer { get { return c4Timer; } set { c4Timer = value; } }
        public string ServerMap
        {
            get { return serverMap; }
            set
            {
                if (serverMap != value)
                {
                    serverMap = value;
                    //if (GetValue<YesNo>("wireframeEnabled") == YesNo.No)
                    //    return;
                    //LoadMap();
                }
            }
        }
        public string ServerIP { get { return serverIP; } set { serverIP = value; } }
        public string ServerName { get { return serverName; } set { serverName = value; } }
        public BSP CurrentMap { get { return currentMap; } }
        public int ShotsFired { get { return shots; } set { shots = value; } }
        public int WinsCT { get { return winsCT; } set { winsCT = value; } }
        public int WinsT { get { return winsT; } set { winsT = value; } }
        public SignOnState SignOnState { get { return signOnState; } set { signOnState = value; } }
        public Vector3 ViewAngles { get { return viewAngles; } set { viewAngles = value; } }
        public Vector3 ViewOffset { get { return viewOffset; } set { viewOffset = value; } }
        public GlowObjectDefinition[] GlowObjects { get { return glowObjects; } set { glowObjects = value; } }
        public float FlashMaxDuration { get; set; }
        public float FlashMaxAlpha { get; set; }
        #endregion
        public CSGOImplementation()
            : base()
        {
            viewAngles = Vector3.Zero;
            viewOffset = Vector3.Zero;
            #region SETTINGS
            this.settingsStorage.Add("aimAllowAimJump", YesNo.Yes);
            this.settingsStorage.Add("aimbotBone", AimBone.Head);
            this.settingsStorage.Add("aimbotEnabled", YesNo.Yes);
            this.settingsStorage.Add("aimbotCompensateRecoil", OnOff.Off);
            this.settingsStorage.Add("aimbotRagemode", OnOff.Off);
            this.settingsStorage.Add("aimbotKey", System.Windows.Forms.Keys.MButton);
            this.settingsStorage.Add("aimbotMethod", AimMethod.NearestToCrosshair);
            this.settingsStorage.Add("aimbotRadius", 250f);
            this.settingsStorage.Add("aimbotSmooth", OnOff.On);
            this.settingsStorage.Add("aimbotSpeed", 100f);
            this.settingsStorage.Add("aimbotTarget", Target.Enemies);
            this.settingsStorage.Add("aimSpottedOnly", YesNo.Yes);

            this.settingsStorage.Add("crosshairDrawData", OnOff.On);
            this.settingsStorage.Add("crosshairEnabled", YesNo.Yes);

            this.settingsStorage.Add("espDrawArrow", OnOff.On);
            this.settingsStorage.Add("espDrawBox", OnOff.On);
            this.settingsStorage.Add("espDrawCircle", OnOff.On);
            this.settingsStorage.Add("espDrawChickens", OnOff.On);
            this.settingsStorage.Add("espDrawDetails", OnOff.On);
            this.settingsStorage.Add("espDrawDistance", OnOff.On);
            this.settingsStorage.Add("espDrawHealth", OnOff.On);
            this.settingsStorage.Add("espDrawHostages", OnOff.On);
            this.settingsStorage.Add("espDrawLines", OnOff.On);
            this.settingsStorage.Add("espDrawName", OnOff.On);
            this.settingsStorage.Add("espDrawSkeleton", OnOff.On);
            this.settingsStorage.Add("espDrawTarget", Target.Enemies);
            this.settingsStorage.Add("espDrawWeapons", OnOff.On);
            this.settingsStorage.Add("espEnabled", YesNo.Yes);
            this.settingsStorage.Add("espGlowEnabled", YesNo.Yes);
            this.settingsStorage.Add("espGlowFadingEnabled", YesNo.No);
            this.settingsStorage.Add("espGlowFadingDistance", 50f);

            this.settingsStorage.Add("miscAutoPistolEnabled", YesNo.Yes);
            this.settingsStorage.Add("miscBunnyHopEnabled", YesNo.Yes);

            this.settingsStorage.Add("menuEnabled", YesNo.Yes);

            this.settingsStorage.Add("radarDrawLines", OnOff.On);
            this.settingsStorage.Add("radarDrawTarget", Target.Enemies);
            this.settingsStorage.Add("radarDrawView", OnOff.On);
            this.settingsStorage.Add("radarEnabled", YesNo.Yes);
            this.settingsStorage.Add("radarZoom", 20f);

            this.settingsStorage.Add("rcsEnabled", YesNo.No);
            this.settingsStorage.Add("rcsForce", 1.5f);

            this.settingsStorage.Add("spectatorDisableAim", YesNo.Yes);
            this.settingsStorage.Add("spectatorDisableTrigger", YesNo.No);
            this.settingsStorage.Add("spectatorDrawWarning", YesNo.Yes);

            this.settingsStorage.Add("triggerbotDelay", 20f);
            this.settingsStorage.Add("triggerbotEnabled", YesNo.Yes);
            this.settingsStorage.Add("triggerbotRecoilThreshold", 3f);
            this.settingsStorage.Add("triggerbotSnipersOnly", YesNo.No);
            this.settingsStorage.Add("triggerbotSpeedThreshold", 5f);
            this.settingsStorage.Add("triggerbotTarget", Target.Enemies);
            this.settingsStorage.Add("triggerKey", System.Windows.Forms.Keys.LButton);

            this.settingsStorage.Add("miscInfoEnabled", YesNo.Yes);
            this.settingsStorage.Add("crosshairDrawRecoil", YesNo.Yes);

            this.settingsStorage.Add("miscNoFlash", YesNo.No);
            this.settingsStorage.Add("miscNoSmoke", YesNo.No);

            this.settingsStorage.Add("soundEspEnabled", YesNo.No);
            this.settingsStorage.Add("soundEspSound", 1f);
            this.settingsStorage.Add("soundEspRange", 50f);
            this.settingsStorage.Add("soundEspInterval", 1000f);
            this.settingsStorage.Add("soundEspVolume", 1000f);

            this.settingsStorage.Add("crosshairDrawSoundESP", YesNo.No);
            #endregion
            this.highlighted = new bool[64];
            for (int i = 0; i < highlighted.Length; i++)
                highlighted[i] = false;

        }

        public override void Init()
        {
            this.form = new frmCSGOOverlay();
            this.gameController = new CSGOGameController(Form);
        }

        protected override void InterpretSetting(string name, string value)
        {
            frmCSGOOverlay form = ((frmCSGOOverlay)Program.GameController.Form);
            if (form == null)
                return;
            switch (name)
            {
                case "aimAllowAimJump":
                    form.aimAllowAimJump.SwitchToVal(ParseEnum<YesNo>(value));
                    break;
                case "aimbotBone":
                    form.aimbotBone.SwitchToVal(ParseEnum<AimBone>(value));
                    break;
                case "aimbotEnabled":
                    form.aimbotEnabled.SwitchToVal(ParseEnum<YesNo>(value));
                    break;
                case "aimbotCompensateRecoil":
                    form.aimbotCompensateRecoil.SwitchToVal(ParseEnum<OnOff>(value));
                    break;
                case "aimbotKey":
                    form.aimbotKey.SwitchToVal(ParseEnum<System.Windows.Forms.Keys>(value));
                    break;
                case "aimbotMethod":
                    form.aimbotMethod.SwitchToVal(ParseEnum<AimMethod>(value));
                    break;
                case "aimbotTarget":
                    form.aimbotTarget.SwitchToVal(ParseEnum<Target>(value));
                    break;
                case "aimbotRadius":
                    form.aimbotRadius.SwitchToVal(Convert.ToSingle(value));
                    break;
                case "aimbotRagemode":
                    form.aimbotRagemode.SwitchToVal(ParseEnum<OnOff>(value));
                    break;
                case "aimbotSmooth":
                    form.aimbotSmooth.SwitchToVal(ParseEnum<OnOff>(value));
                    break;
                case "aimbotSpeed":
                    form.aimbotSpeed.SwitchToVal(Convert.ToSingle(value));
                    break;
                case "aimSpottedOnly":
                    form.aimSpottedOnly.SwitchToVal(ParseEnum<YesNo>(value));
                    break;
                //Crosshair
                case "crosshairDrawData":
                    form.crosshairDrawData.SwitchToVal(ParseEnum<OnOff>(value));
                    break;
                case "crosshairDrawRecoil":
                    form.crosshairDrawRecoil.SwitchToVal(ParseEnum<YesNo>(value));
                    break;
                case "crosshairDrawSoundESP":
                    form.crosshairDrawSoundESP.SwitchToVal(ParseEnum<YesNo>(value));
                    break;
                case "crosshairEnabled":
                    form.crosshairEnabled.SwitchToVal(ParseEnum<YesNo>(value));
                    break;
                //ESP
                case "espDrawBox":
                    form.espDrawBox.SwitchToVal(ParseEnum<OnOff>(value));
                    break;
                case "espDrawCircle":
                    form.espDrawCircle.SwitchToVal(ParseEnum<OnOff>(value));
                    break;
                case "espDrawDistance":
                    form.espDrawDistance.SwitchToVal(ParseEnum<OnOff>(value));
                    break;
                case "espDrawDetails":
                    form.espDrawDetails.SwitchToVal(ParseEnum<OnOff>(value));
                    break;
                case "espDrawHealth":
                    form.espDrawHealth.SwitchToVal(ParseEnum<OnOff>(value));
                    break;
                case "espDrawLines":
                    form.espDrawLines.SwitchToVal(ParseEnum<OnOff>(value));
                    break;
                case "espDrawName":
                    form.espDrawName.SwitchToVal(ParseEnum<OnOff>(value));
                    break;
                case "espDrawSkeleton":
                    form.espDrawSkeleton.SwitchToVal(ParseEnum<OnOff>(value));
                    break;
                case "espDrawTarget":
                    form.espTarget.SwitchToVal(ParseEnum<Target>(value));
                    break;
                case "espDrawWeapons":
                    form.espDrawWeapons.SwitchToVal(ParseEnum<OnOff>(value));
                    break;
                case "espEnabled":
                    form.espEnabled.SwitchToVal(ParseEnum<YesNo>(value));
                    break;
                case "espGlowEnabled":
                    form.espGlowEnabled.SwitchToVal(ParseEnum<YesNo>(value));
                    break;
                case "espGlowFadingEnabled":
                    form.espGlowFadingEnabled.SwitchToVal(ParseEnum<YesNo>(value));
                    break;
                case "espGlowFadingDistance":
                    form.espGlowFadingDistance.SwitchToVal(Convert.ToSingle(value));
                    break;
                //Radar
                case "radarDrawLines":
                    form.radarDrawLines.SwitchToVal(ParseEnum<OnOff>(value));
                    break;
                case "radarDrawTarget":
                    form.radarTarget.SwitchToVal(ParseEnum<Target>(value));
                    break;
                case "radarDrawView":
                    form.radarDrawView.SwitchToVal(ParseEnum<OnOff>(value));
                    break;
                case "radarEnabled":
                    form.radarEnabled.SwitchToVal(ParseEnum<YesNo>(value));
                    break;
                case "radarZoom":
                    form.radarZoom.SwitchToVal(Convert.ToSingle(value));
                    break;
                //RCS
                case "rcsEnabled":
                    form.rcsEnabled.SwitchToVal(ParseEnum<YesNo>(value));
                    break;
                case "rcsForce":
                    form.rcsForce.SwitchToVal(Convert.ToSingle(value));
                    break;
                //SoundESP
                case "soundEspEnabled":
                    form.soundEspEnabled.SwitchToVal(ParseEnum<YesNo>(value));
                    break;
                case "soundEspSound":
                    form.soundEspSound.SwitchToVal(Convert.ToSingle(value));
                    break;
                case "soundEspRange":
                    form.soundEspRange.SwitchToVal(Convert.ToSingle(value));
                    break;
                case "soundEspInterval":
                    form.soundEspInterval.SwitchToVal(Convert.ToSingle(value));
                    break;
                case "soundEspVolume":
                    form.soundEspVolume.SwitchToVal(Convert.ToSingle(value));
                    break;
                //Spectator
                case "spectatorDrawWarning":
                    form.spectatorDrawWarning.SwitchToVal(ParseEnum<YesNo>(value));
                    break;
                case "spectatorDisableAim":
                    form.spectatorDisableAim.SwitchToVal(ParseEnum<YesNo>(value));
                    break;
                case "spectatorDisableTrigger":
                    form.spectatorDisableTrigger.SwitchToVal(ParseEnum<YesNo>(value));
                    break;
                //Trigger
                case "triggerKey":
                    form.triggerKey.SwitchToVal(ParseEnum<System.Windows.Forms.Keys>(value));
                    break;
                case "triggerbotEnabled":
                    form.triggerbotEnabled.SwitchToVal(ParseEnum<YesNo>(value));
                    break;
                case "triggerbotTarget":
                    form.triggerbotTarget.SwitchToVal(ParseEnum<Target>(value));
                    break;
                case "triggerbotDelay":
                    form.triggerbotDelay.SwitchToVal(Convert.ToSingle(value));
                    break;
                case "triggerbotRecoilThreshold":
                    form.triggerbotRecoilThreshold.SwitchToVal(Convert.ToSingle(value));
                    break;
                case "triggerbotSpeedThreshold":
                    form.triggerbotSpeedThreshold.SwitchToVal(Convert.ToSingle(value));
                    break;
                case "triggerbotSnipersOnly":
                    form.triggerbotSnipersOnly.SwitchToVal(ParseEnum<YesNo>(value));
                    break;
                //Misc
                case "miscAutoPistolEnabled":
                    form.miscAutoPistolEnabled.SwitchToVal(ParseEnum<YesNo>(value));
                    break;
                case "miscBunnyHopEnabled":
                    form.miscBunnyHopEnabled.SwitchToVal(ParseEnum<YesNo>(value));
                    break;
                case "miscInfoEnabled":
                    form.miscInfoEnabled.SwitchToVal(ParseEnum<YesNo>(value));
                    break;
                case "miscNoFlash":
                    form.miscNoFlash.SwitchToVal(ParseEnum<YesNo>(value));
                    break;
                case "miscNoSmoke":
                    form.miscNoSmoke.SwitchToVal(ParseEnum<YesNo>(value));
                    break;
                default:
                    Program.PrintError("Could not interpret porperty \"{0}\" (value: \"{1}\")", name, value);
                    break;
            }
        }
        public Player GetCurrentPlayer()
        {
            if (localPlayer == null)
                return null;
            if (localPlayer.SpectatorView == SpectatorView.NotSpectating)
                return localPlayer;
            else
                if (players == null)
                    return null;
                else
                    foreach (Player player in players)
                    {
                        if (player != null)
                            if (player.Index == localPlayer.SpectatorTarget)
                                return player;
                    }
            return null;
        }
        public Player GetPlayerByIndex(int index)
        {
            try
            {
                if (index > 0 && index <= 64)
                    return players[index - 1];
            }
            catch { }
            return null;
        }
        public override void SaveSettings(string fileName)
        {
            base.SaveSettings(fileName);
            if (form != null)
                if (((frmCSGOOverlay)form).currentConfig != null)
                    ((frmCSGOOverlay)form).currentConfig.Text = String.Format("Current config: {0}", fileName);
        }
        public override void ReadSettings(string fileName)
        {
            base.ReadSettings(fileName);
            if (form != null)
                if (((frmCSGOOverlay)form).currentConfig != null)
                    ((frmCSGOOverlay)form).currentConfig.Text = String.Format("Current config: {0}", fileName);
        }
        public void LoadMap()
        {
            string mapPath = string.Format(
                        "{0}\\csgo\\maps\\{1}.bsp",
                        Path.GetDirectoryName(GameController.Process.Modules[0].FileName),
                        serverMap);
            if (File.Exists(mapPath))
            {
                Program.PrintInfo("[BSP] Loading BSP...");
                currentMap = new BSP(mapPath);
                Program.PrintInfo(
                    "[BSP] Loaded, version {0}, map revision {1}\n[{7} brushes]\n[{8} brushsides]\n[{2} faces]\n[{3} originalFaces]\n[{4} surfedges]\n[{5} edges]\n[{6} vertices]\n[{9} nodes]\n[{10} leafs]",
                    currentMap.Header.version.ToString(),
                    currentMap.Header.mapRevision.ToString(),
                    currentMap.Faces.Length.ToString(),
                    currentMap.OriginalFaces.Length.ToString(),
                    currentMap.Surfedges.Length.ToString(),
                    currentMap.Edges.Count.ToString(),
                    currentMap.Vertices.Length.ToString(),
                    currentMap.Brushes.Length.ToString(),
                    currentMap.Brushsides.Length.ToString(),
                    currentMap.Nodes.Length.ToString(),
                    currentMap.Leafs.Length.ToString());
            }
            else
            {
                Program.PrintError("[BSP] BSP \"{0}\" not found at \"{1}\"!", serverMap, mapPath);
            }
        }

        public float GetPlayerKMH()
        {
            Player currentPlayer = GetCurrentPlayer();
            if (currentPlayer == null)
                return 0f;
            Vector2 velXY = new Vector2(currentPlayer.Velocity.X, currentPlayer.Velocity.Y);
            float length = velXY.Length();
            float speedPercent = 100f / 320f * (length % 320f);
            float speedMeters = length * 0.01905f;
            float speedKiloMetersPerHour = speedMeters * 60f * 60f / 1000f;
            return speedKiloMetersPerHour;
        }
    }
}
