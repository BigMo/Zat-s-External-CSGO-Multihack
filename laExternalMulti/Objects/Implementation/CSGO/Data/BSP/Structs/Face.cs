using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.Data.BSP.Structs
{
    public struct Face
    {
        public ushort planeNumber;
        public byte side;
        public byte onNode;
        public int firstEdge;
        public short numEdges;
        public short texinfo;
        public short dispinfo;
        public short surfaceFogVolumeID;
        public byte[] styles;
        public int lightOffset;
        public float area;
        public int[] LightmapTextureMinsInLuxels;
        public int[] LightmapTextureSizeInLuxels;
        public int originalFace;
        public ushort numPrims;
        public ushort firstPrimID;
        public uint smoothingGroups;
    }
}
