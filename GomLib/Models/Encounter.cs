using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Xml.XPath;

namespace GomLib.Models
{
    public class Encounter : GameObject
    {
        public ulong NodeId { get; set; }
        public Dictionary<string,string> Spawners { get; set; }

        public HydraScript HydraScript { get; set; }
    }
}
