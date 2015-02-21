using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.DomTypeLoaders
{
    interface IDomTypeLoader
    {
        int SupportedType { get; }
        DomType Load(GomBinaryReader reader);
    }
}
