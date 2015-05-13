using laExternalMulti.Objects.Implementation.CSGO.Data.Enums;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.Data
{
    public class Entity
    {
        #region VARIABLES
        private long address, radarAddress;
        private int health = 100;
        private int index;
        private short ownerEntity;
        private int classIDInt;
        private ClassID classID;
        protected float x, y, z, pitch, yaw, roll, velx, vely, velz, baseVelx, baseVely, baseVelz, punchX, punchY, punchZ;
        private Team team;
        private Vector3 vector3, velocity, baseVelocity, viewAngle, punchVector;
        private Vector2 vector2;
        protected string name;
        private Skeleton skeleton;
        private PlayerState state;
        private SpectatorView spectatorView;
        private int spectatorTarget;
        public const float FOV_DEGREE = 60f;
        protected bool isDormant;
        protected bool isSpotted;
        private LifeState lifeState;
        private int spottedMask;
        #endregion

        #region PROPERTIES
        public int Index { get { return index; } }
        public short OwnerEntity { get { return ownerEntity; } }
        public long Address { get { return address; } }
        public long RadarAddress { get { return radarAddress; } }
        public int Health { get { return health; } }
        public float X { get { return x; } }
        public float Y { get { return y; } }
        public float Z { get { return z; } }
        public float Pitch { get { return pitch; } }
        public float Yaw { get { return yaw; } }
        public float Roll { get { return roll; } }
        public Team InTeam { get { return team; } }
        public LifeState LifeState { get { return lifeState; } }
        public PlayerState State { get { return state; } }
        public SpectatorView SpectatorView { get { return spectatorView; } }
        public int SpectatorTarget { get { return spectatorTarget; } }
        public ClassID ClassID { get { return classID; } }
        public int ClassIDInt { get { return classIDInt; } }
        public Vector3 Vector3 { get { return vector3; } }
        public Vector2 Vector2 { get { return vector2; } }
        public Vector3 Velocity { get { return velocity; } }
        public Vector3 BaseVelocity { get { return velocity; } }
        public Vector3 ViewAngle { get { return viewAngle; } }
        public Vector3 PunchVector { get { return punchVector; } }
        public string Name { get { return name; } }
        public Skeleton Skeleton { get { return skeleton; } }
        public bool IsDormant { get { return isDormant; } }
        public bool IsSpotted { get { return isSpotted; } set { isSpotted = value; } }
        public int SpottedMask { get { return spottedMask; } }
        #endregion

        #region METHODS
        public Entity(long address, long radarAddress, int id)
        {
            Update(address, radarAddress, id);
        }

        public Entity(Entity copyFrom)
        {
            this.address = copyFrom.Address;
            this.health = copyFrom.Health;
            this.x = copyFrom.X;
            this.y = copyFrom.Y;
            this.z = copyFrom.Z;
            this.pitch = copyFrom.Pitch;
            this.yaw = copyFrom.Yaw;
            this.roll = copyFrom.Roll;
            this.viewAngle = new Vector3(pitch, yaw, roll);
            this.team = copyFrom.InTeam;
            this.vector3 = new Vector3(x, y, z);
            this.vector2 = new Vector2(x, y);
            this.velx = copyFrom.velx;
            this.vely = copyFrom.vely;
            this.velz = copyFrom.velz;
            this.velocity = new Vector3(velx, vely, velz);
            this.punchX = copyFrom.punchX;
            this.punchY = copyFrom.punchY;
            this.punchZ = copyFrom.punchZ;
            this.punchVector = new Vector3(punchX, punchY, punchZ);
            this.name = (string)copyFrom.name.Clone();
            if (copyFrom.skeleton != null)
                this.skeleton = (Skeleton)copyFrom.Skeleton.Clone();
            this.classID = copyFrom.ClassID;
        }
        #endregion

        #region METHODS
        public virtual void Update(long address, long radarAddress, int id)
        {
            this.index = id;
            IntPtr handle = Program.GameImplementation.GameController.Process.Handle;
            this.address = address;
            this.radarAddress = radarAddress;

            byte[] entityData = WinAPI.ReadMemory(handle, address, 6500);

            int vt = BitConverter.ToInt32(entityData, 0x8);
            int fn = WinAPI.ReadInt32(handle, vt + 2 * 0x4);
            int cls = WinAPI.ReadInt32(handle, fn + 0x1);
            this.classIDInt = WinAPI.ReadInt32(handle, cls + 20);
            this.classID = (ClassID)this.classIDInt;

            int namePointer = WinAPI.ReadInt32(handle, cls + 8);
            byte[] nameData = WinAPI.ReadMemory(handle, namePointer, 32);
            this.name = WinAPI.ReadString(handle, namePointer, 32, Encoding.ASCII); //Encoding.ASCII.GetString(nameData);

            if (this.name.Length > 0)
            {
                if (this.name[0] == 'C' && this.name != "C4")
                    this.name = this.name.Substring(1, this.name.Length - 1);
                if (this.name.StartsWith("Weapon"))
                {
                    this.classID = Enums.ClassID.Weapon;
                    this.name = this.name.Substring(6, this.name.Length - 6);
                }
                else if (this.name == "HEGrenade" || this.name == "Incendiary" || this.name == "Flash" || this.name == "Molotov" || this.name == "Decoy")
                {
                    this.classID = Enums.ClassID.Weapon;
                }
                else if (this.classID == Enums.ClassID.AK47 || this.classID == Enums.ClassID.DEagle)
                {
                    this.classID = Enums.ClassID.Weapon;
                }
            }
            if (!(
                    classID == Enums.ClassID.CSPlayer ||
                    classID == Enums.ClassID.Hostage ||
                    classID == Enums.ClassID.Chicken ||
                    classID == Enums.ClassID.Weapon ||
                    classID == Enums.ClassID.C4 ||
                    classID == Enums.ClassID.PlantedC4 ||
                    classID == Enums.ClassID.SmokeGrenade)
                )
                return;

            int newHealth = BitConverter.ToInt32(entityData, GameOffsets.CL_ENTITY_HEALTH);
            if (newHealth != health)
                HealthChanged(health, newHealth);
            this.health = newHealth;
            this.ownerEntity = BitConverter.ToInt16(entityData, GameOffsets.CL_ENTITY_OWNER_ENTITY);
            this.x = BitConverter.ToSingle(entityData, GameOffsets.CL_ENTITY_X);
            this.y = BitConverter.ToSingle(entityData, GameOffsets.CL_ENTITY_Y);
            this.z = BitConverter.ToSingle(entityData, GameOffsets.CL_ENTITY_Z);
            this.pitch = BitConverter.ToSingle(entityData, GameOffsets.CL_ENTITY_PITCH);
            this.yaw = BitConverter.ToSingle(entityData, GameOffsets.CL_ENTITY_YAW);
            this.roll = BitConverter.ToSingle(entityData, GameOffsets.CL_ENTITY_ROLL);
            this.viewAngle = new Vector3(pitch, yaw, roll);
            this.team = (Team)BitConverter.ToInt32(entityData, GameOffsets.CL_ENTITY_TEAM);//BitConverter.ToInt32(entityData, GameOffsets.CL_ENTITY_TEAM);
            this.vector3 = new Vector3(x, y, z);
            this.vector2 = new Vector2(x, y);
            this.velx = BitConverter.ToSingle(entityData, GameOffsets.CL_ENTITY_VELOCITY_X);
            this.vely = BitConverter.ToSingle(entityData, GameOffsets.CL_ENTITY_VELOCITY_Y);
            this.velz = BitConverter.ToSingle(entityData, GameOffsets.CL_ENTITY_VELOCITY_Z);
            this.baseVelx = BitConverter.ToSingle(entityData, GameOffsets.CL_ENTITY_BASE_VELOCITY_X);
            this.baseVely = BitConverter.ToSingle(entityData, GameOffsets.CL_ENTITY_BASE_VELOCITY_Y);
            this.baseVelz = BitConverter.ToSingle(entityData, GameOffsets.CL_ENTITY_BASE_VELOCITY_Z);
            this.punchX = BitConverter.ToSingle(entityData, GameOffsets.CL_ENTITY_PUNCHVEC);
            this.punchY = BitConverter.ToSingle(entityData, GameOffsets.CL_ENTITY_PUNCHVEC + 0x4);
            this.punchZ = BitConverter.ToSingle(entityData, GameOffsets.CL_ENTITY_PUNCHVEC + 0x8);
            this.punchVector = new Vector3(punchX, punchY, punchZ);
            this.state = (PlayerState)entityData[GameOffsets.CL_ENTITY_STATE];//BitConverter.ToInt32(entityData, GameOffsets.CL_ENTITY_STATE);
            this.spectatorView = (SpectatorView)entityData[GameOffsets.CL_ENTITY_SPECTATOR_VIEW];//BitConverter.ToInt32(entityData, GameOffsets.CL_ENTITY_SPECTATOR_VIEW);
            this.spectatorTarget = entityData[GameOffsets.CL_ENTITY_SPECTATOR_PLAYER];
            this.velocity = new Vector3(velx, vely, velz);
            this.baseVelocity = new Vector3(baseVelx, baseVely, baseVelz);
            this.isDormant = entityData[GameOffsets.CL_ENTITY_DORMANT] == 1;
            this.isSpotted = entityData[GameOffsets.CL_ENTITY_SPOTTED] == 1;
            this.spottedMask = BitConverter.ToInt32(entityData, GameOffsets.CL_ENTITY_SPOTTED_MASK);
            this.lifeState = (LifeState)entityData[GameOffsets.CL_ENTITY_LIFESTATE];//BitConverter.ToInt32(entityData, GameOffsets.CL_ENTITY_LIFESTATE);
            if (this.skeleton == null)
                this.skeleton = new Skeleton(
                    BitConverter.ToInt32(entityData, GameOffsets.CL_ENTITY_BONEMATRIX),
                    !(this.classID == Enums.ClassID.Chicken || this.classID == Enums.ClassID.Hostage || this.classID == Enums.ClassID.CSPlayer)
                    );
            else
                this.skeleton.Update(BitConverter.ToInt32(entityData, GameOffsets.CL_ENTITY_BONEMATRIX));
        }
        public virtual bool IsValid()
        {
            return
                !(x == 0f && y == 0f && z == 0f) &&
                this.Address != 0L &&
            (
                this.classID == Enums.ClassID.Chicken ||
                this.classID == Enums.ClassID.Hostage ||
                this.classID == Enums.ClassID.CSPlayer ||
                this.classID == Enums.ClassID.Weapon ||
                this.classID == Enums.ClassID.C4 ||
                this.classID == Enums.ClassID.PlantedC4
            );
        }
        public bool SeenBy(int index)
        {
            return (spottedMask & (0x1 << index)) != 0;
        }
        public bool SeenBy(Entity ent)
        {
            return SeenBy(ent.Index - 1);
        }
        protected virtual void HealthChanged(int oldHealth, int newHealth) { this.health = newHealth; }

        public float DistanceToOtherEntityInMetres(Entity other)
        {
            return Geometry.GetDistanceToPoint(this.Vector3, other.Vector3) * 0.01905f;
        }
        #endregion

        public object Clone()
        {
            return new Entity(this);
        }
    }
}
