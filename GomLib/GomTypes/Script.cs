using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypes
{
    public class Script : GomType
    {
        public Script() : base(GomTypeId.Script) { }

        public ulong DomClassId { get; internal set; }
        public DomClass DomClass { get; internal set; }

        internal override void Link(DataObjectModel dom)
        {
            this._dom = dom;
            if (DomClassId != 0)
            {
                DomClass = _dom.Get<DomClass>(DomClassId);
            }
        }

        public override string ToString()
        {
            return System.String.Format("script {0}", this.DomClass);
        }

        public override object ReadData(DataObjectModel dom, GomBinaryReader reader)
        {
            //Console.WriteLine(reader.ReadString());

            if (_dom == null) _dom = dom;
            ulong instanceId = reader.ReadUInt64();
            var gomObj = _dom.Get<GomObject>(instanceId);
            //if (gomObj != null)
            //{
            //    gomObj.Load();
            //}

            return gomObj;
            

                       

            throw new NotImplementedException();
        }
    }
}
