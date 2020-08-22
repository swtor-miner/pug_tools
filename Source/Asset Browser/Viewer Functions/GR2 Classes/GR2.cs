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

        // public GR2_Material materialOverride;

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
                //Console.WriteLine("Invalid header" + header.ToString());
                return;
            }
            else
            {
                //Console.WriteLine("Valid header");

                this.version_major = br.ReadUInt32();
                this.version_minor = br.ReadUInt32();
                _ = br.ReadUInt32();

                //Move to 0x10
                br.BaseStream.Seek(0x10, SeekOrigin.Begin);
                //fs.Position = 0x10;

                this.num50Offsets = br.ReadUInt32();
                this.gr2_type = br.ReadUInt32();
                this.numMeshes = br.ReadUInt16();
                this.numMaterials = br.ReadUInt16();
                this.numBones = br.ReadUInt16();
                this.numAttach = br.ReadUInt16();

                //Move to 0x30
                br.BaseStream.Seek(0x30, SeekOrigin.Begin);
                //fs.Position = 0x30;
                global_box = new GR2_Bounding_Box(br);

                //Move to 0x50
                br.BaseStream.Seek(0x50, SeekOrigin.Begin);
                //fs.Position = 0x50;

                this.offset50offset = br.ReadUInt32();
                this.offsetMeshHeader = br.ReadUInt32();
                this.offsetMaterialName = br.ReadUInt32();
                this.offsetBoneStruct = br.ReadUInt32();
                this.offsetAttach = br.ReadUInt32();

                //if this is a skeleton parse the bone structure
                if (this.gr2_type == 2)
                {
                    br.BaseStream.Seek(this.offsetBoneStruct, SeekOrigin.Begin);
                    //fs.Position = this.offsetBoneStruct;

                    for (int intCount = 0; intCount < this.numBones; intCount++)
                    {
                        GR2_Bone_Skeleton skeleton_bone = new GR2_Bone_Skeleton(br, intCount);
                        this.skeleton_bones.Add(skeleton_bone);
                    }
                }

                if (numMeshes > 0)
                {
                    br.BaseStream.Seek(this.offsetMeshHeader, SeekOrigin.Begin);
                    //fs.Position = this.offsetMeshHeader;
                    for (int intCount = 0; intCount < numMeshes; intCount++)
                    {
                        GR2_Mesh mesh = new GR2_Mesh(br)
                        {
                            _GR2 = this
                        };
                        this.meshes.Add(mesh);
                    }

                    foreach (GR2_Mesh mesh in this.meshes)
                    {
                        br.BaseStream.Seek(mesh.offsetMeshPieces, SeekOrigin.Begin);
                        //fs.Position = mesh.offsetMeshPieces;
                        for (int intCount = 0; intCount < mesh.numPieces; intCount++)
                        {
                            GR2_Mesh_Piece piece = new GR2_Mesh_Piece(br, intCount, mesh);
                            mesh.meshPieces.Add(piece);
                        }

                        br.BaseStream.Seek(mesh.offsetMeshVerts, SeekOrigin.Begin);
                        //fs.Position = mesh.offsetMeshVerts;
                        for (int intCount = 0; intCount < mesh.numVerts; intCount++)
                        {
                            GR2_Mesh_Vertex vert = new GR2_Mesh_Vertex(br, mesh.bitFlag2);
                            mesh.meshVerts.Add(vert);
                        }

                        br.BaseStream.Seek(mesh.offsetMeshVertIndex, SeekOrigin.Begin);
                        //fs.Position = mesh.offsetMeshVertIndex;
                        for (int intCount = 0; intCount < mesh.numVertIndex; intCount++)
                        {
                            GR2_Mesh_Vertex_Index index = new GR2_Mesh_Vertex_Index(br);
                            mesh.meshVertIndex.Add(index);
                        }

                        br.BaseStream.Seek(mesh.offsetMeshBones, SeekOrigin.Begin);
                        //fs.Position = mesh.offsetMeshBones;
                        for (int intCount = 0; intCount < mesh.numBones; intCount++)
                        {
                            GR2_Mesh_Bone bone = new GR2_Mesh_Bone(br);
                            mesh.meshBones.Add(bone);
                        }
                    }
                }

                if (numMaterials > 0)
                {
                    br.BaseStream.Seek(this.offsetMaterialName, SeekOrigin.Begin);
                    //fs.Position = this.offsetMaterialName;
                    for (int intCount = 0; intCount < numMaterials; intCount++)
                    {
                        GR2_Material material = new GR2_Material(br);
                        if (globalMaterials != null)
                        {
                            if (!globalMaterials.Keys.Contains(material.materialName))
                                globalMaterials.Add(material.materialName, material);
                        }
                        this.materials.Add(material);
                    }
                }

                if (numAttach > 0)
                {
                    br.BaseStream.Seek(this.offsetAttach, SeekOrigin.Begin);
                    //fs.Position = this.offsetAttach;
                    for (int intCount = 0; intCount < numAttach; intCount++)
                    {
                        GR2_Attachment attach = new GR2_Attachment(br);
                        this.attachments.Add(attach);
                    }
                }
            }
        }

        public Matrix GetTransform()
        {
            Matrix output = Matrix.Identity;
            if (attachMatrix != new Matrix())
                output *= this.attachMatrix;
            return (output * this.transformMatrix);
        }
    }
}

