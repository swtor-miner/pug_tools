using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypeLoaders
{
    class ListLoader : IGomTypeLoader
    {
        DataObjectModel _dom;

        public ListLoader(DataObjectModel dom)
        {
            _dom = dom;
        }

        public GomTypeId SupportedType { get { return GomTypeId.List; } }

        public GomType Load(GomBinaryReader reader, bool fromGom, DataObjectModel dom)
        {
            if (_dom == null) _dom = dom;
            var result = new GomTypes.List();

            if (fromGom)
            {
                result.ContainedType = _dom.gomTypeLoader.Load(reader, dom, fromGom);
            }

            return result;
        }
    }
}
