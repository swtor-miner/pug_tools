using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypeLoaders
{
    class TimeSpanLoader : IGomTypeLoader
    {
        public GomTypeId SupportedType { get { return GomTypeId.TimeSpan; } }

        public GomType Load(GomBinaryReader reader, bool fromGom, DataObjectModel dom)
        {
            return new GomTypes.TimeSpan();
        }
    }
}
