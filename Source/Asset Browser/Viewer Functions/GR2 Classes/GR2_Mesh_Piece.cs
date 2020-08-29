using System.IO;

namespace FileFormats
{
    public class GR2_Mesh_Piece
    {
        public uint startIndex = 0;
        public uint numPieceFaces = 0;
        public int matID = -1;
        public int index = 0;
        public GR2_Bounding_Box piece_box;
        public GR2_Mesh mesh;

        public GR2_Mesh_Piece(BinaryReader br, int _, GR2_Mesh parent)
        {
            mesh = parent;
            startIndex = br.ReadUInt32();
            numPieceFaces = br.ReadUInt32();
            matID = br.ReadInt32();
            index = br.ReadInt32();
            piece_box = new GR2_Bounding_Box(br);
        }
    }
}
