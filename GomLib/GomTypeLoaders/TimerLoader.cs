using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypeLoaders
{
    class TimerLoader : IGomTypeLoader
    {
        public GomTypeId SupportedType { get { return GomTypeId.Timer; } }

        public GomType Load(GomBinaryReader reader, bool fromGom, DataObjectModel dom)
        {
            return new GomTypes.Timer();
        }
    }
}
