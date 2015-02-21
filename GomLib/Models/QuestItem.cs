﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class QuestItem : IEquatable<QuestItem>
    {
        public string Name { get; set; }
        public ulong Id { get; set; }
        public string Fqn { get; set; }
        public long MaxCount { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public DataObjectModel _dom { get; set; }

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
    }
}
