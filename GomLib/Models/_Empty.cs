using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class Empty : GameObject, IEquatable<Empty>
    {
        public ulong NodeId { get; set; }
        //public Dictionary<string, string> LocalizedTitle { get; set; }
        //public string Title { get; set; }

        //[Newtonsoft.Json.JsonIgnore]

        public override HashSet<string> GetDependencies()
        {
            HashSet<string> returnList = new HashSet<string>();
            //Add code here to add things like icons and models to the list.
            return returnList;
        }

        public override int GetHashCode()
        {
            int hash = NodeId.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is Empty obj2)) return false;

            return Equals(obj2);
        }

        public bool Equals(Empty obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (NodeId != obj.NodeId)
                return false;

            return true;
        }
    }
}
