﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileFormats
{
    public class GR2_Mesh_Vertex
    {
        public float X = 0;
        public float Y = 0;
        public float Z = 0;

        public float boneIndex1;
        public float boneIndex2;
        public float boneIndex3;
        public float boneIndex4;

        public float boneWeight1;
        public float boneWeight2;
        public float boneWeight3;
        public float boneWeight4;

        public float normX;
        public float normY;
        public float normZ;
        public float normW;

        public float tanX;
        public float tanY;
        public float tanZ;
        public float tanW;
        
        public SlimDX.Half texHalfU;
        public SlimDX.Half texHalfV;

        public float texU;
        public float texV;

        public GR2_Mesh_Vertex(BinaryReader br, ushort vertexSize)
        {
            switch (vertexSize)
            {
                case 12:
                    this.X = br.ReadSingle();
                    this.Y = br.ReadSingle();
                    this.Z = br.ReadSingle();
                    break;
                case 24:
                    this.X = br.ReadSingle();
                    this.Y = br.ReadSingle();
                    this.Z = br.ReadSingle();

                    this.normX = File_Helpers.byteToFloat(br.ReadByte());
                    this.normY = File_Helpers.byteToFloat(br.ReadByte());
                    this.normZ = File_Helpers.byteToFloat(br.ReadByte());
                    this.normW = File_Helpers.byteToFloat(br.ReadByte());

                    this.tanX = File_Helpers.byteToFloat(br.ReadByte());
                    this.tanY = File_Helpers.byteToFloat(br.ReadByte());
                    this.tanZ = File_Helpers.byteToFloat(br.ReadByte());
                    this.tanW = File_Helpers.byteToFloat(br.ReadByte());

                    this.texHalfU.RawValue = br.ReadUInt16();
                    this.texHalfV.RawValue = br.ReadUInt16();
                    float[] texFloats24 = SlimDX.Half.ConvertToFloat(new SlimDX.Half[] { this.texHalfU, this.texHalfV });
                    this.texU = texFloats24[0];
                    this.texV = texFloats24[1];
                    
                    break;
                case 32:
                    this.X = br.ReadSingle();
                    this.Y = br.ReadSingle();
                    this.Z = br.ReadSingle();

                    this.boneWeight1 = File_Helpers.byteToFloat(br.ReadByte());;
                    this.boneWeight2 = File_Helpers.byteToFloat(br.ReadByte());;
                    this.boneWeight3 = File_Helpers.byteToFloat(br.ReadByte());;
                    this.boneWeight4 = File_Helpers.byteToFloat(br.ReadByte());;
                    this.boneIndex1 = File_Helpers.byteToFloat(br.ReadByte());;
                    this.boneIndex2 = File_Helpers.byteToFloat(br.ReadByte());;
                    this.boneIndex3 = File_Helpers.byteToFloat(br.ReadByte());;
                    this.boneIndex4 = File_Helpers.byteToFloat(br.ReadByte());;

                    this.normX = File_Helpers.byteToFloat(br.ReadByte());
                    this.normY = File_Helpers.byteToFloat(br.ReadByte());
                    this.normZ = File_Helpers.byteToFloat(br.ReadByte());
                    this.normW = File_Helpers.byteToFloat(br.ReadByte());

                    this.tanX = File_Helpers.byteToFloat(br.ReadByte());
                    this.tanY = File_Helpers.byteToFloat(br.ReadByte());
                    this.tanZ = File_Helpers.byteToFloat(br.ReadByte());
                    this.tanW = File_Helpers.byteToFloat(br.ReadByte());
                    
                    this.texHalfU.RawValue = br.ReadUInt16();
                    this.texHalfV.RawValue = br.ReadUInt16();
                    float[] texFloats32 = SlimDX.Half.ConvertToFloat(new SlimDX.Half[] { this.texHalfU, this.texHalfV });
                    this.texU = texFloats32[0];
                    this.texV = texFloats32[1];

                    break;
                case 36:
                    this.X = br.ReadSingle();
                    this.Y = br.ReadSingle();
                    this.Z = br.ReadSingle();

                    this.boneWeight1 = File_Helpers.byteToFloat(br.ReadByte());;
                    this.boneWeight2 = File_Helpers.byteToFloat(br.ReadByte());;
                    this.boneWeight3 = File_Helpers.byteToFloat(br.ReadByte());;
                    this.boneWeight4 = File_Helpers.byteToFloat(br.ReadByte());;
                    this.boneIndex1 = File_Helpers.byteToFloat(br.ReadByte());;
                    this.boneIndex2 = File_Helpers.byteToFloat(br.ReadByte());;
                    this.boneIndex3 = File_Helpers.byteToFloat(br.ReadByte());;
                    this.boneIndex4 = File_Helpers.byteToFloat(br.ReadByte());;

                    this.normX = File_Helpers.byteToFloat(br.ReadByte());
                    this.normY = File_Helpers.byteToFloat(br.ReadByte());
                    this.normZ = File_Helpers.byteToFloat(br.ReadByte());
                    this.normW = File_Helpers.byteToFloat(br.ReadByte());

                    this.tanX = File_Helpers.byteToFloat(br.ReadByte());
                    this.tanY = File_Helpers.byteToFloat(br.ReadByte());
                    this.tanZ = File_Helpers.byteToFloat(br.ReadByte());
                    this.tanW = File_Helpers.byteToFloat(br.ReadByte());

                    this.texHalfU.RawValue = br.ReadUInt16();
                    this.texHalfV.RawValue = br.ReadUInt16();
                    float[] texFloats36 = SlimDX.Half.ConvertToFloat(new SlimDX.Half[] { this.texHalfU, this.texHalfV });
                    this.texU = texFloats36[0];
                    this.texV = texFloats36[1];

                    //unknown
                    br.ReadByte();
                    br.ReadByte();
                    br.ReadByte();
                    br.ReadByte();
                    break;
                default:
                    throw new GR2_Vertex_Size_Exception("Invalid Vertex Size");
                    
            }
        }
    }


    public class GR2_Vertex_Size_Exception : Exception
    {
        public GR2_Vertex_Size_Exception(string message) : base(message) { }
    }
}
