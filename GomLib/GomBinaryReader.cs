using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GomLib
{
    public enum Endianness
    {
        LittleEndian,
        BigEndian
    }

    public class GomBinaryReader : BinaryReader
    {
        readonly DataObjectModel _dom;

        public GomBinaryReader(Stream str, DataObjectModel dom) : base(str)
        {
            _dom = dom;
        }
        public GomBinaryReader(Stream str, Encoding encoding, DataObjectModel dom) : base(str, encoding)
        {
            _dom = dom;
        }

        public ulong ReadNumber()
        {
            //System.Console.WriteLine(this.ToString());
            ulong val = 0;
            byte b0 = ReadByte();
            if (b0 == 0xD2) { b0 = ReadByte(); } //flag on lookup lists. Unsure of function.
            if (b0 < 0xC0) { return b0; }
            if (b0 < 0xC8)
            {
                byte[] byteBuffer = new byte[b0 - 0xBF];
                Read(byteBuffer, 0, byteBuffer.Length);

                for (int i = 0; i < byteBuffer.Length; i++)
                {
                    val <<= 8;
                    val += byteBuffer[i];
                }

                return val;
            }
            else if (b0 < 0xD0)
            {
                byte[] byteBuffer = new byte[b0 - 0xC7];
                Read(byteBuffer, 0, byteBuffer.Length);

                for (int i = 0; i < byteBuffer.Length; i++)
                {
                    val <<= 8;
                    val += byteBuffer[i];
                }

                return val;
            }
            /*else if (b0 == 0xD8)
            {
                return (ulong)this.ReadInt32(); // This is wrong, but I don't know wtf it's looking for.
            }*/
            else if (b0 == 0xD0)
            {
                return ReadByte();
            }
            else if (b0 == 0xEF) //this is likely a bug due to not having a GomTypeId.RawData reader
            {
                byte[] byteBuffer = new byte[4];//this is wrong, but it avoids the exception for now.
                Read(byteBuffer, 0, byteBuffer.Length); // it's almost right, but the ctlCoverMovementDirection_c value isn't being read right, which throws the following values off.

                for (int i = 0; i < byteBuffer.Length; i++)
                {
                    val <<= 8;
                    val += byteBuffer[i];
                }

                //var bs = this.ReadByte();
                return val;
            }
            else
            {
                throw new InvalidOperationException(string.Format("Unknown Number Prefix: 0x{0:X}", b0));
            }
        }

        public long ReadSignedNumber()
        {
            long val = 0;
            byte b0 = ReadByte();
            if (b0 == 0xD2)
            {
                int num = ReadByte();
                string result = ReadFixedLengthString(num);
                val = long.Parse(result);
            } //flag on lookup lists. Unsure of function.
            else
            {
                if (b0 < 0xC0) { return b0; }
                if (b0 < 0xC8)
                {
                    byte[] byteBuffer = new byte[b0 - 0xBF];
                    Read(byteBuffer, 0, byteBuffer.Length);

                    for (int i = 0; i < byteBuffer.Length; i++)
                    {
                        val <<= 8;
                        val |= byteBuffer[i];
                    }
                    val = -val;
                }
                else if (b0 < 0xD0)
                {
                    byte[] byteBuffer = new byte[b0 - 0xC7];
                    Read(byteBuffer, 0, byteBuffer.Length);

                    for (int i = 0; i < byteBuffer.Length; i++)
                    {
                        val <<= 8;
                        val |= byteBuffer[i];
                    }
                }
                else if (b0 == 0xD0)
                {
                    return 0; // (long)this.ReadByte(); // this was wrong
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Unknown Number Prefix: 0x{0:X}", b0));
                }
            }

            //if (val == -2305634256081214423)
            //{
            //    Console.WriteLine("Gotcha! long");
            //}

            return val;
        }

        public TypedValue ReadTypedValue()
        {
            TypedValueType valType = (TypedValueType)ReadByte();
            TypedValue result = new TypedValue(valType);
            result.Parse(this);

            return result;
        }

        public ulong ReadVariableWidthUInt64(int lengthInBytes)
        {
            if ((lengthInBytes < 1) || (lengthInBytes > 8))
            {
                throw new ArgumentOutOfRangeException("lengthInBytes", "Length in bytes must be between 1 and 8");
            }

            ulong result = 0;

            byte[] bytes = ReadBytes(lengthInBytes);
            int shiftAmt = lengthInBytes - 1;
            for (var i = 0; i < lengthInBytes; i++)
            {
                ulong val = bytes[i];
                val <<= (8 * shiftAmt);
                shiftAmt--;
                result += val;
            }

            return result;
        }

        public GomType ReadGomType()
        {
            return _dom.gomTypeLoader.Load(this, _dom);
        }

        public string ReadNullTerminatedString()
        {
            return ReadNullTerminatedString(Encoding.UTF8);
        }

        public string ReadNullTerminatedString(Encoding encoding)
        {
            List<byte> byteBuffer = new List<byte>();
            byte b = ReadByte();
            // Read until we encounter a null byte
            while (b != 0)
            {
                byteBuffer.Add(b);
                b = ReadByte();
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

        public string ReadLengthPrefixString()
        {
            return ReadLengthPrefixString(Encoding.UTF8);
        }

        public string ReadLengthPrefixString(Encoding encoding)
        {
            int len = (int)ReadNumber();
            return ReadFixedLengthString(len, encoding);
        }

        public string ReadFixedLengthString(int length)
        {
            return ReadFixedLengthString(length, Encoding.UTF8);
        }

        public string ReadFixedLengthString(int length, Encoding encoding)
        {
            byte[] buff = ReadBytes(length);
            string result = encoding.GetString(buff);
            //if (result.Equals("plc.location.tatooine.item.treasure_chest.chest", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    Console.WriteLine("Gotcha! string");
            //}
            return result;
        }
    }
}
