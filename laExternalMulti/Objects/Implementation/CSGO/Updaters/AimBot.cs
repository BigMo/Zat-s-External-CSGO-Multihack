using laExternalMulti.Objects.Implementation.CSGO.Data;
using laExternalMulti.Objects.Implementation.CSGO.Data.AimMethods;
using laExternalMulti.Objects.Implementation.CSGO.Data.Enums;
using laExternalMulti.Objects.Implementation.CSGO.Data.WeaponData;
using laExternalMulti.Objects.Updaters;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace laExternalMulti.Objects.Implementation.CSGO.Updaters
{
    class AimBot : ThreadedUpdater
    {
        #region VARIABLES
        private CSGOImplementation csgo;
        private AimMethodImplementation lowestHP, nearestToPlayer, nearestToCrosshair, currentImplementation, rageMode;
        private AimMethodResult lastTarget;
        private long lastTick;
        private Vector2 lastPoint;
        private AimBone lastAimBone, randomAimBone;
        public static AimBone[] RandomBones = new AimBone[] { AimBone.Head, AimBone.Neck, AimBone.Torso, AimBone.Hip, AimBone.Knees, AimBone.Feet };
        public static AimBone[] RandomLethalBones = new AimBone[] { AimBone.Neck, AimBone.Torso };
        public static AimBone[] RandomBodyBones = new AimBone[] { AimBone.Torso, AimBone.Hip };
        #endregion

        #region CONSTRUCTOR
        public AimBot()
            : base((int)(1000f / 60f))
        {
            lowestHP = new LowestHP();
            nearestToCrosshair = new NearestToCrosshair();
            nearestToPlayer = new NearestToPlayer();
            rageMode = new RageMode();
            currentImplementation = nearestToCrosshair;
            lastTarget = null;
            lastTick = 0L;
            lastPoint = Vector2.Zero;
        }
        #endregion

        public override void OnUpdaterTick()
        {
            if (Program.GameImplementation == null)
                return;
            if (Program.GameController == null)
                return;
            if (!Program.GameController.IsGameRunning)
                return;
            csgo = (CSGOImplementation)Program.GameImplementation;
            CSGOGameController csgoController = (CSGOGameController)Program.GameController;
            if (csgo.SignOnState != SignOnState.SIGNONSTATE_FULL)
                return;
            if (csgoController.MemoryUpdater.Tick == lastTick)
                return;
            lastTick = csgoController.MemoryUpdater.Tick;

            if (csgo.LocalPlayer == null)
                return;
            if (csgo.LocalPlayer.Health <= 0)
                return;
            if ((csgo.WeaponType == WeaponType.Grenade || csgo.WeaponType == WeaponType.Melee || csgo.WeaponType == WeaponType.Special))
                return;
            if (csgo.WeaponClip1 <= 0)
                return;
            if (csgo.IsReloading)
                return;

            bool rcsCompensateOn = csgo.GetValue<OnOff>("aimbotCompensateRecoil") == OnOff.On;
            CSGOGameController controller = (CSGOGameController)Program.GameController;
            Vector3 rcsCompensation = Vector3.Zero;
            controller.RecoilControl.IsActive = lastTarget == null;

            if (csgo.GetValue<YesNo>("aimbotEnabled") == YesNo.No)
                return;
            if (csgo.GetValue<YesNo>("spectatorDisableAim") == YesNo.Yes && csgo.FirstPersonSpectator)
                return;

            //Aimkey not down
            //-> Reset target
            if (!Program.GameController.InputUpdater.KeyIsDown(Program.GameImplementation.GetValue<Keys>("aimbotKey")))
            {
                lastTarget = null;
                lastAimBone = (AimBone)(((int)lastAimBone + 1) % 9);
                return;
            }

            //Aimkey down but dead target
            //-> Release key to reset target
            if (lastTarget != null)
                if (!lastTarget.IsValid())
                    if (csgo.GetValue<YesNo>("aimAllowAimJump") == YesNo.No)
                        return;

            Vector2 screenM = new Vector2(csgo.ScreenSize.Width / 2f, csgo.ScreenSize.Height / 2f);
            AimBone bone = csgo.GetValue<AimBone>("aimbotBone");
            if (bone != lastAimBone)
            {
                lastAimBone = bone;
                if (IsRandomAimBone(lastAimBone))
                {
                    randomAimBone = GetRandomBone(bone);
                };
            }
            if (IsRandomAimBone(bone))
                bone = randomAimBone;
            //Aimkey down but no target
            //-> Get new target
            if (lastTarget == null || (lastTarget != null && !lastTarget.IsValid()) || csgo.GetValue<YesNo>("aimAllowAimJump") == YesNo.Yes)
            {
                Target target = csgo.GetValue<Target>("aimbotTarget");
                AimMethod method = csgo.GetValue<AimMethod>("aimbotMethod");
                float radius = csgo.GetValue<float>("aimbotRadius");

                if (csgo.GetValue<OnOff>("aimbotRagemode") == OnOff.On)
                {
                    currentImplementation = rageMode;
                }
                else
                {
                    switch (method)
                    {
                        case AimMethod.LowestHP:
                            currentImplementation = lowestHP;
                            break;
                        case AimMethod.NearestToCrosshair:
                            currentImplementation = nearestToCrosshair;
                            break;
                        case AimMethod.NearestToPlayer:
                            currentImplementation = nearestToPlayer;
                            break;
                    }
                }
                lastTarget = currentImplementation.GetAimTarget(csgo, target, bone, screenM, radius);
            }
            //No target found?
            //-> Break.
            if (lastTarget == null || (lastTarget != null && !lastTarget.IsValid()))
                return;

            //Vector2 aimPos = currentImplementation.AimAt(csgo, bone, csgo.GetPlayerByIndex(lastTarget.PlayerIndex));
            //if (aimPos != lastPoint && aimPos != Vector2.Zero)
            //{
            if (csgo.ViewAngles == Vector3.Zero)
                return;
            Vector3 viewAngles = csgo.ViewAngles;
            viewAngles = Geometry.CalcAngle(csgo.LocalPlayer.Vector3 + csgo.ViewOffset, csgo.Players[lastTarget.PlayerIndex - 1].Skeleton.GetBone(bone));
            if (rcsCompensateOn)
                viewAngles = viewAngles - csgo.LocalPlayer.PunchVector * 2;

            if (csgo.GetValue<OnOff>("aimbotSmooth") == OnOff.On)
            {
                Vector3 smoothed = viewAngles - csgo.ViewAngles;
                smoothed *= csgo.GetValue<float>("aimbotSpeed") / 100f;
                if (Math.Abs(smoothed.Y) < 45)
                    viewAngles = csgo.ViewAngles + smoothed;
            }
            viewAngles.Z = 0f;
            viewAngles = Geometry.ClampAngle(viewAngles);
            ((CSGOGameController)Program.GameController).MemoryUpdater.WriteViewAngles(viewAngles);
            //}
        }

        private AimBone GetRandomBone(AimBone bone)
        {
            switch (bone)
            {
                case AimBone.Random:
                    return RandomBones[Program.random.Next(0, RandomBones.Length)];
                case AimBone.RandomLethal:
                    return RandomLethalBones[Program.random.Next(0, RandomLethalBones.Length)];
                case AimBone.RandomBody:
                    return RandomBodyBones[Program.random.Next(0, RandomBodyBones.Length)];
            }
            return AimBone.Feet;
        }
        private bool IsRandomAimBone(AimBone bone)
        {
            return
                bone == AimBone.Random ||
                bone == AimBone.RandomLethal ||
                bone == AimBone.RandomBody;
        }
    }
}
