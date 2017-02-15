using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GomLib.Models
{
    public class MapPage: IEquatable<MapPage>
    {
        public int Id { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public Area Area { get; set; }
        public long Guid { get; set; }
        public long SId { get; set; }
        public string MapName { get; set; }
        public string Name { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public MapPage Parent { get; set; }
        public long ParentId { get; set; }
        public float MinX { get; set; }
        public float MinY { get; set; }
        public float MinZ { get; set; }
        public float MaxX { get; set; }
        public float MaxY { get; set; }
        public float MaxZ { get; set; }
        public bool IsHeroic { get; set; }
        public bool MountAllowed { get; set; }
        public string Tag { get; set; }
        public bool HasImage { get; set; }
        public float Volume { get; set; }
        public float MiniMapVolume { get; set; }
        public float MiniMapMinX { get; set; }
        public float MiniMapMinZ { get; set; }
        public float MiniMapMaxX { get; set; }
        public float MiniMapMaxZ { get; set; }
        public string ImagePath
        {
            get
            {
                return String.Format("/resources/world/areas/{0}/{1}_r.dds", Area.AreaId, MapName);
            }
        }

        public long ExplorationId { get; set; }
        public float mapFowRadius { get; internal set; }

        public int MiniMapColumnCount
        {
            get
            {
                if(_MiniMapColumCount != -1)
                {
                    return _MiniMapColumCount;
                } else
                {
                    FindMiniMapPartCounts();
                    return _MiniMapColumCount;
                }
            }
        }

        public int MiniMapRowCount
        {
            get
            {
                if(_MiniMapRowcount != -1)
                {
                    return _MiniMapRowcount;
                } else
                {
                    FindMiniMapPartCounts();
                    return _MiniMapRowcount;
                }
            }
        }

        internal int _MiniMapColumCount = -1;
        internal int _MiniMapRowcount = -1;

        public void FindMiniMapPartCounts()
        {
            //Find how many map tiles we have for the minimap.
            string miniMapBasePath = "/resources/world/areas/{0}/minimaps/{1}_{2}_{3}_r.dds";
            int columns = 0;
            int rows = 0;

            //Find the row tile count.
            bool foundAllRows = false;
            while (!foundAllRows)
            {
                string miniMapPartPath = string.Format(miniMapBasePath, Area.AreaId, MapName,
                    rows.ToString("00"), "00");

                if (Area._dom._assets.HasFile(miniMapPartPath))
                {
                    //Found map tile. Increment counter.
                    rows++;
                }
                else
                {
                    //Can't find map tile.
                    foundAllRows = true;
                }
            }

            //Find the column tile count.
            bool foundAllColumns = false;
            while (!foundAllColumns)
            {
                string miniMapPartPath = string.Format(miniMapBasePath, Area.AreaId, MapName,
                    "00", columns.ToString("00"));

                if (Area._dom._assets.HasFile(miniMapPartPath))
                {
                    //Found map tile. Increment counter.
                    columns++;
                }
                else
                {
                    //Can't find map tile.
                    foundAllColumns = true;
                }
            }

            _MiniMapColumCount = columns;
            _MiniMapRowcount = rows;
        }

        public void CalculateVolume()
        {
            float dx = MaxX - MinX;
            float dy = MaxY - MinY;
            float dz = MaxZ - MinZ;
            this.Volume = dx * dy * dz;

            float mdx = MiniMapMaxX - MiniMapMinX;
            float mdz = MiniMapMaxZ - MiniMapMinZ;
            this.MiniMapVolume = mdx * dy * mdz;
        }

        public bool ContainsPoint(float x, float y, float z)
        {
            return  HasImage &&         // We don't want to link MapMarkers to maps with no image
                    (x >= MinX) &&
                    (y >= MinY) &&
                    (z >= MinZ) &&
                    (x <= MaxX) &&
                    (y <= MaxY) &&
                    (z <= MaxZ);
        }

        public bool MiniMapContainsPoint(float x, float y, float z)
        {
            return HasImage &&         // We don't want to link MapMarkers to maps with no image
                    (x >= MiniMapMinX) && (x >= MinX) &&
                    (y >= MinY) &&
                    (z >= MiniMapMinZ) && (z >= MinZ) &&
                    (x <= MiniMapMaxX) && (x <= MaxX) &&
                    (y <= MaxY) &&
                    (z <= MiniMapMaxZ) && (z <= MaxZ);
        }

        public string ExplorationType { get; set; }

        public override int GetHashCode()  //should be fixed.
        {
            int hash = Id.GetHashCode();
            hash ^= Guid.GetHashCode();
            hash ^= SId.GetHashCode();
            if (MapName != null) hash ^= MapName.GetHashCode();
            if (Name != null) hash ^= Name.GetHashCode();
            hash ^= ParentId.GetHashCode();
            hash ^= MinX.GetHashCode();
            hash ^= MinY.GetHashCode();
            hash ^= MinZ.GetHashCode();
            hash ^= MaxX.GetHashCode();
            hash ^= MaxY.GetHashCode();
            hash ^= MaxZ.GetHashCode();
            hash ^= IsHeroic.GetHashCode();
            hash ^= MountAllowed.GetHashCode();
            if (Tag != null) hash ^= Tag.GetHashCode();
            hash ^= HasImage.GetHashCode();
            hash ^= Volume.GetHashCode();
            hash ^= MiniMapVolume.GetHashCode();
            hash ^= MiniMapMinX.GetHashCode();
            hash ^= MiniMapMinZ.GetHashCode();
            hash ^= MiniMapMaxX.GetHashCode();
            hash ^= MiniMapMaxZ.GetHashCode();

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            MapPage itm = obj as MapPage;
            if (itm == null) return false;

            return Equals(itm);
        }

        public bool Equals(MapPage itm)
        {
            if (itm == null) return false;

            if (ReferenceEquals(this, itm)) return true;

            if (this.GetHashCode() != itm.GetHashCode())
                return false;
            if (this.Id != itm.Id)
                return false;

            return true;
        }

        public XElement ToXElement(bool verbose)
        {
            return new XElement("MapPage",
                new XAttribute("Id", Id),
                new XElement("Name", Name),
                new XElement("MapName", MapName),
                new XElement("Guid", Guid),
                new XElement("HasImage", HasImage),
                new XElement("ImagePath", ImagePath),
                new XElement("IsHeroic", IsHeroic),
                new XElement("MaxX", MaxX),
                new XElement("MaxY", MaxY),
                new XElement("MaxZ", MaxZ),
                new XElement("MiniMapMaxX", MiniMapMaxX),
                new XElement("MiniMapMaxZ", MiniMapMaxZ),
                new XElement("MiniMapMinX", MiniMapMinX),
                new XElement("MiniMapMinZ", MiniMapMinZ),
                new XElement("MiniMapVolume", MiniMapVolume),
                new XElement("MinX", MinX),
                new XElement("MinY", MinY),
                new XElement("MinZ", MinZ),
                new XElement("MountAllowed", MountAllowed),
                new XElement("Tag", Tag),
                new XElement("Volume", Volume));

        }
    }
}
