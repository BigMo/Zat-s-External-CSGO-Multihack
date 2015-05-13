using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.Data.BSP.Structs
{
    public struct Header
    {
        public int ident;
        public int version;
        public Lump[] lumps;
        public int mapRevision;
    }
}
