using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypes
{
    public class EmbeddedClass : GomType
    {
        public EmbeddedClass() : base(GomTypeId.EmbeddedClass) { }

        public ulong DomClassId { get; internal set; }
        public DomClass DomClass { get; internal set; }

        internal override void Link(DataObjectModel dom)
        {
            _dom = dom;
            DomClass = _dom.Get<DomClass>(DomClassId);
        }

        public override string ToString()
        {
            return string.Format("class {0}", DomClass);
        }

        public override object ReadData(DataObjectModel dom, GomBinaryReader reader)
        {
            if (DomClass == null) Link(dom);
            var obj = _dom.scriptObjectReader.ReadObject(DomClass, reader, dom);
            return obj;
        }
    }
}
