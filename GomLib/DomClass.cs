using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib
{
    public class DomClass : DomType
    {
        internal List<UInt64> ComponentIds { get; private set; }
        public List<UInt64> FieldIds { get; private set; }

        public List<DomClass> Components { get; private set; }
        public List<DomField> Fields { get; private set; }

        public DomClass()
        {
            ComponentIds = new List<ulong>();
            FieldIds = new List<ulong>();
            Components = new List<DomClass>();
            Fields = new List<DomField>();
        }

        public override void Link(DataObjectModel dom)
        {
            this.Dom_ = dom;
            foreach (var cId in this.ComponentIds)
            {
                var component = Dom_.Get<DomClass>(cId);
                this.Components.Add(component);
            }

            foreach (var fId in this.FieldIds)
            {
                var field = Dom_.Get<DomField>(fId);
                this.Fields.Add(field);
            }
        }

        public override void Print(System.IO.TextWriter writer)
        {
            writer.WriteLine("class {0} : {1} // 0x{2:X}", Name, System.String.Join(", ", this.Components.Select(c => c.Name).ToArray()), Id);
            if (!String.IsNullOrEmpty(Description)) writer.WriteLine("\t{0}", Description);
            foreach (var field in this.Fields)
            {
                writer.WriteLine("\t{0} {1} // 0x{2:X}", field.GomType, field.Name, field.Id);
            }
            foreach (var c in this.Components)
            {
                foreach (var f in c.Fields)
                {
                    writer.WriteLine("\t{0} {1} // 0x{2:X}", f.GomType, f.Name, f.Id);
                }
            }
        }
    }
}
