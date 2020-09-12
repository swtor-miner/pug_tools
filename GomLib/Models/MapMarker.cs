using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class MapMarker
    {
        public Placeable Placeable { get; set; }
        public Npc Npc { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public MapMarkerIcon Icon { get; set; }
        public MapPage Map { get; set; }
        public string RoomName { get; set; }

        public ulong ItemId
        {
            get
            {
                if (Npc != null) { return Npc.Id; }
                return Placeable.Id;
            }
        }

        public string ItemType
        {
            get
            {
                if (Npc != null) { return "Npc"; }
                return "Placeable";
            }
        }

        public MapMarker Clone()
        {
            return new MapMarker()
            {
                Placeable = Placeable,
                Npc = Npc,
                X = X,
                Y = Y,
                Z = Z,
                Icon = Icon,
                Map = Map,
                RoomName = RoomName
            };
        }
    }
}
