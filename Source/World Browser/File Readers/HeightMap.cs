using System;
using System.IO;
using SlimDX;

namespace FileFormats
{
    public class HeightMap
    {
        public bool hasHoles;
        public float[,] elevation;
        public byte headerBitFlag;
        public byte[] holeMap;
        public byte[,] textures;
        public uint width;
        public uint depth;

        public HeightMap(BinaryReader br)
        {
            headerBitFlag = br.ReadByte();
            width = br.ReadUInt32();
            depth = br.ReadUInt32();
            elevation = new float[depth, width];

            if ((headerBitFlag & 8) != 0)
            {
                float unkX = br.ReadSingle();
                float unkY = br.ReadSingle();
                float unkZ = br.ReadSingle();

                Console.WriteLine("Unknown heightmap floats {0}, {1}, {2}", unkX, unkY, unkZ);
            }

            hasHoles = br.ReadByte() == 1;

            if (hasHoles) holeMap = br.ReadBytes((int)Math.Round(Math.Ceiling((double)(width * depth / 8.0))));

            // Read edges
            for (int i = 0; i <= width - 1; i++)
            {
                elevation[0, i] = br.ReadSingle();
                elevation[depth - 1, i] = br.ReadSingle();
            }

            for (int j = 1; j <= depth - 2; j++)
            {
                elevation[j, 0] = br.ReadSingle();
                elevation[j, width - 1] = br.ReadSingle();
            }

            // Read center
            for (int j = 1; j <= width - 2; j++)
            {
                for (int k = 1; k <= depth - 2; k++)
                {
                    elevation[k, j] = br.ReadInt16() / 512f;
                }
            }

            // if (headerBitFlag == 1)
            // {
            //     byte numLayers = br.ReadByte();
            //     sbyte[] layers = new sbyte[numLayers - 1 + 1];

            //     for (int i = 0; i <= numLayers - 1; i++)
            //     {
            //         layers[i] = br.ReadSByte();
            //     }
            // }
            // else if (headerBitFlag == 9)
            // {
            //     br.ReadByte(); // Unknown byte
            //     textures = new byte[depth, width];

            //     for (int i = 0; i <= width - 1; i++)
            //     {
            //         for (int j = 0; j <= depth - 1; j++)
            //         {
            //             byte numInfluences = br.ReadByte();

            //             for (int k = 1; k <= numInfluences; k++)
            //             {
            //                 byte textureId = br.ReadByte();
            //                 br.ReadByte(); // Texture weight
            //                 textures[i, j] = textureId;
            //             }
            //         }
            //     }

            //     br.ReadUInt32(); // Four 00 bytes
            // }
            // else
            // {
            //     Console.WriteLine("ERROR: unknown header byte in heightmap: " + headerBitFlag.ToString());
            // }
        }

        public bool CheckNoHole(int j, int k)
        {
            int index = (int)(k * width) + j;
            int @byte = index >> 3;
            int bit = index - (@byte * 8);
            return (holeMap[@byte] & (1 << bit)) == (1 << bit);
        }
    }
}
