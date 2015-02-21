using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class Area
    {
        [Newtonsoft.Json.JsonIgnore]
        public DataObjectModel _dom;

        public int Id { get; set; }
        public Guid CommentableId { get; set; }
        public long DisplayNameId { get; set; }
        public string Name { get; set; }
        public ulong AreaId { get; set; }
        public string ZoneName { get; set; }

        public List<MapPage> MapPages { get; set; }

        private bool sortedByVolume = false;

        public Dictionary<ulong, string> Assets { get; set; }
        private void SortMaps()
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
    }
}
