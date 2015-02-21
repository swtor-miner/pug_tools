using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypeLoaders
{
    class VectorLoader : IGomTypeLoader
    {
        public GomTypeId SupportedType { get { return GomTypeId.Vec3; } }

        public GomType Load(GomBinaryReader reader, bool fromGom, DataObjectModel dom)
        {
            return new GomTypes.Vector();
        }
    }
}
