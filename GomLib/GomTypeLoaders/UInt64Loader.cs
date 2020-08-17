using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypeLoaders
{
    class UInt64Loader : IGomTypeLoader
    {
        public GomTypeId SupportedType { get { return GomTypeId.UInt64; } }

        public GomType Load(GomBinaryReader reader, bool fromGom, DataObjectModel dom)
        {
            return new GomTypes.UInt64();
        }
    }
}
