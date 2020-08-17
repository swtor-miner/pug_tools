using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GomLib.Models;
using System.IO;

namespace GomLib.FileLoaders
{
    public class RoomDatLoader
    {
        private readonly DataObjectModel _dom;
        public RoomDatLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
        }

        public RoomDat Load(string room, int offset, AreaDat area)
        {
            if (area == null) return null;
            var file = _dom._assets.FindFile(String.Format("/resources/world/areas/{0}/{1}.dat", area.AreaId, room.ToLower()));
            if (file == null) return null;
            Stream fileStream = file.OpenCopyInMemory();
            BinaryReader binReader = new BinaryReader(fileStream);
            return Load(new RoomDat(room, offset, area), binReader);
        }

        public RoomDat Load(RoomDat room, BinaryReader br)
        {
            room.Dom_ = _dom;
            int header = br.ReadInt32();
            if (header == 24)
            {
                char c = br.ReadChar();
                StringBuilder formatter = new StringBuilder();
                while (c != '\0')
                {
                    formatter.Append(c);
                    c = br.ReadChar();
                }
                string format = formatter.ToString();
                if (format != "ROOM_DAT_BINARY_FORMAT_") return null; //not a room dat file

                br.BaseStream.Position = 0x1C; //Skip room header

                uint instanceOffset = br.ReadUInt32();
                _ = br.ReadUInt32();
                _ = br.ReadUInt32();
                _ = br.ReadUInt64(); //Always 281479271743491 : (03 00 01 00 01 00 01 00)
                uint fileNameLength = br.ReadUInt32();
                _ = ReadString(br, fileNameLength);

                //Instances
                br.BaseStream.Position = instanceOffset;
                uint numInstances = br.ReadUInt32();
                room.Instances = new Dictionary<ulong, AssetInstance>();
                for (uint i = 0; i < numInstances; i++)
                {
                    AssetInstance inst = new AssetInstance(room);
                    uint instanceHeader = br.ReadUInt32();
                    if (instanceHeader != 0xABCD1234)  // 0x3412CDAB
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                    else
                    {
                        // string sdifn = "";
                    }

                    _ = br.ReadByte();
                    inst.InstanceId = br.ReadUInt64();
                    inst.AssetId = br.ReadUInt64();
                    _ = br.ReadByte();
                    uint numProperties = br.ReadUInt32();
                    uint propteriesLength = br.ReadUInt32();

                    long startOffset = br.BaseStream.Position;

                    if (!room.Area.Assets.ContainsKey(inst.AssetId))
                    {
                        br.BaseStream.Position = startOffset + propteriesLength;
                        continue;
                    }

                    _ = br.ReadByte();
                    try
                    {
                        inst.RawProperties = new Dictionary<uint, object>();
                        for (uint p = 0; p < numProperties; p++)
                        {
                            DatTypeId type = (DatTypeId)br.ReadByte();
                            uint propertyId = br.ReadUInt32();

                            object o = null;
                            switch (type)
                            {
                                case DatTypeId.Boolean:

                                    byte b = br.ReadByte();
                                    if (b > 1)
                                        throw new IndexOutOfRangeException();
                                    bool boo = Convert.ToBoolean(b);
                                    o = boo;
                                    break;
                                case DatTypeId.Unknown1:
                                    int ival = br.ReadInt32();
                                    if (ival > 1)
                                        o = ival;
                                    break;
                                //case DatTypeId.Unknown2:
                                //    break;
                                case DatTypeId.UInt32:
                                    uint val = br.ReadUInt32();
                                    o = val;
                                    break;
                                case DatTypeId.Single:
                                    float flo = br.ReadSingle();
                                    o = flo;
                                    break;
                                case DatTypeId.UInt64:
                                    ulong lval = br.ReadUInt64();
                                    o = lval;
                                    break;
                                case DatTypeId.Vector3:
                                    float x = br.ReadSingle();
                                    float y = br.ReadSingle();
                                    float z = br.ReadSingle();
                                    o = new Vec3(x, y, z);
                                    break;
                                case DatTypeId.Unknown7:
                                    //byte[] bytes = br.ReadBytes(16);
                                    List<float> vec4 = new List<float>
                                    {
                                        br.ReadSingle(),
                                        br.ReadSingle(),
                                        br.ReadSingle(),
                                        br.ReadSingle()
                                    };
                                    o = vec4;
                                    break;
                                case DatTypeId.String: //these are different than the other strings in that they are unicode, but have yet to see a unicode-specific char
                                    uint strlen = br.ReadUInt32();
                                    //byte[] charbytes = br.ReadBytes((int)strlen - 2);
                                    //br.ReadByte(); //null byte
                                    //br.ReadByte(); //null byte
                                    //o = System.Text.Encoding.Unicode.GetString(charbytes);

                                    StringBuilder str = new StringBuilder((int)strlen);
                                    char c1 = br.ReadChar();
                                    char c2 = br.ReadChar();
                                    uint charsRead = 1;
                                    while (c1 != '\0' && c1 != '\0' && charsRead < strlen)
                                    {
                                        str.Append(c1);
                                        if (c2 != '\0')
                                            throw new IndexOutOfRangeException();
                                        c1 = br.ReadChar();
                                        c2 = br.ReadChar();
                                        charsRead++;
                                    }
                                    o = str.ToString();
                                    break;
                                case DatTypeId.Data: //skip raw data properties
                                    uint datalen = br.ReadUInt32();
                                    br.BaseStream.Position += datalen;
                                    break;
                                default:
                                    long curpos = br.BaseStream.Position; //this is for debugging new formats found
                                    byte[] bities = br.ReadBytes(32);
                                    br.BaseStream.Position = curpos;
                                    throw new IndexOutOfRangeException();
                            }
                            inst.RawProperties.Add(propertyId, o);

                        }

                    }
                    catch (Exception)
                    {
                        br.BaseStream.Position = startOffset + propteriesLength;
                    }
                    if (!room.Area.AssetInstances.ContainsKey(inst.AssetId))
                        room.Area.AssetInstances.Add(inst.AssetId, new List<AssetInstance>());
                    room.Area.AssetInstances[inst.AssetId].Add(inst);
                    room.Instances.Add(inst.InstanceId, inst);
                }
            }
            else
                return null; //invalid file type

            br.Dispose();
            return room;
        }

        private static string ReadString(BinaryReader br, uint length)
        {
            long curpos = br.BaseStream.Position;
            long endpos = curpos + length;
            char c = br.ReadChar();
            StringBuilder builder = new StringBuilder();
            while (c != '\0' && br.BaseStream.Position < endpos)
            {
                builder.Append(c);
                c = br.ReadChar();
            }
            return builder.ToString();
        }
    }
    public enum DatTypeId : byte
    {
        Boolean = 0x00,
        Unknown1 = 0x01,
        Unknown2 = 0x02,
        UInt32 = 0x03, // may be Int32
        Single = 0x04,
        UInt64 = 0x05,
        Vector3 = 0x06,
        Unknown7 = 0x07,
        String = 0x08,
        Data = 0x09
    }
}
