using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class MapPage
    {
        public int Id { get; set; }
        public Area Area { get; set; }
        public long Guid { get; set; }
        public long SId { get; set; }
        public string MapName { get; set; }
        public string Name { get; set; }
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

        public ScriptEnum ExplorationType { get; set; }
    }
}
