using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypes
{
    public class ClassRef : GomType
    {
        public ClassRef() : base(GomTypeId.ClassRef) { }

        public ulong DomClassId { get; internal set; }
        public DomClass DomClass { get; internal set; }

        internal override void Link(DataObjectModel dom)
        {
            _dom = dom;
            if (DomClassId != 0)
            {
                DomClass = _dom.Get<DomClass>(DomClassId);
            }
        }

        public override string ToString()
        {
            return string.Format("classref {0}", DomClass);
        }

        public override object ReadData(DataObjectModel dom, GomBinaryReader reader)
        {
            if (_dom == null) _dom = dom;
            ulong instanceId = reader.ReadNumber();
            var gomObj = _dom.Get<GomObject>(instanceId);
            //if (gomObj != null)
            //{
            //    gomObj.Load();
            //}

            return gomObj;
        }
    }
}
