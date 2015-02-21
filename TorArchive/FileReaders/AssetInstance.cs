using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TorLib.FileReaders
{
    public class AssetInstance
    {
        public long Id { get; set; }
        public long AssetId { get; set; }
        public long ParentInstanceId { get; set; }
        public RoomSpecification Room { get; set; }
        public bool HasPosition { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }

        public List<float> GetAbsolutePosition()
        {
            List<float> result = new List<float>(3);
            result.Add(PositionX);
            result.Add(PositionY);
            result.Add(PositionZ);
            long pId = this.ParentInstanceId;
            AssetInstance p;
            while (pId != 0)
            {
                if (!this.Room.InstancesById.ContainsKey(pId)) { break; }
                p = this.Room.InstancesById[pId];
                result[0] += p.PositionX;
                result[1] += p.PositionY;
                result[2] += p.PositionZ;
                pId = p.ParentInstanceId;
            }

            return result;
        }
    }
}
