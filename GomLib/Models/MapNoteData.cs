using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class MapNoteData : GameObject
    {
        public MapNoteData(DataObjectModel dom)
        {
            this._dom = dom;
        }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float Rotation { get; set; }
        public Dictionary<string, bool> MapTags { get; set; }
        public string ParentMapTag { get; set; }
        public bool ShowGhosted { get; set; }
        public bool ShowOffmapArrow { get; set; }
        public bool ShowQuestBreadcrumbOnly { get; set; }
        public bool DoNotBreadcrumb { get; set; }
        public bool IgnoreFoW { get; set; }
        public string TemplateFQN { get; set; }
        [JsonIgnore]
        public string _MPNB62Id { get; set; }
        public string MPNB62Id
        {
            get
            {
                if (_MPNB62Id == null) {
                    var obj = _dom.GetObjectNoLoad(TemplateFQN);
                    if(obj != null)
                    {
                        return obj.Id.ToMaskedBase62();
                    }
                }
                return null;
            }
        }
        [JsonIgnore]
        public MapNote _MPN { get; set; }
        [JsonIgnore]
        public MapNote MPN
        {
            get
            {
                if (_MPN == null)
                {
                    var obj = _dom.GetObject(TemplateFQN);
                    if (obj != null)
                        _MPN = (MapNote)GameObject.Load(obj);
                }
                return _MPN;
            }
        }

        public MapNoteData Load(XElement node)
        {
            foreach (var f in node.Elements())
            {
                var attrib = f.Attribute("name");
                if (!f.HasAttributes || attrib == null)
                    continue;
                switch (attrib.Value)
                {
                    case "mpnPosition":
                        var temp_vec = f.Value.Substring(1, f.Value.Length - 2);
                        var split = temp_vec.Split(',');
                        if (split.Count() == 3)
                        {
                            float x;
                            float.TryParse(split[0], out x);
                            this.PositionX = x;
                            float y;
                            float.TryParse(split[1], out y);
                            this.PositionY = y;
                            float z;
                            float.TryParse(split[2], out z);
                            this.PositionZ = z;
                        }
                        break;
                    case "mpnRotation":
                        var rotsplit = f.Value.Split(',');
                        if (rotsplit.Count() == 3)
                        {
                            float r;
                            float.TryParse(rotsplit[1], out r);
                            this.Rotation = r;
                        }
                        break;
                    case "mpnMapTags":
                        this.MapTags = new Dictionary<string, bool>();
                        for(var m = 0; m < f.Elements().Count(); m = m + 2)
                        {
                            var elems = f.Elements();
                            var k = elems.ElementAt(m);
                            var e = elems.ElementAt(m+1);
                            bool mtb;
                            bool.TryParse(e.Value, out mtb);
                            this.MapTags.Add(k.Value, mtb);
                        }
                        break;
                    case "ParentMapTag":
                        this.ParentMapTag = f.Value;
                        break;
                    case "ShowGhosted":
                        bool g;
                        bool.TryParse(f.Value, out g);
                        this.ShowGhosted = g;
                        break;
                    case "ShowOffmapArrow":
                        bool o;
                        bool.TryParse(f.Value, out o);
                        this.ShowOffmapArrow = o;
                        break; 
                    case "ShowQuestBreadcrumbOnly":
                        bool b;
                        bool.TryParse(f.Value, out b);
                        this.ShowQuestBreadcrumbOnly = b;
                        break;
                    case "DoNotBreadcrumb":
                        bool d;
                        bool.TryParse(f.Value, out d);
                        this.DoNotBreadcrumb = d;
                        break;
                    case "mpnIgnoreFoW":
                        bool w;
                        bool.TryParse(f.Value, out w);
                        this.IgnoreFoW = w;
                        break;
                    case "mpnTemplateFQN":
                        string fqn = f.Value.Replace("\\server\\mpn\\", "mpn.").Replace(".mpn", "").Replace("\\", ".");
                        this.TemplateFQN = fqn;
                        //var objId = _dom.GetObjectId(fqn);
                        //if(objId != null)
                        //{
                        //    //this.MPN = (MapNote)GameObject.Load(obj);
                        //}
                        break;
                }
            }
            return this;
        }

        public override int GetHashCode()
        {
            int result = PositionX.GetHashCode();
            result ^= PositionY.GetHashCode();
            result ^= PositionZ.GetHashCode();
            result ^= Rotation.GetHashCode();
            if (MapTags != null) foreach (var x in MapTags) { result ^= x.Key.GetHashCode(); result ^= x.Value.GetHashCode(); }
            result ^= ParentMapTag.GetHashCode();
            result ^= ShowGhosted.GetHashCode();
            result ^= ShowOffmapArrow.GetHashCode();
            result ^= ShowQuestBreadcrumbOnly.GetHashCode();
            result ^= DoNotBreadcrumb.GetHashCode();
            result ^= IgnoreFoW.GetHashCode();
            //result ^= ShowOffmapArrow.GetHashCode();
            return result;
        }
    }
}
