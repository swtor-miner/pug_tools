using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileFormats
{
    public class GR2_Mesh_Bone
    {
        public uint offsetName = 0;
        public string boneName = "";
        public float unknown1 = 0;
        public float unknown2 = 0;
        public float unknown3 = 0;
        public float unknown4 = 0;
        public float unknown5 = 0;
        public float unknown6 = 0;

        public GR2_Mesh_Bone(BinaryReader br)
        {
            this.offsetName = br.ReadUInt32();
            this.unknown1 = br.ReadSingle();
            this.unknown2 = br.ReadSingle();
            this.unknown3 = br.ReadSingle();
            this.unknown4 = br.ReadSingle();
            this.unknown5 = br.ReadSingle();
            this.unknown6 = br.ReadSingle();
            this.boneName = File_Helpers.ReadString(br, this.offsetName);
        }
    }
}
