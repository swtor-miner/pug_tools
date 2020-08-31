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
            minX = br.ReadSingle();
            minY = br.ReadSingle();
            minZ = br.ReadSingle();
            minW = br.ReadSingle();

            maxX = br.ReadSingle();
            maxY = br.ReadSingle();
            maxZ = br.ReadSingle();
            maxW = br.ReadSingle();
        }
    }
}
