using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.DomTypeLoaders
{
    class FileInstanceLoader : IDomTypeLoader
    {
        private readonly HashSet<string> outDirs = new HashSet<string>();

        public int SupportedType { get { return (int)DomTypes.Instance; } }

        public HashSet<string> OutDirs => outDirs;

        public DomType Load(GomBinaryReader reader)
        {
            GomObject result = new GomObject
            {
                Id = reader.ReadUInt64()
            };
            int nameLen = reader.ReadInt32();
            result.Name = reader.ReadFixedLengthString(nameLen - 1);
            reader.ReadByte();
            result.Description = String.Empty;

            reader.ReadByte();
            reader.ReadInt32();
            reader.ReadInt32();
            reader.ReadInt32();
            result.ClassId = reader.ReadUInt64();
            // reader.ReadBytes(0xC);
            reader.ReadBytes(0x8);
            result.ObjectSizeInFile = reader.ReadInt32(); // 0x24
            result.IsCompressed = false;
            // result.NodeDataOffset = 20 + nameLen + 37; // 8 byte file header + node ID + nameLen + node name + 37 byte node header
            result.NodeDataOffset = 20 + nameLen + 33; // 8 byte file header + node ID + nameLen + node name + 33 byte node header

            if (result.ObjectSizeInFile > 0)
            {
                // Copy the data to the instance
                //var buff = reader.ReadBytes(result.ObjectSizeInFile);

                result.DataLength = result.ObjectSizeInFile;
                //result.DataBuffer = buff;
            }
            else
            {
                result.DataLength = 0;
            }

            // Gom.AddObject(result);

            return result;
        }
    }
}
