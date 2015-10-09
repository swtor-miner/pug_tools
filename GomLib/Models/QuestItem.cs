using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class QuestItem : IEquatable<QuestItem>
    {
        public string Name { get; set; }
        [JsonConverter(typeof(ULongConverter))]
        public ulong Id { get; set; }
        public string Fqn { get; set; }
        public long MaxCount { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public DataObjectModel _dom { get; set; }

        [JsonConverter(typeof(LongConverter))]
        public long GUID { get; set; }
        public long Min { get; set; }
        public long Max { get; set; }

        public ulong VariableId { get; set; }
        public long UnknownLong { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            QuestItem qsi = obj as QuestItem;
            if (qsi == null) return false;

            return Equals(qsi);
        }

        public bool Equals(QuestItem qsi)
        {
            if (qsi == null) return false;

            if (ReferenceEquals(this, qsi)) return true;

            if (this.Fqn != qsi.Fqn)
                return false;
            if (this.GUID != qsi.GUID)
                return false;
            if (this.Id != qsi.Id)
                return false;
            if (this.Max != qsi.Max)
                return false;
            if (this.MaxCount != qsi.MaxCount)
                return false;
            if (this.Min != qsi.Min)
                return false;
            if (this.Name != qsi.Name)
                return false;
            if (this.UnknownLong != qsi.UnknownLong)
                return false;
            if (this.VariableId != qsi.VariableId)
                return false;
            return true;
        }

        public XElement ToXElement()
        {
            return ToXElement(false);
        }
        public XElement ToXElement(bool verbose)
        {
            XElement questItem = new XElement("QuestItem",
                new XElement("Name", Name),
                new XElement("MaxCount", MaxCount),
                new XElement("Min", Min),
                new XElement("Max", Max),
                new XElement("GUID", GUID),
                new XAttribute("Id", VariableId),
                new XElement("UnknownLong", UnknownLong));
            questItem.Add(new GameObject().ToXElement(Id, _dom, verbose));
            return questItem;
        }
    }
}
