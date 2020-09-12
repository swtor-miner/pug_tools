using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileFormats
{
    public class GR2_Mesh_Vertex_Index
    {
        public ushort index;

        public GR2_Mesh_Vertex_Index(BinaryReader br)
        {
            index = br.ReadUInt16();
        }
    }
}
