using laExternalMulti.Objects.Implementation.CSGO.Data.Enums;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.Data
{
    public class Player : Entity
    {
        #region VARIABLES
        private int weaponIndex, money;
        #endregion

        #region PROPERTIES
        public int Armor
        {
            get
            {
                if (((CSGOImplementation)Program.GameImplementation).ScrBrdArmor != null)
                    return ((CSGOImplementation)Program.GameImplementation).ScrBrdArmor[this.Index - 1];
                else
                    return 0;
            }
        }
        public int Assists
        {
            get
            {
                if (((CSGOImplementation)Program.GameImplementation).ScrBrdAssists != null)
                    return ((CSGOImplementation)Program.GameImplementation).ScrBrdAssists[this.Index - 1];
                else
                    return 0;
            }
        }
        public int Deaths
        {
            get
            {
                if (((CSGOImplementation)Program.GameImplementation).ScrBrdDeaths != null)
                    return ((CSGOImplementation)Program.GameImplementation).ScrBrdDeaths[this.Index - 1];
                else
                    return 0;
            }
        }
        //public override int Health
        //{
        //    get
        //    {
        //        if (((CSGOImplementation)Program.GameImplementation).ScrBrdHealth != null)
        //            return ((CSGOImplementation)Program.GameImplementation).ScrBrdHealth[this.Index - 1];
        //        else
        //            return 0;
        //    }
        //}
        public int Kills
        {
            get
            {
                if (((CSGOImplementation)Program.GameImplementation).ScrBrdKills != null)
                    return ((CSGOImplementation)Program.GameImplementation).ScrBrdKills[this.Index - 1];
                else
                    return 0;
            }
        }
        public int Score
        {
            get
            {
                if (((CSGOImplementation)Program.GameImplementation).ScrBrdScore != null)
                    return ((CSGOImplementation)Program.GameImplementation).ScrBrdScore[this.Index - 1];
                else
                    return 0;
            }
        }
        public int Rank
        {
            get
            {
                if (((CSGOImplementation)Program.GameImplementation).ScrBrdRanks != null)
                    return ((CSGOImplementation)Program.GameImplementation).ScrBrdRanks[this.Index - 1];
                else
                    return 0;
            }
        }
        public int Wins
        {
            get
            {
                if (((CSGOImplementation)Program.GameImplementation).ScrBrdWins != null)
                    return ((CSGOImplementation)Program.GameImplementation).ScrBrdWins[this.Index - 1];
                else
                    return 0;
            }
        }
        public int Money { get { return money; } }
        public int WeaponIndex { get { return weaponIndex; } set { weaponIndex = value; } }
        #endregion

        #region CONSTRUCTORS
        public Player(long address, long radarAddress, int id)
            : base(address, radarAddress, id)
        {
            IntPtr handle = Program.GameImplementation.GameController.Process.Handle;

            this.name = WinAPI.ReadString(handle, radarAddress + GameOffsets.CL_ENTITY_RADARBASENAME, 32, Encoding.Unicode);
            this.weaponIndex = WinAPI.ReadInt16(handle, address + GameOffsets.CL_ENTITY_ACTIVE_WEAPON) - 64 - 1;
        }

        public override void Update(long address, long radarAddress, int id)
        {
            base.Update(address, radarAddress, id);
            IntPtr handle = Program.GameImplementation.GameController.Process.Handle;

            this.name = WinAPI.ReadString(handle, radarAddress + GameOffsets.CL_ENTITY_RADARBASENAME, 32);

            if (this.name.Contains('\0'))
                this.name = this.name.Substring(0, this.name.IndexOf('\0'));

            this.weaponIndex = WinAPI.ReadInt16(handle, address + GameOffsets.CL_ENTITY_ACTIVE_WEAPON) - 64 - 1;
            this.money = WinAPI.ReadInt32(handle, address + GameOffsets.CL_ENTITY_ACCOUNT);

            if (this.Yaw == 0f && this.Skeleton != null)
            {
                CheckYaw();
            }
        }

        public Player(Player copyFrom)
            : base(copyFrom)
        { }
        #endregion

        #region METHODS
        public override bool IsValid()
        {
            return
                base.IsValid() &&
                !this.IsDormant &&
                this.Health > 0 &&
                !(this.X == 0f && this.Y == 0f && this.Z == 0f) &&
                this.Address != 0L &&
                this.LifeState == Enums.LifeState.Alive;
        }
        public void CheckYaw()
        {
            CSGOImplementation csgo = (CSGOImplementation)Program.GameImplementation;
            if (csgo.LocalPlayer == null)
                return;
            if (this.Index == csgo.LocalPlayer.Index)
                return;
            if (this.WeaponIndex >= 0 && this.WeaponIndex < csgo.Entities.Length)
            {
                if (csgo.Entities[this.WeaponIndex] != null)
                    if (WeaponData.WeaponHandler.Instance.GetWeaponType(csgo.Entities[this.WeaponIndex].ClassIDInt) != WeaponData.WeaponType.Melee)
                    {
                        this.yaw = Geometry.DegreeBetweenVectors(
                            new Vector2(this.Skeleton.Weapon1.X, this.Skeleton.Weapon1.Y),
                            new Vector2(this.Skeleton.Weapon2.X, this.Skeleton.Weapon2.Y)) - 90f;
                        return;
                    }
            }
            this.yaw = -1f;
        }
        protected override void HealthChanged(int oldHealth, int newHealth)
        {
            if (newHealth < oldHealth)
            {
                if (this.Skeleton != null && ((CSGOImplementation)Program.GameImplementation).TargetIndex == this.Index)
                    ((CSGOImplementation)Program.GameImplementation).Damages.Add(new Damage((oldHealth - newHealth).ToString(), this.Skeleton.GetBone(AimBone.Head) + Vector3.Up * 4));
            }
            base.HealthChanged(oldHealth, newHealth);
        }
        #endregion
    }
}
