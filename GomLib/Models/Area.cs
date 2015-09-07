using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace GomLib.Models
{
    public class Area : PseudoGameObject, IEquatable<Area>
    {
        //[Newtonsoft.Json.JsonIgnore]
        //public DataObjectModel _dom;

        //public long Id { get; set; }
        //public Guid CommentableId { get; set; }
        public long DisplayNameId { get; set; }
        //public string Name { get; set; }
        public ulong AreaId { get; set; }
        public string ZoneName { get; set; }

        public List<MapPage> MapPages { get; set; }

        public Dictionary<ulong, long> FowGroupStringIds { get; set; }

        internal Dictionary<ulong, Dictionary<string, string>> _FowGroupLocalizedStrings { get; set; }
        public Dictionary<ulong, Dictionary<string, string>> FowGroupLocalizedStrings { get
            {
                if (_FowGroupLocalizedStrings == null)
                {
                    var strTable = _dom.stringTable.Find("str.sys.worldmap");
                    _FowGroupLocalizedStrings = new Dictionary<ulong, Dictionary<string, string>>();
                    foreach (var kvp in FowGroupStringIds)
                    {
                        _FowGroupLocalizedStrings.Add(kvp.Key, strTable.GetLocalizedText(kvp.Value, "str.sys.worldmap"));
                    }
                }
                return _FowGroupLocalizedStrings;
            }
        }
        private bool sortedByVolume = false;

        public Dictionary<ulong, string> Assets { get; set; }//old
        public AreaDat AreaDat { get; set; } //new

        public void SortMaps()
        {
            if (this.MapPages == null) { return; }
            this.MapPages.Sort((x, y) => x.Volume.CompareTo(y.Volume)); // Sort MapPages by Volume (smallest to largest)
            sortedByVolume = true;
        }

        public MapPage FindSmallestMapContaining(float x, float y, float z)
        {
            if (!sortedByVolume) { this.SortMaps(); }
            return this.MapPages.Find(m => m.ContainsPoint(x, y, z)); // Find first Map that contains the point -- since maps are sorted from smallest to largest, the first found will be the one we want
        }

        public MapPage FindSmallestMapContainingInMinimap(float x, float y, float z)
        {
            if (!sortedByVolume) { this.SortMaps(); }
            return this.MapPages.Find(m => m.MiniMapContainsPoint(x, y, z));
        }

        public override int GetHashCode()  //should be fixed.
        {
            int hash = Id.GetHashCode();
            hash ^= DisplayNameId.GetHashCode();
            if (Name != null) hash ^= Name.GetHashCode();
            hash ^= AreaId.GetHashCode();
            if (ZoneName != null) hash ^= ZoneName.GetHashCode();
            if (MapPages != null) foreach (var x in MapPages) { hash ^= x.GetHashCode(); }
            hash ^= sortedByVolume.GetHashCode();
            if (Assets != null) foreach (var x in Assets) { hash ^= x.GetHashCode(); }

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            Area itm = obj as Area;
            if (itm == null) return false;

            return Equals(itm);
        }

        public bool Equals(Area itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (this.GetHashCode() != itm.GetHashCode())
                return false;
            if (this.Id != itm.Id)
                return false;

            return true;
        }

        public override XElement ToXElement(bool verbose)
        {
            XElement area = new XElement("Area");
            if (this.Id == 0) return area;
            if (verbose)
            {
                //XElement MapPages = new XElement("MapPages");
                area.Add(new XAttribute("Id", Id),
                    new XElement("Name", Name),
                    new XElement("ChatZone", ZoneName),
                    //new XElement("CommentableId",   CommentableId), //this is randomly generated and has no meaning.
                    new XElement("DisplayNameId", DisplayNameId));
                string ImagePath = null;
                if (MapPages != null)
                {
                    foreach (var map_page in MapPages)
                    {
                        area.Add(map_page.ToXElement(verbose));
                    }

                }
                if (Assets.Count > 0)
                {
                    XElement assets = new XElement("Assets");
                    foreach (var kvp in Assets)
                    {
                        assets.Add(new XElement("Asset",
                            kvp.Value,
                            new XAttribute("Id", kvp.Key)));
                    }
                    area.Add(assets);
                }
            }
            else
            {
                area.Add(new XAttribute("Id", Id),
                    new XElement("Name", Name),
                    new XElement("ChatZone", ZoneName)
                );
            }
            return area;
        }
    }
}
