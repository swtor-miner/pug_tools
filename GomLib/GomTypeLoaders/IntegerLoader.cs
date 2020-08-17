using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypeLoaders
{
    class IntegerLoader : IGomTypeLoader
    {
        public GomTypeId SupportedType { get { return GomTypeId.Int64; } }

        public GomType Load(GomBinaryReader reader, bool fromGom, DataObjectModel dom)
        {
            return new GomTypes.Integer();
        }
    }
}
