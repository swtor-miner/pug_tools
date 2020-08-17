using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.DomTypeLoaders
{
    class FieldLoader : IDomTypeLoader
    {
        public int SupportedType { get { return (int)DomTypes.Field; } }

        public DomType Load(GomBinaryReader reader)
        {
            DomField result = new DomField();
            LoaderHelper.ParseShared(reader, result);

            reader.BaseStream.Position = 0x12;
            short typeOffset = reader.ReadInt16();

            reader.BaseStream.Position = typeOffset;
            result.GomType = reader.ReadGomType();

            return result;
        }
    }
}
