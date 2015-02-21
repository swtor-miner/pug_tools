using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Xml.XPath;

namespace GomLib.Models
{
    public class Spawner : GameObject
    {
        public SpawnerType SpawnerType { get; set; }
        public List<GameObject> Entities { get; set; }
    }
}
