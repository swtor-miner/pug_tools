using System;

namespace GomLib
{
    public abstract class DomType
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public DataObjectModel Dom_ { get; set; }

        /// <summary>Second Loading Pass, used to link types to other types</summary>
        public virtual void Link(DataObjectModel _dom) { }

        public virtual void Print(System.IO.TextWriter writer)
        {
            writer.WriteLine("{0} - 0x{1:X}", Name, Id);
            if (!string.IsNullOrEmpty(Description))
            {
                writer.WriteLine("\t{0}", Description);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
