using System.Collections.Generic;
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
            offsetMeshName = br.ReadUInt32();
            bitFlag1 = br.ReadSingle();
            numPieces = br.ReadUInt16();
            numBones = br.ReadUInt16();
            bitFlag2 = br.ReadUInt16();
            vertexSize = br.ReadUInt16();
            numVerts = br.ReadUInt32();
            numVertIndex = br.ReadUInt32();
            offsetMeshVerts = br.ReadUInt32();
            offsetMeshPieces = br.ReadUInt32();
            offsetMeshVertIndex = br.ReadUInt32();
            offsetMeshBones = br.ReadUInt32();

            meshName = File_Helpers.ReadString(br, offsetMeshName);
        }
    }
}
