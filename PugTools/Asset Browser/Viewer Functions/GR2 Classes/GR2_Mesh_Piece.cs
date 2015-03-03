using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public GR2_Mesh_Piece(BinaryReader br, int intCount, GR2_Mesh parent)
        {
            this.mesh = parent;
            this.startIndex = br.ReadUInt32();
            this.numPieceFaces = br.ReadUInt32();
            this.matID = br.ReadInt32();
            this.index = br.ReadInt32();            
            this.piece_box = new GR2_Bounding_Box(br);
        }
    }
}
