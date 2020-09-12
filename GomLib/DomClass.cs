using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib
{
    public class DomClass : DomType
    {
        internal List<ulong> ComponentIds { get; private set; }
        public List<ulong> FieldIds { get; private set; }

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
            Dom_ = dom;
            foreach (var cId in ComponentIds)
            {
                var component = Dom_.Get<DomClass>(cId);
                Components.Add(component);
            }

            foreach (var fId in FieldIds)
            {
                var field = Dom_.Get<DomField>(fId);
                Fields.Add(field);
            }
        }

        public override void Print(System.IO.TextWriter writer)
        {
            writer.WriteLine("class {0} : {1} // 0x{2:X}", Name, string.Join(", ", Components.Select(c => c.Name).ToArray()), Id);
            if (!string.IsNullOrEmpty(Description)) writer.WriteLine("\t{0}", Description);
            foreach (var field in Fields)
            {
                writer.WriteLine("\t{0} {1} // 0x{2:X}", field.GomType, field.Name, field.Id);
            }
            foreach (var c in Components)
            {
                foreach (var f in c.Fields)
                {
                    writer.WriteLine("\t{0} {1} // 0x{2:X}", f.GomType, f.Name, f.Id);
                }
            }
        }
    }
}
