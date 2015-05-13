using laExternalMulti.Objects.Implementation.CSGO.Data;
using laExternalMulti.Objects.Implementation.CSGO.Data.Enums;
using laExternalMulti.Objects.Implementation.CSGO.Data.WeaponData;
using laExternalMulti.Objects.UI;
using laExternalMulti.Objects.Updaters;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace laExternalMulti.Objects.Implementation.CSGO.Updaters
{
    class MemoryUpdater : ThreadedUpdater
    {
        #region VARIABLES
        private long tick;
        private long newPlayers = 0L;
        //private Dictionary<int, string> entityClasses;
        private const int MAX_PLAYERS = 64, MAX_ENTITIES = 1024;
        private bool updatedOffsets = false;
        private static SigScanner scanner;
        private const int MAX_DUMP_SIZE = 0xFFFF;
        public static int dllClientAddress, dllEngineAddress;
        public static long dllClientSize, dllEngineSize;
        private static int localPlayer;

        public int entityListAddress, localAddress, radarAddress, scoreBoardAddress, enginePointer;
        public bool isWritingToMem = false;
        #endregion

        #region PROPERTIES
        public long Tick { get { return tick; } }
        //public int EntityCount { get { return entityClasses.Count; } }
        #endregion

        #region CONSTRUCTOR
        public MemoryUpdater()
            : base(0)
        {
            tick = 0;
            //entityClasses = new Dictionary<int, string>();
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
            if (!Program.GameController.IsInGame)
                return;
            CSGOImplementation csgo = ((CSGOImplementation)Program.GameImplementation);

            IntPtr handle = Program.GameImplementation.GameController.Process.Handle;
            
            //Get addresses
            if (!updatedOffsets)
            {
                FindOffsets();
            }
            entityListAddress = dllClientAddress + GameOffsets.CL_ENTITY_LIST;
            localAddress = WinAPI.ReadInt32(handle, dllClientAddress + GameOffsets.CL_LOCAL_BASE_ENTITY);
            radarAddress = WinAPI.ReadInt32(handle, dllClientAddress + GameOffsets.CL_RADAR_BASE);
            radarAddress = WinAPI.ReadInt32(handle, radarAddress + GameOffsets.CL_RADAR_OFFSET); //B658BEC
            scoreBoardAddress = WinAPI.ReadInt32(handle, dllClientAddress + GameOffsets.CL_SCRBRD_BASE);
            enginePointer = WinAPI.ReadInt32(handle, dllEngineAddress + GameOffsets.EN_ENGINE_POINTER);
            csgo.SignOnState = (SignOnState)WinAPI.ReadInt32(handle, enginePointer + GameOffsets.EN_SIGNONSTATE);

            if (csgo.SignOnState < SignOnState.SIGNONSTATE_PRESPAWN || csgo.SignOnState > SignOnState.SIGNONSTATE_FULL)
                return;

            //General
            csgo.ScreenSize = new SharpDX.Size2(Program.GameController.WindowArea.Width, Program.GameController.WindowArea.Height);

            int targetIndex = WinAPI.ReadInt32(handle, localAddress + GameOffsets.CL_LOCAL_CROSSHAIR_TARGET);
            Matrix4x4 viewMatrix = Matrix4x4.ReadMatrix(handle, dllClientAddress + GameOffsets.CL_LOCAL_VIEWMATRIX);
            bool c4Planted = false;


            //Refresh players
            if (Environment.TickCount - newPlayers >= 1000)
            {
                newPlayers = Environment.TickCount;
                csgo.Players = null;
                csgo.Entities = null;
            }

            //Read scrbrd-data
            byte[] scrbrdData = WinAPI.ReadMemory(handle, scoreBoardAddress, 0x1C38);

            if (csgo.ScrBrdArmor == null)
                csgo.ScrBrdArmor = new int[MAX_PLAYERS];
            if (csgo.ScrBrdAssists == null)
                csgo.ScrBrdAssists = new int[MAX_PLAYERS];
            if (csgo.ScrBrdDeaths == null)
                csgo.ScrBrdDeaths = new int[MAX_PLAYERS];
            if (csgo.ScrBrdHealth == null)
                csgo.ScrBrdHealth = new int[MAX_PLAYERS];
            if (csgo.ScrBrdKills == null)
                csgo.ScrBrdKills = new int[MAX_PLAYERS];
            if (csgo.ScrBrdScore == null)
                csgo.ScrBrdScore = new int[MAX_PLAYERS];
            if (csgo.ScrBrdRanks == null)
                csgo.ScrBrdRanks = new int[MAX_PLAYERS];
            if (csgo.ScrBrdWins == null)
                csgo.ScrBrdWins = new int[MAX_PLAYERS];

            for (int i = 0; i < MAX_PLAYERS; i++)
            {
                csgo.ScrBrdArmor[i] = BitConverter.ToInt32(scrbrdData, GameOffsets.CL_SCRBRD_ARMOR + 4 * i);
                csgo.ScrBrdAssists[i] = BitConverter.ToInt32(scrbrdData, GameOffsets.CL_SCRBRD_ASSISTS + 4 * i);
                csgo.ScrBrdDeaths[i] = BitConverter.ToInt32(scrbrdData, GameOffsets.CL_SCRBRD_DEATHS + 4 * i);
                csgo.ScrBrdHealth[i] = BitConverter.ToInt32(scrbrdData, GameOffsets.CL_SCRBRD_HEALTH + 4 * i);
                csgo.ScrBrdKills[i] = BitConverter.ToInt32(scrbrdData, GameOffsets.CL_SCRBRD_KILLS + 4 * i);
                csgo.ScrBrdScore[i] = BitConverter.ToInt32(scrbrdData, GameOffsets.CL_SCRBRD_SCORE + 4 * i);
                csgo.ScrBrdRanks[i] = BitConverter.ToInt32(scrbrdData, GameOffsets.CL_SCRBRD_RANKING + 4 * i);
                csgo.ScrBrdWins[i] = BitConverter.ToInt32(scrbrdData, GameOffsets.CL_SCRBRD_WINS + 4 * i);
            }

            //Read players & entities
            if (csgo.Players == null)
                csgo.Players = new Player[MAX_PLAYERS];
            if (csgo.Entities == null)
                csgo.Entities = new Entity[MAX_ENTITIES - MAX_PLAYERS];

            int maxIndex = 2048;// = WinAPI.ReadInt32(handle, entityListAddress + 0x4);
            //maxIndex -= entityListAddress;
            //maxIndex /= GameOffsets.CL_ENTITY_SIZE;
            byte[] entityList = WinAPI.ReadMemory(handle, entityListAddress, maxIndex * GameOffsets.CL_ENTITY_SIZE);
            for (int i = 0; i < maxIndex; i++)
            {
                try
                {
                    int address = BitConverter.ToInt32(entityList, GameOffsets.CL_ENTITY_SIZE * i);
                    if (address != 0)
                        if (i < 64)
                        {
                            if (csgo.Players[i] == null)
                                csgo.Players[i] = new Player(address, radarAddress + GameOffsets.CL_RADAR_SIZE * i, i + 1);
                            else
                                csgo.Players[i].Update(address, radarAddress + GameOffsets.CL_RADAR_SIZE * i, i + 1);
                        }
                        else
                        {
                            if (csgo.Entities[i - csgo.Players.Length] == null)
                                csgo.Entities[i - csgo.Players.Length] = new Entity(address, radarAddress, i);
                            else
                                csgo.Entities[i - csgo.Players.Length].Update(address, radarAddress, i);

                            //if (!entityClasses.ContainsKey(csgo.Entities[i - csgo.Players.Length].ClassIDInt))
                            //    entityClasses.Add(csgo.Entities[i - csgo.Players.Length].ClassIDInt, csgo.Entities[i - csgo.Players.Length].Name);

                            if (!csgo.Entities[i - csgo.Players.Length].IsValid())
                                csgo.Entities[i - csgo.Players.Length] = null;
                            else
                                if (csgo.Entities[i - csgo.Players.Length].ClassID == ClassID.PlantedC4)
                                    c4Planted = true;
                        }
                } catch { }
            }

            //Get weaponID
            long weaponHandle = WinAPI.ReadInt32(handle, localAddress + GameOffsets.CL_LOCAL_ACTIVE_WEAPON);
            long weaponIDFirst = weaponHandle & 0xFFF;
            long weaponBase = WinAPI.ReadInt32(handle, entityListAddress + ((weaponIDFirst - 1) * 0x10));
            int weaponID = WinAPI.ReadInt32(handle, weaponBase + GameOffsets.CL_LOCAL_WEAPON_ID);
            float accuracyPenality = WinAPI.ReadFloat(handle, weaponBase + GameOffsets.CL_LOCAL_WEAPON_ACCURACYPENALITY);
            //int weaponShotsFired = WinAPI.ReadInt32(handle, localPlayer + GameOffsets.CL_LOCAL_WEAPON_SHOTS_FIRED);

            //Debug.WriteLine(accuracyPenality);
            ZoomLevel zoom = (ZoomLevel)WinAPI.ReadMemory(handle, weaponBase + GameOffsets.CL_LOCAL_WEAPON_ZOOM, 1)[0];
            bool isReloading = WinAPI.ReadMemory(handle, weaponBase + GameOffsets.CL_LOCAL_WEAPON_RELOAD, 1)[0] == 1;

            //Get clips
            long clip1 = WinAPI.ReadInt32(handle, dllClientAddress + GameOffsets.CL_RADAR_BASE);
            long clip2 = WinAPI.ReadInt32(handle, clip1 + GameOffsets.CL_WEAPON_OFFSET);
            int weaponClip1 = WinAPI.ReadInt32(handle, clip2 + GameOffsets.CL_WEAPON_AMMO_PRIM);
            int weaponClip2 = WinAPI.ReadInt32(handle, clip2 + GameOffsets.CL_WEAPON_AMMO_SEC);

            //Angles
            csgo.ViewAngles = ReadAngle(handle, enginePointer + GameOffsets.EN_VIEWANGLE_X);
            csgo.ViewOffset = ReadAngle(handle, localAddress + GameOffsets.CL_LOCAL_VIEWOFFSET);

            //overwrite data
            csgo.LocalPlayer = FindLocalPlayer(csgo.Players, localAddress);
            if (targetIndex > 0 && targetIndex <= 64)
                csgo.TargetPlayer = csgo.Players[targetIndex - 1];
            else
                csgo.TargetPlayer = null;
            csgo.TargetIndex = targetIndex;
            if (csgo.C4Planted != c4Planted)
            {
                csgo.C4Planted = c4Planted;
                csgo.C4PlantTime = Environment.TickCount;
            }
            csgo.AccuracyPenality = accuracyPenality;
            csgo.C4Timer = WinAPI.ReadFloat(handle, dllClientAddress + GameOffsets.CL_NETVAR_MPC4TIMER);
            csgo.ServerMap = WinAPI.ReadString(handle, dllClientAddress + GameOffsets.CL_SRV_BASE + GameOffsets.CL_SRV_MAP, 32, Encoding.ASCII);
            csgo.ServerIP = WinAPI.ReadString(handle, dllClientAddress + GameOffsets.CL_SRV_BASE + GameOffsets.CL_SRV_IP, 32, Encoding.ASCII);
            csgo.ServerName = WinAPI.ReadString(handle, dllClientAddress + GameOffsets.CL_SRV_BASE + GameOffsets.CL_SRV_Name, 32, Encoding.ASCII);
            csgo.IsReloading = isReloading;
            csgo.IsShooting = WinAPI.ReadMemory(handle, dllClientAddress + GameOffsets.CL_LOCAL_BUTTONS_ATTACK, 1)[0] == 5;
            csgo.ViewMatrix = viewMatrix;
            csgo.WeaponClip1 = weaponClip1;
            csgo.WeaponClip2 = weaponClip2;
            csgo.WeaponFireRate = WeaponHandler.Instance.GetWeaponFireRate(weaponID);
            csgo.WeaponName = WeaponHandler.Instance.GetWeaponName(weaponID);
            csgo.WeaponType = WeaponHandler.Instance.GetWeaponType(weaponID);
            csgo.FlashMaxAlpha = WinAPI.ReadFloat(handle, localAddress + GameOffsets.CL_LOCAL_FLASH_MAX_ALPHA);
            csgo.FlashMaxDuration = WinAPI.ReadFloat(handle, localAddress + GameOffsets.CL_LOCAL_FLASH_MAX_DURATION);
            //csgo.WeaponShotsFired = weaponShotsFired;
            csgo.ZoomLevel = zoom;

            csgo.FirstPersonSpectator = false;
            csgo.Spectators.Clear();
            if (csgo.LocalPlayer != null)
            {
                foreach (Player player in csgo.Players)
                {
                    if (player == null)
                        continue;
                    if (player.SpectatorTarget == csgo.LocalPlayer.Index)
                    {
                        csgo.Spectators.Add(player);
                        if (player.SpectatorView == Data.Enums.SpectatorView.Ego)
                            csgo.FirstPersonSpectator = true;
                    }
                }
            }

            if (csgo.LocalPlayer == null)
                return;

            if (csgo.GetValue<YesNo>("miscBunnyHopEnabled") == YesNo.Yes)
            {
                if (WinAPI.GetKeyDown(System.Windows.Forms.Keys.Space))
                {
                    int addrJmp = dllClientAddress + GameOffsets.CL_LOCAL_BUTTONS_JUMP;

                    //Test stuff
                    if (csgo.LocalPlayer.State == PlayerState.Jump)// && WinAPI.GetKeyDown(System.Windows.Forms.Keys.Up))
                    {
                        byte[] buffer = BitConverter.GetBytes(4);
                        WinAPI.WriteMemory(handle, addrJmp, buffer, buffer.Length);
                    }
                    else if (csgo.LocalPlayer.State == PlayerState.Stand)
                    {
                        byte[] buffer2 = BitConverter.GetBytes(5);
                        WinAPI.WriteMemory(handle, addrJmp, buffer2, buffer2.Length);
                    }
                }
            }
            if (csgo.GetValue<YesNo>("miscAutoPistolEnabled") == YesNo.Yes)
            {
                if (WinAPI.GetKeyDown(System.Windows.Forms.Keys.LButton) && csgo.WeaponType == WeaponType.Pistol)
                {
                    IntPtr bytesWritten = IntPtr.Zero;
                    int addrJmp = dllClientAddress + GameOffsets.CL_LOCAL_BUTTONS_ATTACK;
                    int val = WinAPI.ReadInt32(handle, addrJmp);
                    if (val == 0)
                    {
                        WinAPI.WriteMemory(handle, addrJmp, BitConverter.GetBytes(1), 4);
                    }
                    else
                    {
                        WinAPI.WriteMemory(handle, addrJmp, BitConverter.GetBytes(0), 4);
                    }
                }
            }

            //Glow-stuff
            int glowAddr = WinAPI.ReadInt32(handle, dllClientAddress + GameOffsets.CL_GLOWMANAGER);
            int objectCount = WinAPI.ReadInt32(handle, dllClientAddress + GameOffsets.CL_GLOWMANAGER + 4);
            GlowObjectDefinition[] glowObjects = new GlowObjectDefinition[objectCount];
            byte[] glowObjectData = WinAPI.ReadMemory(handle, glowAddr, GlowObjectDefinition.GetSize() * objectCount);

            for (int i = 0; i < glowObjects.Length; i++)
            {
                byte[] subData = new byte[GlowObjectDefinition.GetSize()];
                Array.Copy(glowObjectData, GlowObjectDefinition.GetSize() * i, subData, 0, GlowObjectDefinition.GetSize());

                glowObjects[i] = WinAPI.GetStructure<GlowObjectDefinition>(subData);
            }
            csgo.GlowObjects = glowObjects;
            tick++;
        }
        #region HELPERS
        private Player PlayerExists(int address)
        {
            CSGOImplementation csgo = ((CSGOImplementation)Program.GameImplementation);
            foreach (Player plr in csgo.Players)
                if (plr != null)
                    if (plr.Address == address)
                        return plr;
            return null;
        }
        public int GetGlowObjectByAddress(int address)
        {
            CSGOImplementation csgo = ((CSGOImplementation)Program.GameImplementation);
            if (csgo.GlowObjects == null)
                return -1;
            for (int i = 0; i < csgo.GlowObjects.Length; i++ )
            {
                if (csgo.GlowObjects[i].pEntity == address)
                    return i;
            }
            return -1;
        }
        private Vector3 ReadAngle(IntPtr handle, int address)
        {
            byte[] data = WinAPI.ReadMemory(handle, address, 12);
            return new Vector3(BitConverter.ToSingle(data, 0), BitConverter.ToSingle(data, 4), BitConverter.ToSingle(data, 8));
        }
        public void WriteViewAngles(Vector3 angles)
        {
            WriteAngles(angles, enginePointer + GameOffsets.EN_VIEWANGLE_X);
        }
        public void WriteAngles(Vector3 angles, int address)
        {
            byte[] data = new byte[12];
            Array.Copy(BitConverter.GetBytes(angles.X), 0, data, 0, 4);
            Array.Copy(BitConverter.GetBytes(angles.Y), 0, data, 4, 4);
            Array.Copy(BitConverter.GetBytes(angles.Z), 0, data, 8, 4);
            WinAPI.WriteMemory(scanner.Process.Handle, address, data, 12);
        }
        public void WriteGlowObject(GlowObjectDefinition def, int index)
        {
            byte[] data = def.GetBytes();
            byte[] writeData = new byte[GlowObjectDefinition.GetSize() - 14];
            Array.Copy(data, 4, writeData, 0, writeData.Length);
            int glowAddr = WinAPI.ReadInt32(scanner.Process.Handle, dllClientAddress + GameOffsets.CL_GLOWMANAGER);            
            WinAPI.WriteMemory(scanner.Process.Handle, glowAddr + GlowObjectDefinition.GetSize() * index + 4, writeData, writeData.Length);
        }
        private Player FindLocalPlayer(Player[] players, long address)
        {
            if (((CSGOImplementation)Program.GameImplementation).Players == null)
                return null;
            foreach (Player player in players)
                if (player == null)
                    continue;
                else
                    if (player.Address == address)
                        return player;
            return null;
        }
        #endregion
        #region OFFSET-SCANNER
        private void FindOffsets()
        {
            updatedOffsets = true;
            Program.PrintInfo("Beginning offset-scan...");
            dllClientAddress = Program.GameImplementation.GameController.GetModuleBaseAddressByName(@"bin\client.dll").ToInt32();
            dllEngineAddress = Program.GameImplementation.GameController.GetModuleBaseAddressByName(@"engine.dll").ToInt32();
            dllClientSize = Program.GameImplementation.GameController.GetModuleSize(@"bin\client.dll");
            dllEngineSize = Program.GameImplementation.GameController.GetModuleSize(@"engine.dll");

            if (dllClientAddress == 0 || dllClientSize == 0L)
            {
                Program.PrintError(" > NOPE: Module client.dll not found");
                Console.ReadKey();
                return;
            }
            if (dllEngineAddress == 0 || dllEngineSize == 0)
            {
                Program.PrintError(" > NOPE: Module engine.dll not found");
                Console.ReadKey();
                return;
            }
            scanner = new SigScanner(Program.GameController.Process, IntPtr.Zero, MAX_DUMP_SIZE);

            FindEntityList();
            FindLocalPlayer();
            FindRadarBase();
            FindScoreBoardBase();
            FindCrosshairIndex();
            FindServerBase();
            FindEnginePointer();
            FindAttack();
            FindJump();
            FindGlowObjectBase();
            FindEngineBuffer();
            FindViewMatrix();
            FindFlashMaxAlpha();
            FindFlashMaxDuration();
            Program.PrintInfo("Offset-scan finished");
        }
        private static void FindEntityList()
        {
            byte[] pattern = new byte[]{ 
                0x05, 0x00, 0x00, 0x00, 0x00, //add eax, client.dll+xxxx
                0xC1, 0xe9, 0x00,                   //shr ecx, x
                0x39, 0x48, 0x04                    //cmp [eax+04], ecx
                };
            string mask = MaskFromPattern(pattern);
            int address, val1, val2;

            address = FindAddress(pattern, 1, mask, dllClientAddress, dllClientSize);
            val1 = WinAPI.ReadInt32(scanner.Process.Handle, address);
            address = FindAddress(pattern, 7, mask, dllClientAddress, dllClientSize);
            val2 = WinAPI.ReadByte(scanner.Process.Handle, address);
            val1 = val1 + val2 - dllClientAddress;

            GameOffsets.CL_ENTITY_LIST = val1;
            PrintAddress("EntityList", val1);
        }
        private static void FindLocalPlayer()
        {
            byte[] pattern = new byte[]{ 
                0x8D, 0x34, 0x85, 0x00, 0x00, 0x00, 0x00,       //lea esi, [eax*4+client.dll+xxxx]
                0x89, 0x15, 0x00, 0x00, 0x00, 0x00,             //mov [client.dll+xxxx],edx
                0x8B, 0x41, 0x08,                               //mov eax,[ecx+08]
                0x8B, 0x48, 0x00                                //mov ecx,[eax+04]
                };
            string mask = MaskFromPattern(pattern);
            int address, val1, val2;

            address = FindAddress(pattern, 3, mask, dllClientAddress, dllClientSize);
            val1 = WinAPI.ReadInt32(scanner.Process.Handle, address);
            address = FindAddress(pattern, 18, mask, dllClientAddress, dllClientSize);
            val2 = WinAPI.ReadByte(scanner.Process.Handle, address);
            val1 += val2;
            val1 -= dllClientAddress;

            GameOffsets.CL_LOCAL_BASE_ENTITY = val1;
            localPlayer = val1;
            PrintAddress("LocalPlayer", val1);
        }
        private static void FindScoreBoardBase()
        {
            //Find pointer from engine.dll
            byte[] pattern = new byte[]{ 
                0x89, 0x4D, 0xF4,                   //mov [ebp-0C],ecx
                0x8B, 0x0D, 0x00, 0x00, 0x00, 0x00, //mov ecx,[engine.dll+xxxx]
                0x53,                               //push ebx
                0x56,                               //push esi
                0x57,                               //push edi
                0x8B, 0x01                          //moveax,[ecx]
                };
            string mask = MaskFromPattern(pattern);
            int address, pointer, offset;

            address = FindAddress(pattern, 5, mask, dllEngineAddress, dllEngineSize);
            pointer = WinAPI.ReadInt32(scanner.Process.Handle, address);
            pointer = pointer - dllEngineAddress;

            byte[] short1 = BitConverter.GetBytes((short)(0x0004));
            pattern = new byte[]{
                0xCC,                               //int 3
                0xCC,                               //int 3
                0x55,                               //push ebp
                0x8B, 0xEC,                         //mov ebp,esp
                0x8B, 0x45, 0x08,                   //mov eax,[ebp+08]
                0x8B, 0x44, 0xC1, 0x00,             //mov eax,[acx+eax*8+xx]
                0x5D,                               //pop ebp
                0xC2, short1[0], short1[1],         //ret 0004
                0xCC,                               //int 3
                0xCC                                //int 3
            };
            mask = MaskFromPattern(pattern);

            address = FindAddress(pattern, 11, mask, dllClientAddress, dllClientSize);
            offset = WinAPI.ReadByte(scanner.Process.Handle, address);
            //assume constant eax 46
            address = WinAPI.ReadInt32(scanner.Process.Handle, dllEngineAddress + pointer); //0x46 * offset + pointer;
            address = address + 0x46 * 8 + offset;
            address -= dllClientAddress;
            GameOffsets.CL_SCRBRD_BASE = address;
            PrintAddress("ScoreBoardBase", address);
        }
        private static void FindRadarBase()
        {
            byte[] int1 = BitConverter.GetBytes(0x00100000);
            byte[] pattern = new byte[]{ 
                0xA1, 0x00, 0x00, 0x00, 0x00,                   //mov eax,[client.dll+xxxx]
                0xA9, int1[0], int1[1], int1[2], int1[3],       //test eax, 00100000
                0x74, 0x06,                                     //je client.dll+2E78C6
                0x81, 0xCE, int1[0], int1[1], int1[2], int1[3]
                };
            string mask = MaskFromPattern(pattern);
            int address, val1, val2;

            address = FindAddress(pattern, 1, mask, dllClientAddress, dllClientSize);
            val1 = WinAPI.ReadInt32(scanner.Process.Handle, address);

            pattern = new byte[] {
                0x8B, 0x47, 0x00,                               //mov eax,[edi+xx]
                0x8B, 0x0C, 0xB0,                               //mov ecx,[eax+esi*4]
                0x80, 0x79, 0x0D, 0x00
            };
            mask = MaskFromPattern(pattern);

            address = FindAddress(pattern, 2, mask, dllClientAddress, dllClientSize);
            val2 = WinAPI.ReadByte(scanner.Process.Handle, address);

            address = val1 + val2 - dllClientAddress;

            GameOffsets.CL_RADAR_BASE = address;
            PrintAddress("RadarBase", address);
        }
        private static void FindViewMatrix()
        {
            byte[] pattern = {
                                0x53, 0x8B, 0xDC, 0x83, 0xEC, 0x08, 0x83, 0xE4,
                                0xF0, 0x83, 0xC4, 0x04, 0x55, 0x8B, 0x6B, 0x04,
                                0x89, 0x6C, 0x24, 0x04, 0x8B, 0xEC, 0xA1, 0x00,
                                0x00, 0x00, 0x00, 0x81, 0xEC, 0x98, 0x03, 0x00,
                                0x00
                                };
            int address = FindAddress(pattern, 0, "xxxxxxxxxxxxxxxxxxxxxxx????xxxxxx", dllClientAddress, dllClientSize);
            if (address == 0)
            {
                return;
            }
            address = WinAPI.ReadInt32(scanner.Process.Handle, address + 0x4EE);
            address -= dllClientAddress;
            address += 0x80;
            GameOffsets.CL_LOCAL_VIEWMATRIX = address;
            PrintAddress("ViewMatrix", address);
        }
        private static void FindCrosshairIndex()
        {
            if (localPlayer == 0x0)
            {
                Program.PrintError("LocalPlayer-offset is invalid, won't find xhair this way.");
                return;
            }

            byte[] pattern = new byte[]{ 
                0x56,                           //push esi
                0x57,                           //push edi
                0x8B, 0xF9,                     //mov edi,ecx
                0xC7, 0x87, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,  //mov [edi+xxxx], ????
                0x8B, 0x0D, 0x00, 0x00, 0x00, 0x0, //mov ecx,[client.dll+????]
                0x81, 0xF9, 0x00, 0x00, 0x00, 0x0, //cmp ecx, client.dll+????
                0x75, 0x07,                     //jne client.dll+????
                0xA1, 0x00, 0x00, 0x00, 0x00,   //mov eax,[client.dll+????]
                0xEB, 0x07                      //jmp client.dll+????
                };
            string mask = MaskFromPattern(pattern);
            int address, val1;

            address = FindAddress(pattern, 6, mask, dllClientAddress, dllClientSize);
            val1 = WinAPI.ReadInt32(scanner.Process.Handle, address);
            //val1 -= localPlayer;

            GameOffsets.CL_LOCAL_CROSSHAIR_TARGET = val1;
            PrintAddress("CrosshairIndex", val1);
        }
        private static void FindAttack()
        {
            byte[] int1 = BitConverter.GetBytes(0xFFFFFFFD);
            byte[] pattern = new byte[]{ 
                0x89, 0x15, 0x00, 0x00, 0x00, 0x00, //mov [client.dll+xxxx],edx
                0x8B, 0x15, 0x00, 0x00, 0x00, 0x00, //mov edx, [client.dll+????]
                0xF6, 0xC2, 0x03,                   //test dl, 03
                0x74, 0x03,                         //je client.dll+???? 
                0x83, 0xCE, 0x04,                   //or esi,04
                0xA8, 0x04,                         //test al,04
                0xBF, int1[0], int1[1], int1[2], int1[3]        //mov edi,FFFFFFFD
                };
            string mask = MaskFromPattern(pattern);
            int address, val1;

            address = FindAddress(pattern, 2, mask, dllClientAddress, dllClientSize);            //Find x1
            if (address != 0)
            {
                val1 = WinAPI.ReadInt32(scanner.Process.Handle, address);    //Read x1
                address = val1 - dllClientAddress;
            }

            GameOffsets.CL_LOCAL_BUTTONS_ATTACK = address;
            PrintAddress("attack", address);
        }
        private static void FindFlashMaxAlpha()
        {
            byte[] pattern = new byte[]{ 
                0x0F, 0x2F, 0xF2,
                0x0F, 0x87, 0x00, 0x00, 0x00, 0x00,
                0xF3, 0x0F, 0x10, 0xA1, 0x00, 0x00, 0x00, 0x00, //<<<
                0x0F, 0x2F, 0xCC,
                0x0F, 0x83
                };
            string mask = MaskFromPattern(pattern);
            int address, val1;

            address = FindAddress(pattern, 13, mask, dllClientAddress, dllClientSize);
            val1 = WinAPI.ReadInt32(scanner.Process.Handle, address);

            GameOffsets.CL_LOCAL_FLASH_MAX_ALPHA = val1;
            PrintAddress("FlashMaxAlpha", val1);
        }
        private static void FindFlashMaxDuration()
        {
            byte[] pattern = new byte[]{ 
                0x84, 0xC0,
                0x0F, 0x84, 0x00, 0x00, 0x00, 0x00,
                0xF3, 0x0F, 0x10, 0x87, 0x00, 0x00, 0x00, 0x00, //<<<
                0x0F, 0x57, 0xC9,
                0x0F, 0x2E, 0x86
                };
            string mask = MaskFromPattern(pattern);
            int address, val1;

            address = FindAddress(pattern, 12, mask, dllClientAddress, dllClientSize);
            val1 = WinAPI.ReadInt32(scanner.Process.Handle, address);

            GameOffsets.CL_LOCAL_FLASH_MAX_DURATION = val1;
            PrintAddress("FlashMaxDuration", val1);
        }
        private static void FindJump()
        {
            byte[] int1 = BitConverter.GetBytes(0xFFFFFFFD);
            byte[] pattern = new byte[]{ 
                0x89, 0x15, 0x00, 0x00, 0x00, 0x00, //mov [client.dll+xxxx],edx
                0x8B, 0x15, 0x00, 0x00, 0x00, 0x00, //mov edx,[client.dll+xxxx]
                0xF6, 0xC2, 0x03,                   //test dl, 03
                0x74, 0x03,                         //je client.dll+???? 
                0x83, 0xCE, 0x08, //or esi,08
                0xA8, 0x08,       //test al,08
                0xBF, int1[0], int1[1], int1[2], int1[3]        //mov edi,FFFFFFFD
                };
            string mask = MaskFromPattern(pattern);
            int address, val1;

            address = FindAddress(pattern, 2, mask, dllClientAddress, dllClientSize);            //Find x1
            if (address != 0)
            {
                val1 = WinAPI.ReadInt32(scanner.Process.Handle, address);    //Read x1
                address = val1 - dllClientAddress;
            }
            GameOffsets.CL_LOCAL_BUTTONS_JUMP = address;
            PrintAddress("jump", address);
        }
        private static void FindServerBase()
        {
            byte[] pattern = new byte[]{
                0x81, 0xC6, 0x00, 0x00, 0x00, 0x00,
                0x81, 0xFE, 0x00, 0x00, 0x00, 0x00,
                0x7C, 0xEB,
                0x8B, 0x0D, 0x00, 0x00, 0x00, 0x00, //<<<<
                0x5F,
                0x5E,
                0x85, 0xC9,
                0x74, 0x0F,
                0x8B, 0x01,
                0xFF, 0x50, 0x04,
                0xC7, 0x05
                };
            string mask = MaskFromPattern(pattern);
            int address, val1;

            address = FindAddress(pattern, 16, mask, dllClientAddress, dllClientSize);            //Find x1
            if (address != 0)
            {
                val1 = WinAPI.ReadInt32(scanner.Process.Handle, address);    //Read x1
                address = val1 - dllClientAddress;
            }

            GameOffsets.CL_SRV_BASE = address;
            PrintAddress("ServerBase", address);
        }
        private static void FindEnginePointer()
        {
            byte[] pattern = new byte[]{
                0xC2, 0x00, 0x00,
                0xCC,
                0xCC,
                0x8B, 0x0D, 0x00, 0x00, 0x00, 0x00, //<<<<
                0x33, 0xC0,
                0x83, 0xB9
                };
            string mask = MaskFromPattern(pattern);
            int address, val1;

            address = FindAddress(pattern, 7, mask, dllEngineAddress, dllEngineSize);            //Find x1
            if (address != 0)
            {
                val1 = WinAPI.ReadInt32(scanner.Process.Handle, address);    //Read x1
                address = val1 - dllEngineAddress;
            }

            GameOffsets.EN_ENGINE_POINTER = address;
            PrintAddress("EnginePointer", address);
        }
        private static void FindEngineBuffer()
        {
            byte[] pattern = new byte[]{
               0x66, 0xA1, 0x8C, 0x0B, 0xE3, 0x0F, 0x8B, 0xF2, 0x66, 0x89, 0x07, 0x8A, 0x02
            };
            string mask = "xx????xxxxxxx";
            int address, val1;

            address = FindAddress(pattern, 2, mask, dllEngineAddress, dllEngineSize);
            if (address != 0)
            {
                val1 = WinAPI.ReadInt32(scanner.Process.Handle, address);    //Read x1
                address = val1 - dllEngineAddress;
            }

            GameOffsets.EN_ENGINE_BUFFER = address;
            PrintAddress("EngineBuffer", address);
        }
        private static void FindMapName()
        {
            byte[] pattern = new byte[]{ 
               0x72, 0xEF,
               0xC6, 0x00, 0x00,
               0xB8, 0x00, 0x00, 0x00, 0x00,
               0x80, 0x3D, 0x00, 0x00, 0x00, 0x00, 0x00,
               0x74, 0x15,
               0x8A, 0x08,
               0x80, 0xF9
                };
            string mask = MaskFromPattern(pattern);
            int address, val1;

            address = FindAddress(pattern, 6, mask, dllEngineAddress, dllEngineSize);
            val1 = WinAPI.ReadInt32(scanner.Process.Handle, address);

            address = val1 - dllEngineAddress;

            GameOffsets.CL_LOCAL_CURRENT_MAP = address;
            PrintAddress("CurrentMap", address);
        }
        private static void FindGlowObjectBase()
        {
            byte[] pattern = new byte[]{ 
                0x8D, 0x8F, 0x00, 0x00, 0x00, 0x00,
                0xA1, 0x00, 0x00, 0x00, 0x00, //<<<<<
                0xC7, 0x04, 0x02, 0x00, 0x00, 0x00, 0x00,
                0x89, 0x35, 0x00, 0x00, 0x00, 0x00,
                0x8B, 0x51
                };
            string mask = MaskFromPattern(pattern);
            int address, val1;

            address = FindAddress(pattern, 7, mask, dllClientAddress, dllClientSize);            //Find x1
            if (address != 0)
            {
                val1 = WinAPI.ReadInt32(scanner.Process.Handle, address);    //Read x1
                address = val1 - dllClientAddress;
            }
            GameOffsets.CL_GLOWMANAGER = address;
            PrintAddress("GlowObjectBase", address);
        }
        private static string MaskFromPattern(byte[] pattern)
        {
            StringBuilder builder = new StringBuilder();
            foreach (byte data in pattern)
                if (data == 0x00)
                    builder.Append('?');
                else
                    builder.Append('x');
            return builder.ToString();
        }
        private static int FindAddress(byte[] pattern, int offset, string mask, int dllAddress, long dllSize)
        {
            int address = 0;
            for (int i = 0; i < dllSize && address == 0; i += (int)(MAX_DUMP_SIZE * 0.75))
            {
                scanner.Address = new IntPtr(dllAddress + i);
                address = scanner.FindPattern(pattern, mask, offset).ToInt32();
                scanner.ResetRegion();
            }

            return address;
        }
        private static void PrintAddress(string name, int value)
        {
            Program.PrintSideInfo(" {0} {1}", name.PadRight(16, ' '), string.Format("0x{0}", value.ToString("X").PadLeft(8, '0')));
        }
        #endregion
    }
}
