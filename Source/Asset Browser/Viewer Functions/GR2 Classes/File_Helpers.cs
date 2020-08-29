using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public static float ByteToNormal(byte byteValue)
        {
            return (float)((byteValue - 127.5) / 127.5);
        }

        public static float ByteToFloat(byte byteValue)
        {
            return byteValue / 255;
        }

        public static Matrix ReadMatrix(BinaryReader br, bool invert = false)
        {
            Matrix temp = new Matrix
            {
                M11 = br.ReadSingle(),
                M12 = br.ReadSingle(),
                M13 = br.ReadSingle(),
                M14 = br.ReadSingle(),

                M21 = br.ReadSingle(),
                M22 = br.ReadSingle(),
                M23 = br.ReadSingle(),
                M24 = br.ReadSingle(),

                M31 = br.ReadSingle(),
                M32 = br.ReadSingle(),
                M33 = br.ReadSingle(),
                M34 = br.ReadSingle(),

                M41 = br.ReadSingle(),
                M42 = br.ReadSingle(),
                M43 = br.ReadSingle(),
                M44 = br.ReadSingle()
            };

            if (invert)
                temp.Invert();

            return temp;
        }

        public static Vector4 StringToVec4(string value)
        {
            string[] temp = value.Split(',');
            if (temp.Count() == 4)
                return new Vector4(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]), float.Parse(temp[3]));
            else if (temp.Count() == 3)
                return new Vector4(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]), 0);
            else
                return new Vector4();
        }

        public static Vector3 StringToVec3(string value)
        {
            string[] temp = value.Split(',');
            return new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
        }

        public static Vector2 StringToVec2(string value)
        {
            string[] temp = value.Split(',');
            return new Vector2(float.Parse(temp[0]), float.Parse(temp[1]));
        }

        public static ushort ReverseBytes(ushort value)
        {
            return (ushort)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        public static uint ReverseBytes(uint value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }

        public static ulong ReverseBytes(ulong value)
        {
            return (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
                   (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
                   (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
                   (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;
        }

        public static uint GetFNV1Hash(string name)
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
