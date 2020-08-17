using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.DomTypeLoaders
{
    class ClassLoader : IDomTypeLoader
    {
        public int SupportedType { get { return (int)DomTypes.Class; } }

        public DomType Load(GomBinaryReader reader)
        {
            DomClass result = new DomClass();
            LoaderHelper.ParseShared(reader, result);

            reader.BaseStream.Position = 0x2A;
            short numComponents = reader.ReadInt16();
            short componentOffset = reader.ReadInt16();
            short numFields = reader.ReadInt16();
            short fieldsOffset = reader.ReadInt16();

            if (numComponents > 0)
            {
                reader.BaseStream.Position = componentOffset;
                for (var i = 0; i < numComponents; i++)
                {
                    result.ComponentIds.Add(reader.ReadUInt64());
                }
            }

            if (numFields > 0)
            {
                reader.BaseStream.Position = fieldsOffset;
                for (var i = 0; i < numFields; i++)
                {
                    result.FieldIds.Add(reader.ReadUInt64());
                }
            }
            
            return result;
        }
    }
}
