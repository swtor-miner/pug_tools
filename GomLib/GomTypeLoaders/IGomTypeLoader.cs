using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypeLoaders
{
    interface IGomTypeLoader
    {
        GomTypeId SupportedType { get; }
        GomType Load(GomBinaryReader reader, bool fromGom, DataObjectModel dom);
    }
}
