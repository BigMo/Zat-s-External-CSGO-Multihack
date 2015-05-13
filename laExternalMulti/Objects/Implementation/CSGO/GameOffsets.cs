using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO
{
    class GameOffsets
    {
        //Basic
        public static int CL_ENTITY_LIST =  0x4A10384; //0x4A10364; //0x4A0B0C4; //0x4A0A0C4; //0x4A0A0C4; //0x4A090A4; //0x4A09064; //0x4A01B54; //0x4A00AD4; //0x49F37B4; //0x49FFA64;
        public static int CL_ENTITY_SIZE = 16;
        public static int CL_LOCAL_ACTIVE_WEAPON = 0x12C0;
        public static int CL_LOCAL_BASE_ENTITY = 0xA6DA14; //0xA68A14; //0xA67A14; //0xA67A44; //0xA66A44; //0xA66A24; //0xA5F7F4; //0xA5E7C4; //0xA5E7C4; //0xA5D7C4; //0xA58734;
        public static int CL_LOCAL_CURRENT_MAP = 0x4A4F68C; //ASCII
        public static int CL_LOCAL_BUTTONS_ATTACK = 0x2E7D034; //0x2E7C034; //0x2E7B014; //0x2E73A94;
        public static int CL_LOCAL_BUTTONS_JUMP = 0x00; //0x2E7B008;
        public static int CL_LOCAL_CROSSHAIR_TARGET = 0x23F8; //0x23F0; //0x23DC;
        public static int CL_LOCAL_VIEWMATRIX = 0x04A05584; //0x04A04564; //0x4A03564; //0x4A03584; //0x4A05894;//0x4A05914; //0x4A058D4;//0x4A058B4; //0x4A00614; //0x49FF614; //0x49FE5F4; //0x49FE5B4; //0x49F70A4; //0x49F6024; //0x49F4FB4;
        public static int CL_LOCAL_WEAPON_ACCURACYPENALITY = 0x1668;
        public static int CL_LOCAL_WEAPON_ID = 0x1684;
        public static int CL_LOCAL_WEAPON_RELOAD = 0x15F9;
        public static int CL_LOCAL_WEAPON_ZOOM = 0x16D8;
        public static int CL_LOCAL_WEAPON_SHOTS_FIRED = 0x1d58;
        public static int CL_LOCAL_VIEWOFFSET = 0x104;
        public static int CL_LOCAL_FLASH_MAX_ALPHA = 0x1D9C;
        public static int CL_LOCAL_FLASH_MAX_DURATION = 0x1DA0;

        //Clip
        public static int CL_WEAPON_OFFSET = 0xC;
        public static int CL_WEAPON_AMMO_PRIM = 0x94;
        public static int CL_WEAPON_AMMO_SEC = 0x98;
        public static int CL_WEAPON_KILLSTREAK = 0xB0;

		//Scoreboard
        public static int CL_SCRBRD_BASE = 0x4A305DC; //0x4A305BC; //0x4A2B31C; //0x4A2A31C; //0x4A292FC; //0x4A292BC; //0x4A21DAC;

        public static int CL_SCRBRD_KILLS = 0xBDC;
        public static int CL_SCRBRD_ASSISTS = 0xCE0;
        public static int CL_SCRBRD_DEATHS = 0xDE4;
        public static int CL_SCRBRD_HEALTH = 0x1178;
        public static int CL_SCRBRD_ARMOR = 0x182C;
        public static int CL_SCRBRD_SCORE = 0x1930;
        public static int CL_SCRBRD_RANKING = 0x1A34;
        public static int CL_SCRBRD_WINS = 0x1B38;
        public static int CL_SCRBRD_CLANTAGS = 0x411C; //16 bytes ASCII
        public static string[] CL_SCRBRD_RANKS = new string[]
        {
				//Arrows
                "Silver I",
                "Silver II",
                "Silver III",
                "Silver IV",
                "Silver Elite",
                "Silver Elite Master",
        
				//AKs
                "Gold Nova I",
                "Gold Nova II",
                "Gold Nova III",
				
				//Stars
                "Gold Nova Master",
                "Master Guardian I",
                "Master Guardian II", 
                "Master Guardian Elite",
				
				//Rest
                "Distinguished Master Guardian",
                "Legendary Eagle",
                "Legendary Eagle Master",
                "Supreme Master First Class",
                "The Global Elite"
        };
        //Round-data
        public static int CL_RND_BASE = 0x4A3FCDC; //0x4A3ECDC;
        public static int CL_RND_OFFSET = 0x3C;

        public static int CL_RND_WINSCT = 0x78;
        public static int CL_RND_WINST = 0x74;
        public static int CL_RND_MAPTIME = 0x10C8;

        //Server
        public static int CL_SRV_BASE = 0x4A551C0;
        //Offsets: Size = 0x104, length = 32, ASCII
        public static int CL_SRV_TIME = 0x20;
        public static int CL_SRV_IP =   0x6D8;
        public static int CL_SRV_MAP =  0x7DC;
        public static int CL_SRV_Name = 0x8E0;

        //Radar
        public static int CL_RADAR_BASE = 0x4A44F8C; //0x4A44F6C; //0x4A3FCCC; //0x4A3ECCC; //0x4A3DC9C; //0x4A3DC5C; //0x4A365CC; //0x4A3554C; //0x62F88; //0x4A344DC;
        public static int CL_RADAR_OFFSET = 0x50; //0C?
        public static int CL_RADAR_SIZE = 0x1E0;

        //Player
        public static int CL_ENTITY_ACCOUNT = 0x2374;
        public static int CL_ENTITY_ACTIVE_WEAPON = 0x12C0;
        public static int CL_ENTITY_BASE_VELOCITY_X = 0x11C;
        public static int CL_ENTITY_BASE_VELOCITY_Y = 0x11E;
        public static int CL_ENTITY_BASE_VELOCITY_Z = 0x120;

        public static int CL_ENTITY_BONE_SIZE = 0x30;   //48 bytes (16*4 bytes)
        public static int CL_ENTITY_BONE_X = 0x0C;      //12 = 3
        public static int CL_ENTITY_BONE_Y = 0x1C;      //28 = 7
        public static int CL_ENTITY_BONE_Z = 0x2C;      //44 = 11

        public static int CL_ENTITY_BONEMATRIX = 0xA78;
        public static int CL_ENTITY_DORMANT = 0xE9;
        public static int CL_ENTITY_HEALTH = 0xFC;
        public static int CL_ENTITY_LIFESTATE = 0x25B;
        public static int CL_ENTITY_OWNER_ENTITY = 0x148;
        public static int CL_ENTITY_PITCH = 0x189C + 12;
        public static int CL_ENTITY_PUNCHVEC = 0x13E8; //0x13DC;
        public static int CL_ENTITY_RADARBASENAME = 0x204;
        public static int CL_ENTITY_ROLL = 0x18A4 + 12;
        public static int CL_ENTITY_SPECTATOR_PLAYER = 0x1730;
        public static int CL_ENTITY_SPECTATOR_VIEW = 0x171C;
        public static int CL_ENTITY_SPOTTED = 0x935;
        public static int CL_ENTITY_SPOTTED_MASK = 0x978;
        public static int CL_ENTITY_STATE = 0x100;
        public static int CL_ENTITY_TEAM = 0xF0;
        public static int CL_ENTITY_VELOCITY_X = 0x110;
        public static int CL_ENTITY_VELOCITY_Y = 0x114;
        public static int CL_ENTITY_VELOCITY_Z = 0x118;
        public static int CL_ENTITY_X = 0xA0;
        public static int CL_ENTITY_Y = 0xA4;
        public static int CL_ENTITY_YAW = 0x18A0 + 12;
        public static int CL_ENTITY_Z = 0xA8;

        //Weapon
        public static int CL_WEAPON_OWNER = 0x1594;

        //Glow-stuff
        public static int CL_GLOWMANAGER = 0x00;

        //Vars
        public static int CL_NETVAR_MPC4TIMER = 0xA829E4; //0xA7DA3C; //0xA7CA3C; //0xA7CA18; //0xA7BA14; //0xA7B9F8; //0xA746BC;
        public static int CL_NETVAR_MPFRIENDLYFIRE = 0xA5FC50;
        public static int CL_NETVAR_MPMAXROUNDS = 0xA68910;
        public static int CL_NETVAR_CLSHOWFPS = 0xA68EE0;

        //Engine
        public static int EN_ENGINE_POINTER = 0x55A434;
        public static int EN_SIGNONSTATE = 0xE8;
        public static int EN_ENGINE_BUFFER = 0x00490B8C;
        public static int EN_VIEWANGLE_X = 0x4CE0;
        public static int EN_VIEWANGLE_Y = 0x4CE4;
        public static int EN_VIEWANGLE_Z = 0x4CE8;


        //Misc
        public static int CL_SMOKE_SPAWN_TIME = 0xACC;
    }
}
