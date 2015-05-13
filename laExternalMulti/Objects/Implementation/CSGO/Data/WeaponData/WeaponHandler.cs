using laExternalMulti.Objects.Implementation.CSGO.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.Data.WeaponData
{
    public class WeaponHandler
    {
        #region VARIABLES
        private static WeaponHandler instance = new WeaponHandler();
        private string[] weaponNames;
        private int[] weaponFireRate;
        private const float fireRateMultiplier = 1.5f;
        #endregion

        #region PROPERTIES
        public static WeaponHandler Instance { get { return instance; } }
        #endregion

        #region CONSTRUCTOR
        private WeaponHandler()
        {
            weaponNames = new string[50];
            weaponNames[0] = "0";
            weaponNames[1] = "Deagle";
            weaponNames[2] = "Dual Berettas";
            weaponNames[3] = "Five-Seven";
            weaponNames[4] = "Glock";
            weaponNames[5] = "5";
            weaponNames[6] = "6";
            weaponNames[7] = "AK47";
            weaponNames[8] = "AUG";
            weaponNames[9] = "AWP";
            weaponNames[10] = "Famas";
            weaponNames[11] = "G3SG1";
            weaponNames[12] = "12";
            weaponNames[13] = "Galil";
            weaponNames[14] = "Magma";
            weaponNames[15] = "15";
            weaponNames[16] = "M4A1";
            weaponNames[17] = "MAC10";
            weaponNames[18] = "18";
            weaponNames[19] = "P90";
            weaponNames[20] = "20";
            weaponNames[21] = "21";
            weaponNames[22] = "22";
            weaponNames[23] = "23";
            weaponNames[24] = "UMP-45";
            weaponNames[25] = "XM1014";
            weaponNames[26] = "Bizon";
            weaponNames[27] = "MAG7";
            weaponNames[28] = "Negev";
            weaponNames[29] = "Sawed-off";
            weaponNames[30] = "Tec9";
            weaponNames[31] = "Taser";
            weaponNames[32] = "USP";
            weaponNames[33] = "MP7";
            weaponNames[34] = "MP9";
            weaponNames[35] = "Nova";
            weaponNames[36] = "CZ75";
            weaponNames[37] = "37";
            weaponNames[38] = "SCAR";
            weaponNames[39] = "SG 553";
            weaponNames[40] = "Scout";
            weaponNames[41] = "Golden knife";
            weaponNames[42] = "Knife";
            weaponNames[43] = "Flash";
            weaponNames[44] = "HEgrenade";
            weaponNames[45] = "Smoke";
            weaponNames[46] = "Molotov";
            weaponNames[47] = "Decoy";
            weaponNames[48] = "Incinidary";
            weaponNames[49] = "Bomb";

            weaponFireRate = new int[50];
            weaponFireRate[0] = 0;
            weaponFireRate[1] = CalcRate(267, 12);
            weaponFireRate[2] = CalcRate(750, 32);
            weaponFireRate[3] = CalcRate(400, 69);
            weaponFireRate[4] = CalcRate(400, 84);
            weaponFireRate[5] = 0;
            weaponFireRate[6] = 0;
            weaponFireRate[7] = CalcRate(600, 88);
            weaponFireRate[8] = CalcRate(666, 88);
            weaponFireRate[9] = CalcRate(41, 3);
            weaponFireRate[10] = CalcRate(666, 80);
            weaponFireRate[11] = CalcRate(240, 65);
            weaponFireRate[12] = 0;
            weaponFireRate[13] = CalcRate(666, 76);
            weaponFireRate[14] = CalcRate(600, 73);
            weaponFireRate[15] = 0;
            weaponFireRate[16] = CalcRate(666, 76);
            weaponFireRate[17] = CalcRate(800, 80);
            weaponFireRate[18] = 0;
            weaponFireRate[19] = CalcRate(857, 61);
            weaponFireRate[20] = 0;
            weaponFireRate[21] = 0;
            weaponFireRate[22] = 0;
            weaponFireRate[23] = 0;
            weaponFireRate[24] = CalcRate(571, 76);
            weaponFireRate[25] = CalcRate(240, 4);
            weaponFireRate[26] = CalcRate(750, 80);
            weaponFireRate[27] = CalcRate(92, 4);
            weaponFireRate[28] = CalcRate(1000, 76);
            weaponFireRate[29] = CalcRate(71, 3);
            weaponFireRate[30] = CalcRate(500, 65);
            weaponFireRate[31] = 0;
            weaponFireRate[32] = CalcRate(352, 69);
            weaponFireRate[33] = CalcRate(800, 84);
            weaponFireRate[34] = CalcRate(857, 58);
            weaponFireRate[35] = CalcRate(68, 4);
            weaponFireRate[36] = CalcRate(600, 65);
            weaponFireRate[37] = 0;
            weaponFireRate[38] = CalcRate(240, 65);
            weaponFireRate[39] = CalcRate(666, 69);
            weaponFireRate[40] = CalcRate(48, 100);
            weaponFireRate[41] = 0;
            weaponFireRate[42] = 0;
            weaponFireRate[43] = 0;
            weaponFireRate[44] = 0;
            weaponFireRate[45] = 0;
            weaponFireRate[46] = 0;
            weaponFireRate[47] = 0;
            weaponFireRate[48] = 0;
            weaponFireRate[49] = 0;
        }
        #endregion

        #region METHODS
        public string GetWeaponName(int ID)
        {
            if (ID > 0 && ID < weaponNames.Length)
                return weaponNames[ID];
            return "unknown";
        }
        public int GetWeaponFireRate(int ID)
        {
            if (ID > 0 && ID < weaponNames.Length)
                return weaponFireRate[ID];
            return 400;
        }
        private int CalcRate(int rpm, int recoil)
        {
            float roundsPerSecond = rpm / 60f;
            float timeBetweenShots = 1000f / roundsPerSecond;
            float totalTime = timeBetweenShots + (timeBetweenShots - timeBetweenShots / 100f * recoil);
            totalTime *= fireRateMultiplier;
            return (int)totalTime;
        }
        public WeaponType GetWeaponType(int ID)
        {
            switch (ID)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 30:
                case 32:
                case 36:
                    return WeaponType.Pistol;
                case 7:
                case 10:
                case 13:
                case 16:
                    return WeaponType.AssaultRifle;
                case 8:
                case 39:
                    return WeaponType.ZoomRifle;
                case 9:
                case 40:
                    return WeaponType.Sniper;
                case 11:
                case 38:
                    return WeaponType.AutoSniper;
                case 14:
                case 28:
                    return WeaponType.MachineGun;
                case 17:
                case 19:
                case 24:
                case 26:
                case 33:
                    return WeaponType.MachinePistol;
                case 25:
                case 27:
                case 29:
                    return WeaponType.Shotgun;
                case 41:
                case 42:
                    return WeaponType.Melee;
                case 43:
                case 44:
                case 45:
                case 46:
                case 47:
                case 48:
                    return WeaponType.Grenade;
                case 31:
                case 49:
                    return WeaponType.Special;
                default:
                    return WeaponType.Unknown;
            }
        }
        public WeaponType GetWeaponType(ClassID ID)
        {
            switch (ID)
            {
                case ClassID.AK47:
                case ClassID.WeaponAUG:
                case ClassID.WeaponAWP:
                case ClassID.WeaponBizon:
                case ClassID.WeaponG3SG1:
                case ClassID.WeaponGalilAR:
                case ClassID.WeaponM249:
                case ClassID.WeaponM4A1:
                case ClassID.WeaponMP7:
                case ClassID.WeaponMP9:
                case ClassID.WeaponMag7:
                case ClassID.WeaponNOVA:
                case ClassID.WeaponNegev:
                case ClassID.WeaponSG556:
                case ClassID.WeaponSSG08:
                case ClassID.WeaponUMP45:
                case ClassID.WeaponXM1014:
                    return WeaponType.AssaultRifle;
                default:
                    return WeaponType.Melee;
            }
        }
        #endregion
    }
}
