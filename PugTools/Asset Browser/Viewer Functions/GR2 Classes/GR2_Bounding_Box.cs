using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileFormats
{
    public class GR2_Bounding_Box
    {
        public float minX = 0;
        public float minY = 0;
        public float minZ = 0;
        public float minW = 0;
        public float maxX = 0;
        public float maxY = 0;
        public float maxZ = 0;
        public float maxW = 0;

        public GR2_Bounding_Box(BinaryReader br)
        {
            this.minX = br.ReadSingle();
            this.minY = br.ReadSingle();
            this.minZ = br.ReadSingle();
            this.minW = br.ReadSingle();

            this.maxX = br.ReadSingle();
            this.maxY = br.ReadSingle();
            this.maxZ = br.ReadSingle();
            this.maxW = br.ReadSingle();
        }
    }
}
