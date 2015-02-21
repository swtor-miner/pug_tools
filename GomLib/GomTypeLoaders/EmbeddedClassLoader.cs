using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypeLoaders
{
    class EmbeddedClassLoader : IGomTypeLoader
    {
        public GomTypeId SupportedType { get { return GomTypeId.EmbeddedClass; } }

        public GomType Load(GomBinaryReader reader, bool fromGom, DataObjectModel dom)
        {
            GomTypes.EmbeddedClass t = new GomTypes.EmbeddedClass();

            if (fromGom)
            {
                ulong classTypeId = reader.ReadUInt64();
                t.DomClassId = classTypeId;
            }

            return t;
        }
    }
}
