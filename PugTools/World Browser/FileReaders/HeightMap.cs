using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TorLib;
using SlimDX;

namespace FileFormats
{
    public class HeightMap
    {
        public bool containsHoles;
        
        public float[,] elevation;
        public byte headerByte;
        public byte[] holesArray;
        public Vector3 position;
        public byte[,] textures;
        public uint width;
        public uint depth;

        public HeightMap(BinaryReader br)
        {
            this.headerByte = br.ReadByte();
            this.width = br.ReadUInt32();
            this.depth = br.ReadUInt32();            
            this.elevation = new float[((int) (this.depth - 1L)) + 1, ((int) (this.width - 1L)) + 1];
            if (this.headerByte == 9)
                this.position = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            this.containsHoles = br.ReadBoolean();
            if (this.containsHoles)
            {
                this.holesArray = br.ReadBytes((int) (int) Math.Round(Math.Ceiling((double) (((double) (this.width * this.depth)) / 8.0))));
            }
              int num15 = (int) (this.width - 1L);
            for (int i = 0; i <= num15; i++)
            {
                this.elevation[0, i] = br.ReadSingle();
                this.elevation[(int) (this.depth - 1L), i] = br.ReadSingle();
            }
            int num16 = (int) (this.depth - 2L);
            for (int j = 1; j <= num16; j++)
            {
                this.elevation[j, 0] = br.ReadSingle();
                this.elevation[j, (int) (this.width - 1L)] = br.ReadSingle();
            }
            long num17 = this.width - 2L;
            for (long k = 1L; k <= num17; k += 1L)
            {
                long num18 = this.depth - 2L;
                for (long m = 1L; m <= num18; m += 1L)
                {
                    this.elevation[(int) m, (int) k] = ((float) br.ReadInt16()) / 512f;
                }
            }
            if (this.headerByte == 1)
            {
                byte num5 = br.ReadByte();
                sbyte[] numArray = new sbyte[(num5 - 1) + 1];
                int num19 = num5 - 1;
                for (int n = 0; n <= num19; n++)
                {
                    numArray[n] = br.ReadSByte();
                }
            }
            else if (this.headerByte == 9)
            {
                byte num8 = br.ReadByte();
                this.textures = new byte[((int) (this.depth - 1L)) + 1, ((int) (this.width - 1L)) + 1];
                long num20 = this.width - 1L;
                for (long num9 = 0L; num9 <= num20; num9 += 1L)
                {
                    long num21 = this.depth - 1L;
                    for (long num10 = 0L; num10 <= num21; num10 += 1L)
                    {
                        byte num11 = br.ReadByte();
                        int num22 = num11;
                        for (int num12 = 1; num12 <= num22; num12++)
                        {
                            byte num13 = br.ReadByte();
                            byte num14 = br.ReadByte();
                            this.textures[(int) num9, (int) num10] = num13;
                        }
                    }
                }
                uint num7 = br.ReadUInt32();
            }
            else
            {
                Console.WriteLine("ERROR: unknown header byte in heightmap: " + this.headerByte.ToString());
            }
        }

        public bool checkNoHole(int i, int j)
        {
            int num3 = ((int) (j * this.width) + i);
            int index = num3 >> 3;
            int num2 = num3 - (index * 8);
            return ((this.holesArray[index] & (1 << num2)) == (1 << num2));
        }        
    }
}
