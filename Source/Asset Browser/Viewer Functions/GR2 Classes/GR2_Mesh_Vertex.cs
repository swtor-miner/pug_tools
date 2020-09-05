using System;
using System.IO;

namespace FileFormats
{
    public class GR2_Mesh_Vertex
    {
        public float X = 0;
        public float Y = 0;
        public float Z = 0;

        public float boneIndex1;
        public float boneIndex2;
        public float boneIndex3;
        public float boneIndex4;

        public float boneWeight1;
        public float boneWeight2;
        public float boneWeight3;
        public float boneWeight4;

        public float normX;
        public float normY;
        public float normZ;
        public float normW;

        public float tanX;
        public float tanY;
        public float tanZ;
        public float tanW;

        public SlimDX.Half texHalfU;
        public SlimDX.Half texHalfV;

        public float texU;
        public float texV;

        public GR2_Mesh_Vertex(BinaryReader br, ushort bitFlag2)
        {
            if ((bitFlag2 & 0x1) != 0x1)
            {
                throw new GR2_Vertex_Size_Exception("Invalid Vertex Size");
            }
            else
            {
                X = br.ReadSingle();
                Y = br.ReadSingle();
                Z = br.ReadSingle();
            }
            if ((bitFlag2 & 0x100) == 0x100)
            {
                boneWeight1 = File_Helpers.ByteToFloat(br.ReadByte());
                boneWeight2 = File_Helpers.ByteToFloat(br.ReadByte());
                boneWeight3 = File_Helpers.ByteToFloat(br.ReadByte());
                boneWeight4 = File_Helpers.ByteToFloat(br.ReadByte());
                boneIndex1 = File_Helpers.ByteToFloat(br.ReadByte());
                boneIndex2 = File_Helpers.ByteToFloat(br.ReadByte());
                boneIndex3 = File_Helpers.ByteToFloat(br.ReadByte());
                boneIndex4 = File_Helpers.ByteToFloat(br.ReadByte());
            }
            if ((bitFlag2 & 0x2) == 0x2)
            {
                normX = br.ReadByte(); // File_Helpers.ByteToNormal(br.ReadByte());
                normY = br.ReadByte(); // File_Helpers.ByteToNormal(br.ReadByte());
                normZ = br.ReadByte(); // File_Helpers.ByteToNormal(br.ReadByte());
                normW = br.ReadByte(); // File_Helpers.ByteToNormal(br.ReadByte());

                tanX = br.ReadByte(); // File_Helpers.ByteToNormal(br.ReadByte());
                tanY = br.ReadByte(); // File_Helpers.ByteToNormal(br.ReadByte());
                tanZ = br.ReadByte(); // File_Helpers.ByteToNormal(br.ReadByte());
                tanW = br.ReadByte(); // File_Helpers.ByteToNormal(br.ReadByte());
            }
            if ((bitFlag2 & 0x10) == 0x10)
            {
                br.ReadByte();
                br.ReadByte();
                br.ReadByte();
                br.ReadByte();
            }
            if ((bitFlag2 & 0x20) == 0x20)
            {
                texHalfU.RawValue = br.ReadUInt16();
                texHalfV.RawValue = br.ReadUInt16();
                float[] texFloats = SlimDX.Half.ConvertToFloat(new SlimDX.Half[] { texHalfU, texHalfV });
                texU = texFloats[0];
                texV = texFloats[1];
            }
            if ((bitFlag2 & 0x40) == 0x40)
            {
                br.ReadByte();
                br.ReadByte();
                br.ReadByte();
                br.ReadByte();
            }
            if ((bitFlag2 & 0x80) == 0x80)
            {
                br.ReadByte();
                br.ReadByte();
                br.ReadByte();
                br.ReadByte();
            }
        }
    }


    public class GR2_Vertex_Size_Exception : Exception
    {
        public GR2_Vertex_Size_Exception(string message) : base(message) { }
    }
}
