using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.Data
{
    public class Skeleton : ICloneable
    {
        #region VARIABLES
        public Vector3
            HeadEnd,
            Head,
            Neck,
            Spine1,
            Spine2,
            Spine3,
            Spine4,
            Spine5,
            LeftHand,
            LeftElbow,
            LeftShoulder,
            RightShoulder,
            RightElbow,
            RightHand,
            LeftToe,
            LeftFoot,
            LeftKnee,
            LeftHip,
            RightHip,
            RightKnee,
            RightFoot,
            RightToe,
            Weapon1,
            Weapon2;
        public Vector3[] Arms, Legs, Spine;
        public Vector3[] AllBones;
        #endregion

        #region CONSTRUCTOR
        public Skeleton(long boneMatrixAddress, bool zero = false)
        {
            if (!zero)
            {
                Update(boneMatrixAddress);
            }
            else
            {
                Head = Vector3.Zero;
                Neck = Vector3.Zero;
                Spine1 = Vector3.Zero;
                Spine2 = Vector3.Zero;
                Spine3 = Vector3.Zero;
                Spine4 = Vector3.Zero;
                Spine5 = Vector3.Zero;
                LeftHand = Vector3.Zero;
                LeftElbow = Vector3.Zero;
                LeftShoulder = Vector3.Zero;
                RightShoulder = Vector3.Zero;
                RightElbow = Vector3.Zero;
                RightHand = Vector3.Zero;
                LeftToe = Vector3.Zero;
                LeftFoot = Vector3.Zero;
                LeftKnee = Vector3.Zero;
                LeftHip = Vector3.Zero;
                RightHip = Vector3.Zero;
                RightKnee = Vector3.Zero;
                RightFoot = Vector3.Zero;
                RightToe = Vector3.Zero;
            }
            Arms = new Vector3[] { LeftHand, LeftElbow, LeftShoulder, Spine5, RightShoulder, RightElbow, RightHand };
            Legs = new Vector3[] { /*LeftToe, */LeftFoot, LeftKnee, LeftHip, Spine1, RightHip, RightKnee, RightFoot/*, RightToe */};
            Spine = new Vector3[] { Spine1, Spine2, Spine3, Spine4, Spine5, Neck, Head };
        }

        public Skeleton(Skeleton copyFrom)
        {
            Neck = copyFrom.Neck;
            Spine1 = copyFrom.Spine1;
            Spine2 = copyFrom.Spine2;
            Spine3 = copyFrom.Spine3;
            Spine4 = copyFrom.Spine4;
            Spine5 = copyFrom.Spine5;
            LeftHand = copyFrom.LeftHand;
            LeftElbow = copyFrom.LeftElbow;
            LeftShoulder = copyFrom.LeftShoulder;
            RightShoulder = copyFrom.RightShoulder;
            RightElbow = copyFrom.RightElbow;
            RightHand = copyFrom.RightHand;
            LeftFoot = copyFrom.LeftFoot;
            LeftKnee = copyFrom.LeftKnee;
            LeftHip = copyFrom.LeftHip;
            RightHip = copyFrom.RightHip;
            RightKnee = copyFrom.RightKnee;
            RightFoot = copyFrom.RightFoot;
        }
        #endregion

        public void Update(long boneMatrixAddress)
        {
            byte[] boneData = WinAPI.ReadMemory(Program.GameImplementation.GameController.Process.Handle, boneMatrixAddress, GameOffsets.CL_ENTITY_BONE_SIZE * 64);
            Head = GetBone(boneData, 11);
            Neck = GetBone(boneData, 10);
            HeadEnd = Neck + Vector3.UnitZ * 8f;
            Spine1 = GetBone(boneData, 1);
            Spine2 = GetBone(boneData, 2);
            Spine3 = GetBone(boneData, 3);
            Spine4 = GetBone(boneData, 4);
            Spine5 = GetBone(boneData, 5);
            LeftHand = GetBone(boneData, 21);
            LeftElbow = GetBone(boneData, 31);
            LeftShoulder = GetBone(boneData, 36);
            RightShoulder = GetBone(boneData, 37);
            RightElbow = GetBone(boneData, 38);
            RightHand = GetBone(boneData, 15);
            LeftToe = GetBone(boneData, 38);
            LeftFoot = GetBone(boneData, 28);
            LeftKnee = GetBone(boneData, 27);
            LeftHip = GetBone(boneData, 26);
            RightHip = GetBone(boneData, 23);
            RightKnee = GetBone(boneData, 24);
            RightFoot = GetBone(boneData, 25);
            RightToe = GetBone(boneData, 37);
            Weapon1 = GetBone(boneData, 16);
            Weapon2 = GetBone(boneData, 21);

            if (Head.Z < Neck.Z)
                Head = Neck;
            Arms = new Vector3[] { LeftHand, LeftElbow, LeftShoulder, Spine5, RightShoulder, RightElbow, RightHand };
            Legs = new Vector3[] { /*LeftToe, */LeftFoot, LeftKnee, LeftHip, Spine1, RightHip, RightKnee, RightFoot/*, RightToe */};
            Spine = new Vector3[] { Spine1, Spine2, Spine3, Spine4, Spine5 };
            //AllBones = new Vector3[64];
            //for (int i = 0; i < AllBones.Length; i++)
            //    AllBones[i] = GetBone(boneData, i);
        }

        private Vector3 GetBone(byte[] boneData, int index)
        {

            Vector3 returnVal = new Vector3();
            returnVal.X = BitConverter.ToSingle(boneData, index * GameOffsets.CL_ENTITY_BONE_SIZE + GameOffsets.CL_ENTITY_BONE_X);
            returnVal.Y = BitConverter.ToSingle(boneData, index * GameOffsets.CL_ENTITY_BONE_SIZE + GameOffsets.CL_ENTITY_BONE_Y);
            returnVal.Z = BitConverter.ToSingle(boneData, index * GameOffsets.CL_ENTITY_BONE_SIZE + GameOffsets.CL_ENTITY_BONE_Z);
            return returnVal;
        }

        public object Clone()
        {
            return new Skeleton(this);
        }

        public Vector3 GetBone(AimBone bone)
        {
            switch (bone)
            {
                case AimBone.Head:
                    return Head;
                case AimBone.Neck:
                    return Neck;
                case AimBone.Torso:
                    return Spine3;
                case AimBone.Hip:
                    return Spine1;
                case AimBone.Knees:
                    return LeftKnee + (RightKnee - LeftKnee) / 2f;
                case AimBone.Feet:
                    return LeftFoot + (RightFoot - LeftFoot) / 2f;
                default:
                    return Neck;
            }
        }
    }
}
