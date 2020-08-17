using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TorLib
{
    public enum Endianness
    {
        LittleEndian,
        BigEndian
    }

    public class ExtendedBinaryReader : BinaryReader
    {
        public ExtendedBinaryReader(Stream str) : base(str) { }
        public ExtendedBinaryReader(Stream str, Encoding encoding) : base(str, encoding) { }

        public string ReadNullTerminatedString()
        {
            return this.ReadNullTerminatedString(Encoding.UTF8);
        }

        public string ReadNullTerminatedString(Encoding encoding)
        {
            List<byte> byteBuffer = new List<byte>();
            byte b = this.ReadByte();
            // Read until we encounter a null byte
            while (b != 0)
            {
                byteBuffer.Add(b);
                b = this.ReadByte();
            }

            return encoding.GetString(byteBuffer.ToArray());
        }

        public short ReadInt16(Endianness endianness)
        {
            short val = base.ReadInt16();
            if (endianness == Endianness.LittleEndian) { return val; }
            else { return System.Net.IPAddress.NetworkToHostOrder(val); }
        }

        public ushort ReadUInt16(Endianness endianness)
        {
            if (endianness == Endianness.LittleEndian) { return base.ReadUInt16(); }

            byte[] b = base.ReadBytes(2);
            return BitConverter.ToUInt16(b.Reverse().ToArray(), 0);
        }

        public int ReadInt32(Endianness endianness)
        {
            int val = base.ReadInt32();

            if (endianness == Endianness.LittleEndian) { return val; }
            else { return System.Net.IPAddress.NetworkToHostOrder(val); }
        }

        public uint ReadUInt32(Endianness endianness)
        {
            if (endianness == Endianness.LittleEndian) { return base.ReadUInt32(); }

            byte[] b = base.ReadBytes(4);
            return BitConverter.ToUInt32(b.Reverse().ToArray(), 0);
        }

        public float ReadSingle(Endianness endianness)
        {
            if (endianness == Endianness.LittleEndian) { return base.ReadSingle(); }

            byte[] b = base.ReadBytes(4);
            return BitConverter.ToSingle(b.Reverse().ToArray(), 0);
        }

        public string ReadFixedLengthString(int length)
        {
            return this.ReadFixedLengthString(length, Encoding.UTF8);
        }

        public string ReadFixedLengthString(int length, Encoding encoding)
        {
            byte[] buff = this.ReadBytes(length);
            return encoding.GetString(buff);
        }
    }
}
