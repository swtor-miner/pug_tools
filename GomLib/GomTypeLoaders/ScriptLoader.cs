using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypeLoaders
{
    class ScriptLoader : IGomTypeLoader
    {
        public GomTypeId SupportedType { get { return GomTypeId.Script; } }

        public GomType Load(GomBinaryReader reader, bool fromGom, DataObjectModel dom)
        {
            return new GomTypes.Script();
        }
    }
}
