using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypeLoaders
{
    class MapLoader : IGomTypeLoader
    {
        DataObjectModel _dom;

        public MapLoader(DataObjectModel dom)
        {
            _dom = dom;
        }

        public GomTypeId SupportedType { get { return GomTypeId.Map; } }

        public GomType Load(GomBinaryReader reader, bool fromGom, DataObjectModel dom)
        {
            if (_dom == null) _dom = dom;
            var t = new GomTypes.Map();
            if (fromGom)
            {
                t.KeyType = _dom.gomTypeLoader.Load(reader, dom, fromGom);
                t.ValueType = _dom.gomTypeLoader.Load(reader, dom, fromGom);
            }
            return t;
        }
    }
}
