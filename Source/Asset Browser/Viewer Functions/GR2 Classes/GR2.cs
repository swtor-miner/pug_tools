using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SlimDX;

namespace FileFormats
{
    public class GR2
    {
        public string filename = "";
        public uint version_major = 0;
        public uint version_minor = 0;
        public uint offsetBNRY = 0;
        public uint num50Offsets = 0;
        public uint gr2_type = 0;
        public ushort numMeshes = 0;
        public ushort numMaterials = 0;
        public ushort numBones = 0;
        public ushort numAttach = 0;

        public GR2_Bounding_Box global_box;

        public uint offset50offset = 0;
        public uint offsetMeshHeader = 0;
        public uint offsetMaterialName = 0;
        public uint offsetBoneStruct = 0;
        public uint offsetAttach = 0;

        public List<GR2_Bone_Skeleton> skeleton_bones = new List<GR2_Bone_Skeleton>();
        public List<GR2_Mesh> meshes = new List<GR2_Mesh>();
        public List<GR2_Material> materials = new List<GR2_Material>();
        public List<GR2_Attachment> attachments = new List<GR2_Attachment>();
        public List<GR2> attachedModels = new List<GR2>();

        public Matrix positionMatrix;
        public Matrix scaleMatrix;
        public Matrix rotationMatrix;
        public Matrix attachMatrix;

        public Matrix transformMatrix;

        public Matrix parentPosMatrix;
        public Matrix parentScaleMatrix;
        public Matrix parentRotMatrix;

        private bool disposed = false;
        public bool enabled = true;

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
                skeleton_bones.Clear();
                meshes.Clear();
                materials.Clear();
                attachments.Clear();

            }
            disposed = true;
        }

        ~GR2()
        {
            Dispose(false);
        }

        public GR2()
        {

        }

        public GR2(BinaryReader br, string filename, Dictionary<string, GR2_Material> globalMaterials = null)
        {
            this.filename = filename;
            ulong header = br.ReadUInt32();

            if (header.ToString("X") != "42574147")
            {
                return;
            }
            else
            {
                version_major = br.ReadUInt32();
                version_minor = br.ReadUInt32();

                //Move to 0x10
                br.BaseStream.Seek(0x10, SeekOrigin.Begin);

                num50Offsets = br.ReadUInt32();
                gr2_type = br.ReadUInt32();
                numMeshes = br.ReadUInt16();
                numMaterials = br.ReadUInt16();
                numBones = br.ReadUInt16();
                numAttach = br.ReadUInt16();

                //Move to 0x30
                br.BaseStream.Seek(0x30, SeekOrigin.Begin);
                global_box = new GR2_Bounding_Box(br);

                //Move to 0x50
                br.BaseStream.Seek(0x50, SeekOrigin.Begin);

                offset50offset = br.ReadUInt32();
                offsetMeshHeader = br.ReadUInt32();
                offsetMaterialName = br.ReadUInt32();
                offsetBoneStruct = br.ReadUInt32();
                offsetAttach = br.ReadUInt32();

                //if this is a skeleton parse the bone structure
                if (gr2_type == 2)
                {
                    br.BaseStream.Seek(offsetBoneStruct, SeekOrigin.Begin);

                    for (int intCount = 0; intCount < numBones; intCount++)
                    {
                        GR2_Bone_Skeleton skeleton_bone = new GR2_Bone_Skeleton(br, intCount);
                        skeleton_bones.Add(skeleton_bone);
                    }
                }

                if (numMeshes > 0)
                {
                    br.BaseStream.Seek(offsetMeshHeader, SeekOrigin.Begin);

                    for (int intCount = 0; intCount < numMeshes; intCount++)
                    {
                        GR2_Mesh mesh = new GR2_Mesh(br)
                        {
                            _GR2 = this
                        };
                        meshes.Add(mesh);
                    }

                    foreach (GR2_Mesh mesh in meshes)
                    {
                        br.BaseStream.Seek(mesh.offsetMeshPieces, SeekOrigin.Begin);

                        for (int intCount = 0; intCount < mesh.numPieces; intCount++)
                        {
                            GR2_Mesh_Piece piece = new GR2_Mesh_Piece(br, intCount, mesh);
                            mesh.meshPieces.Add(piece);
                        }

                        br.BaseStream.Seek(mesh.offsetMeshVerts, SeekOrigin.Begin);

                        for (int intCount = 0; intCount < mesh.numVerts; intCount++)
                        {
                            GR2_Mesh_Vertex vert = new GR2_Mesh_Vertex(br, mesh.bitFlag2);
                            mesh.meshVerts.Add(vert);
                        }

                        br.BaseStream.Seek(mesh.offsetMeshVertIndex, SeekOrigin.Begin);

                        for (int intCount = 0; intCount < mesh.numVertIndex; intCount++)
                        {
                            GR2_Mesh_Vertex_Index index = new GR2_Mesh_Vertex_Index(br);
                            mesh.meshVertIndex.Add(index);
                        }

                        br.BaseStream.Seek(mesh.offsetMeshBones, SeekOrigin.Begin);

                        for (int intCount = 0; intCount < mesh.numBones; intCount++)
                        {
                            GR2_Mesh_Bone bone = new GR2_Mesh_Bone(br);
                            mesh.meshBones.Add(bone);
                        }
                    }
                }

                if (numMaterials > 0)
                {
                    br.BaseStream.Seek(offsetMaterialName, SeekOrigin.Begin);

                    for (int intCount = 0; intCount < numMaterials; intCount++)
                    {
                        GR2_Material material = new GR2_Material(br);
                        if (globalMaterials != null)
                        {
                            if (!globalMaterials.Keys.Contains(material.materialName))
                                globalMaterials.Add(material.materialName, material);
                        }
                        materials.Add(material);
                    }
                }

                if (numAttach > 0)
                {
                    br.BaseStream.Seek(offsetAttach, SeekOrigin.Begin);

                    for (int intCount = 0; intCount < numAttach; intCount++)
                    {
                        GR2_Attachment attach = new GR2_Attachment(br);
                        attachments.Add(attach);
                    }
                }
            }
        }

        public Matrix GetTransform()
        {
            Matrix output = Matrix.Identity;
            if (attachMatrix != new Matrix())
                output *= attachMatrix;
            return output * transformMatrix;
        }
    }
}

