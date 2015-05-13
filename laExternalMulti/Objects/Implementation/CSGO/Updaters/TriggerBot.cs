using laExternalMulti.Objects.Implementation.CSGO.Data;
using laExternalMulti.Objects.Implementation.CSGO.Data.Enums;
using laExternalMulti.Objects.Implementation.CSGO.Data.WeaponData;
using laExternalMulti.Objects.Updaters;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace laExternalMulti.Objects.Implementation.CSGO.Updaters
{
    class TriggerBot : ThreadedUpdater
    {
        #region VARIABLES
        private long lastShot = 0L, lastSeen = 0L;
        private CSGOImplementation csgo;
        private bool targetSeen;
        #endregion

        #region CONSTRUCTOR
        public TriggerBot()
            : base((int)(1000f / 60f))
        {
            targetSeen = false;
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
            if (csgo == null)
                return;
            if (csgo.SignOnState != SignOnState.SIGNONSTATE_FULL)
                return;
            if (csgo.Players == null)
                return;
            if (csgo.GetValue<YesNo>("triggerbotEnabled") == YesNo.No)
                return;
            if (csgo.LocalPlayer == null)
                return;
            if (csgo.LocalPlayer.Health <= 0)
                return;
            if (csgo.WeaponClip1 == 0)
                return;
            if (csgo.IsReloading)
                return;
            if (csgo.IsShooting)
                return;

            if ((csgo.TargetPlayer == null && csgo.TargetIndex <= 64))
            {
                targetSeen = false;
                return;
            }
            if (Program.GameImplementation.GetValue<YesNo>("spectatorDisableTrigger") == YesNo.Yes && csgo.FirstPersonSpectator)
                return;

            if (Program.GameImplementation.GetValue<YesNo>("triggerbotSnipersOnly") == YesNo.Yes && csgo.WeaponType != WeaponType.Sniper)
                return;

            Target target = csgo.GetValue<Target>("triggerbotTarget");

            if (csgo.TargetIndex > 0 && csgo.TargetIndex <= 64 && csgo.TargetPlayer != null)
            {
                if (target == Target.Allies && csgo.TargetPlayer.InTeam != csgo.LocalPlayer.InTeam)
                    return;
                if (target == Target.Enemies && csgo.TargetPlayer.InTeam == csgo.LocalPlayer.InTeam)
                    return;
            }
            else
            {
                int idx = csgo.TargetIndex - 1 - csgo.Players.Length;
                if (idx < 0 || idx >= csgo.Entities.Length)
                    return;
                if (csgo.Entities[idx] == null)
                    return;
                if (csgo.Entities[idx].ClassID != Data.Enums.ClassID.CSPlayer && csgo.Entities[idx].ClassID != Data.Enums.ClassID.Chicken)
                    return;
            }

            if (!targetSeen)
            {
                lastSeen = DateTime.Now.Ticks;
                targetSeen = true;
            }

            if (csgo.GetValue<float>("triggerbotSpeedThreshold") < csgo.GetPlayerKMH())
                return;

            TimeSpan spanSeen = new TimeSpan(DateTime.Now.Ticks - lastSeen);

            if (
                targetSeen &&
                spanSeen.TotalMilliseconds >= csgo.GetValue<float>("triggerbotDelay") &&
                (csgo.WeaponType == WeaponType.Sniper || 100f / 9f * csgo.LocalPlayer.PunchVector.Length() < csgo.GetValue<float>("triggerbotRecoilThreshold"))
                )
            {
                if (csgo.WeaponType != WeaponType.Grenade && csgo.WeaponType != WeaponType.Melee && csgo.WeaponType != WeaponType.Special)
                {
                    lastShot = DateTime.Now.Ticks;
                    Shoot();
                }
                else if (csgo.WeaponType == WeaponType.Melee)
                {
                    if (csgo.Players != null && csgo.TargetPlayer != null)
                    {
                        float yaw1 = csgo.LocalPlayer.Yaw, yaw2 = csgo.TargetPlayer.Yaw;
                        //while (yaw1 < 0f)
                        //    yaw1 += 360f;
                        //while (yaw2 < 0f)
                        //    yaw2 += 360f;
                        if (csgo.LocalPlayer.DistanceToOtherEntityInMetres(csgo.TargetPlayer) <= 1)
                            RightKnife();
                    }
                }
            }
        }

        private void Shoot()
        {
            WinAPI.mouse_event(WinAPI.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            Thread.Sleep(10);
            WinAPI.mouse_event(WinAPI.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            Thread.Sleep(10);
            //SpamChat();
        }

        private void RightKnife()
        {
            WinAPI.mouse_event(WinAPI.MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
            Thread.Sleep(10);
            WinAPI.mouse_event(WinAPI.MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
            Thread.Sleep(10);
        }

        //private void SpamChat()
        //{
        //    MemoryUpdater mem = ((CSGOGameController)Program.GameController).MemoryUpdater;
        //    byte[] safeData = mem.ReadEngineBuffer();
        //    string toWrite = string.Format("say Bang#{0}\n", ++shots);
        //    byte[] toWriteData = Encoding.ASCII.GetBytes(toWrite);

        //    mem.WriteToEngineBuffer(toWriteData);
        //    UpdateBuffer();
        //    mem.WriteToEngineBuffer(safeData);

        //    Debug.WriteLine("safeString: {0}, toWrite: {1}", Encoding.ASCII.GetString(safeData).Replace("\n", "\\n").Replace("\0", "\\0"), toWrite.Replace("\n", "\\n").Replace("\0", "\\0"));
        //}

        //private void UpdateBuffer()
        //{
        //    CSGOGameController controller = (CSGOGameController)Program.GameController;

        //    byte stateOrig = WinAPI.ReadMemory(controller.Process.Handle, MemoryUpdater.dllClientAddress + GameOffsets.CL_LOCAL_BUTTONS_FORWARD, 1)[0];
        //    IntPtr bytesWritten = IntPtr.Zero;
        //    WinAPI.WriteProcessMemoryProtected(controller.Process.Handle, (IntPtr)(MemoryUpdater.dllClientAddress + GameOffsets.CL_LOCAL_BUTTONS_FORWARD), new byte[] { 0x00 }, 1, out bytesWritten, WinAPI.Protection.PAGE_EXECUTE_READWRITE);
        //    WinAPI.WriteProcessMemoryProtected(controller.Process.Handle, (IntPtr)(MemoryUpdater.dllClientAddress + GameOffsets.CL_LOCAL_BUTTONS_FORWARD), new byte[] { 0x01 }, 1, out bytesWritten, WinAPI.Protection.PAGE_EXECUTE_READWRITE);
        //    WinAPI.WriteProcessMemoryProtected(controller.Process.Handle, (IntPtr)(MemoryUpdater.dllClientAddress + GameOffsets.CL_LOCAL_BUTTONS_FORWARD), new byte[] { stateOrig }, 1, out bytesWritten, WinAPI.Protection.PAGE_EXECUTE_READWRITE);
        //    WinAPI.GenerateKeyKeyBEvent(WinAPI.VirtualKeyShort.F11, WinAPI.ScanCodeShort.F11, 5);
        //}
    }
}
