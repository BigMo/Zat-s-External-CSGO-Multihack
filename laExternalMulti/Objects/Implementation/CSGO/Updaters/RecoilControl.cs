using laExternalMulti.Objects.Implementation.CSGO.Data;
using laExternalMulti.Objects.Implementation.CSGO.Data.Enums;
using laExternalMulti.Objects.Updaters;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.Updaters
{
    class RecoilControl : ThreadedUpdater
    {
        #region VARIABLES
        private CSGOImplementation csgo;
        private Vector3 lastPunch, currentPunch;
        private bool isActive;
        private static byte[] zeroFloat = BitConverter.GetBytes(0f);
        #endregion

        #region PROPERTIES
        public bool IsActive { get { return isActive; } set { isActive = value; } }
        public Vector3 CurrentPunch { get { return currentPunch; } }
        public Vector3 LastPunch { get { return lastPunch; } }
        public Vector2 RecoilOffset
        {
            get
            {
                float force = csgo.GetValue<float>("rcsForce");
                float distx = Program.GameController.Form.Width / 90f;
                float disty = Program.GameController.Form.Height / 90f;
                distx *= currentPunch.Y * force * -1;
                disty *= currentPunch.X * force;
                return new Vector2(distx, disty);
            }
        }
        #endregion

        #region CONSTRUCTOR
        public RecoilControl()
            : base(1)
        {
            lastPunch = Vector3.Zero;
            currentPunch = Vector3.Zero;
            isActive = true;
        }
        #endregion

        #region METHODS
        public override void OnUpdaterTick()
        {
            if (Program.GameImplementation == null)
                return;
            if (Program.GameController == null)
                return;
            if (!Program.GameController.IsGameRunning)
                return;
            csgo = (CSGOImplementation)Program.GameImplementation;
            CSGOGameController controller = (CSGOGameController)Program.GameController;
            if (csgo == null)
                return;
            if (csgo.SignOnState != SignOnState.SIGNONSTATE_FULL)
                return;
            if (csgo.Players == null)
                return;
            if (csgo.LocalPlayer == null)
                return;
            if (csgo.LocalPlayer.Health <= 0)
                return;

            DoOtherStuff();

            if (csgo.GetValue<YesNo>("rcsEnabled") == YesNo.No)
                return;
            float force = csgo.GetValue<float>("rcsForce");
            if (lastPunch == csgo.LocalPlayer.PunchVector)
                return;
            if (csgo.LocalPlayer.PunchVector == Vector3.Zero)
                return;
            currentPunch = csgo.LocalPlayer.PunchVector - lastPunch;
            if (IsActive && currentPunch != Vector3.Zero)
            {
                Vector3 newViewAngles = csgo.ViewAngles - currentPunch * force;
                newViewAngles = Geometry.ClampAngle(newViewAngles);
                newViewAngles.Z = 0f;
                controller.MemoryUpdater.WriteViewAngles(newViewAngles);
            }
            lastPunch = csgo.LocalPlayer.PunchVector;
        }

        private void DoOtherStuff()
        {
            //No flash
            if (csgo.GetValue<YesNo>("miscNoFlash") == YesNo.Yes)
            {
                if (csgo.FlashMaxAlpha > 0)
                    WinAPI.WriteMemory(Program.GameController.Process.Handle, (int)csgo.LocalPlayer.Address + GameOffsets.CL_LOCAL_FLASH_MAX_ALPHA, zeroFloat, zeroFloat.Length);
                if (csgo.FlashMaxDuration > 0)
                    WinAPI.WriteMemory(Program.GameController.Process.Handle, (int)csgo.LocalPlayer.Address + GameOffsets.CL_LOCAL_FLASH_MAX_DURATION, zeroFloat, zeroFloat.Length);
            }
        }
        #endregion
    }
}
