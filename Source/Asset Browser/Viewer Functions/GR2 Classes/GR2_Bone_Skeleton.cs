using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileFormats
{
    public class GR2_Bone_Skeleton
    {
        public uint offsetBoneName = 0;
        public int boneIndex = 0;
        public int parentBoneIndex = 0;
        public string boneName = "";

        public SlimDX.Matrix parent;
        public SlimDX.Matrix root;

        public GR2_Bone_Skeleton(BinaryReader br, int index)
        {
            this.offsetBoneName = br.ReadUInt32();
            this.parentBoneIndex = br.ReadInt32();

            this.parent = File_Helpers.ReadMatrix(br, true);
            this.root = File_Helpers.ReadMatrix(br, true);

            this.boneName = File_Helpers.ReadString(br, this.offsetBoneName);
            this.boneIndex = index;
        }

    }
}
