using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypeLoaders
{
    class ClassRefLoader : IGomTypeLoader
    {
        public GomTypeId SupportedType { get { return GomTypeId.ClassRef; } }

        public GomType Load(GomBinaryReader reader, bool fromGom, DataObjectModel dom)
        {
            GomTypes.ClassRef t = new GomTypes.ClassRef();

            if (fromGom)
            {
                ulong classTypeId = reader.ReadUInt64();
                t.DomClassId = classTypeId;
            }

            return t;
        }
    }
}
