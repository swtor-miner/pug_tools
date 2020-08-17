using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypeLoaders
{
    class StringLoader : IGomTypeLoader
    {
        public GomTypeId SupportedType { get { return GomTypeId.String; } }

        public GomType Load(GomBinaryReader reader, bool fromGom, DataObjectModel dom)
        {
            return new GomTypes.String();
        }
    }
}
