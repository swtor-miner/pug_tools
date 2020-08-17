using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace FileFormats
{
    public class GR2_Mesh
    {
        public GR2 _GR2;
        public uint offsetMeshName = 0;
        public string meshName = "";
        public float bitFlag1 = 0;
        public ushort numPieces = 0;
        public ushort numBones = 0;
        public ushort bitFlag2 = 0;
        public ushort vertexSize = 0;
        public uint numVerts = 0;
        public uint numVertIndex = 0;
        public uint offsetMeshVerts = 0;
        public uint offsetMeshPieces = 0;
        public uint offsetMeshVertIndex = 0;
        public uint offsetMeshBones = 0;

        public Buffer meshVertBuff;
        public Buffer meshIdxBuff;

        public List<GR2_Mesh_Vertex> meshVerts = new List<GR2_Mesh_Vertex>();
        public List<GR2_Mesh_Piece> meshPieces = new List<GR2_Mesh_Piece>();
        public List<GR2_Mesh_Vertex_Index> meshVertIndex = new List<GR2_Mesh_Vertex_Index>();
        public List<GR2_Mesh_Bone> meshBones = new List<GR2_Mesh_Bone>();
        /*
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                meshVerts.Clear();
                meshPieces.Clear();
                meshVertIndex.Clear();
                meshBones.Clear();

            }
            disposed = true;
        }

        ~GR2_Mesh()
        {
            Dispose(false);
        }
        */
        public GR2_Mesh(BinaryReader br)
        {
            this.offsetMeshName = br.ReadUInt32();
            this.bitFlag1 = br.ReadSingle();
            this.numPieces = br.ReadUInt16();
            this.numBones = br.ReadUInt16();
            this.bitFlag2 = br.ReadUInt16();
            this.vertexSize = br.ReadUInt16();
            this.numVerts = br.ReadUInt32();
            this.numVertIndex = br.ReadUInt32();
            this.offsetMeshVerts = br.ReadUInt32();
            this.offsetMeshPieces = br.ReadUInt32();
            this.offsetMeshVertIndex = br.ReadUInt32();
            this.offsetMeshBones = br.ReadUInt32();

            this.meshName = File_Helpers.ReadString(br, this.offsetMeshName);
        }
    }
}
