using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypeLoaders
{
    class EnumLoader : IGomTypeLoader
    {
        public GomTypeId SupportedType { get { return GomTypeId.Enum; } }

        public GomType Load(GomBinaryReader reader, bool fromGom, DataObjectModel dom)
        {
            GomTypes.Enum t = new GomTypes.Enum();
            if (fromGom)
            {
                ulong enumTypeId = reader.ReadUInt64();
                t.DomEnumId = enumTypeId;
            }

            return t;
        }
    }
}
