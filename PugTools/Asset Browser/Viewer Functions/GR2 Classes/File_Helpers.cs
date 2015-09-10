using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SlimDX;

namespace FileFormats
{
    public static class File_Helpers
    {

        public static string ReadString(BinaryReader br, uint offset)
        {
            //long original_position = fs.Position;
            long original_position = br.BaseStream.Position;

            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            //fs.Position = offset;

            List<byte> strBytes = new List<byte>();
            int b;
            while ((b = br.ReadByte()) != 0x00)
                strBytes.Add((byte)b);
            
            br.BaseStream.Seek(original_position, SeekOrigin.Begin);
            //fs.Position = original_position;
            return Encoding.ASCII.GetString(strBytes.ToArray());
        }

        public static float byteToFloat(byte byteValue)
        {
            return (float)(byteValue / 127.5 - 1.0);
        }

        public static SlimDX.Matrix ReadMatrix(BinaryReader br, bool invert = false)
        {
            SlimDX.Matrix temp = new SlimDX.Matrix();

            temp.M11 = br.ReadSingle();
            temp.M12 = br.ReadSingle();
            temp.M13 = br.ReadSingle();
            temp.M14 = br.ReadSingle();

            temp.M21 = br.ReadSingle();
            temp.M22 = br.ReadSingle();
            temp.M23 = br.ReadSingle();
            temp.M24 = br.ReadSingle();

            temp.M31 = br.ReadSingle();
            temp.M32 = br.ReadSingle();
            temp.M33 = br.ReadSingle();
            temp.M34 = br.ReadSingle();

            temp.M41 = br.ReadSingle();
            temp.M42 = br.ReadSingle();
            temp.M43 = br.ReadSingle();
            temp.M44 = br.ReadSingle();

            if (invert)
                temp.Invert();

            return temp;
        }

        public static Vector4 stringToVec4(string value)
        {
            string[] temp = value.Split(',');
            if (temp.Count() == 4)
                return new Vector4(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]), float.Parse(temp[3]));
            else if (temp.Count() == 3)
                return new Vector4(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]), 0);
            else 
                return new Vector4();
        }

        public static Vector3 stringToVec3(string value)
        {
            string[] temp = value.Split(',');
            return new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
        }

        public static Vector2 stringToVec2(string value)
        {
            string[] temp = value.Split(',');
            return new Vector2(float.Parse(temp[0]), float.Parse(temp[1]));
        }

        public static UInt16 ReverseBytes(UInt16 value)
        {
            return (UInt16)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        public static UInt32 ReverseBytes(UInt32 value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }

        public static UInt64 ReverseBytes(UInt64 value)
        {
            return (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
                   (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
                   (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
                   (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;
        }

        public static uint getFNV1Hash(string name)
        {
            if (name == null || name == "")
                return 0;
            const uint Fnv1Prime = unchecked(16777619);
            const uint Fnv1OffsetBasis = unchecked(2166136261);

            uint hash = Fnv1OffsetBasis;
            char[] arName = name.ToLower().ToArray();

            for (var i = 0; i < arName.Count(); i++)
            {
                unchecked
                {
                    hash *= Fnv1Prime;
                    hash ^= arName[i];
                }
            }

            var mask = 1 << 31;
            hash = (uint)((hash >> 32) ^ (hash & mask));

            return hash;
        }
    }
}
