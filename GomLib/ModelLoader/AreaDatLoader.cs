using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GomLib.Models;
using System.IO;

namespace GomLib.FileLoaders
{
    public class AreaDatLoader
    {
        private DataObjectModel _dom;
        public AreaDatLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
        }

        public AreaDat Load(ulong areaId)
        {
            var file = _dom._assets.FindFile(String.Format("/resources/world/areas/{0}/area.dat", areaId));
            if (file == null) return null;
            Stream fileStream = file.OpenCopyInMemory();
            BinaryReader binReader = new BinaryReader(fileStream);
            return Load(new AreaDat(areaId), binReader);
        }

        public AreaDat Load(AreaDat area, BinaryReader br)
        {
            area._dom = _dom;
            int header = br.ReadInt32();
            if (header == 24)
            {
                char c = br.ReadChar();
                StringBuilder formatter = new StringBuilder();
                while (c != '\0')
                {
                    formatter.Append(c);
                    c = br.ReadChar();
                }
                string format = formatter.ToString();
                if (format != "AREA_DAT_BINARY_FORMAT_") return null; //not an area dat file

                br.BaseStream.Position = 0x1C; //Skip to area header

                uint roomOffset = br.ReadUInt32();
                uint assetsOffset = br.ReadUInt32();
                uint pathsOffset = br.ReadUInt32();
                uint schemesOffset = br.ReadUInt32();
                uint terTexOffset = br.ReadUInt32();
                uint DydTexOffset = br.ReadUInt32();
                uint DydChanParamOffset = br.ReadUInt32();
                uint settingsOffset = br.ReadUInt32();

                uint guidOffset = br.ReadUInt32();

                byte[] unknownBytes = br.ReadBytes(0x16); //Always (01 00) repeating

                br.BaseStream.Position = guidOffset;
                area.AreaGuid = br.ReadUInt64();

                //Rooms
                br.BaseStream.Position = roomOffset;
                uint numRooms = br.ReadUInt32();
                area.RoomNames = new List<string>();
                for (uint i = 0; i < numRooms; i++)
                {
                    uint nameLength = br.ReadUInt32();
                    string room = ReadString(br, nameLength);
                    area.RoomNames.Add(room);
                }

                //Assets
                br.BaseStream.Position = assetsOffset;
                uint numAssets = br.ReadUInt32();
                area.Assets = new Dictionary<ulong, string>();
                area.AssetInstances = new Dictionary<ulong, List<AssetInstance>>();
                for (uint i = 0; i < numAssets; i++)
                {
                    ulong assetId = br.ReadUInt64();
                    uint nameLength = br.ReadUInt32();
                    string assetName = ReadString(br, nameLength);
                    if(assetName.Contains(":enc."))
                    {
                        string sofdijhn = "";
                    }
                    if (assetName.Contains(":enc.") || assetName.Contains("\\enc\\") || assetName.Contains("spn") ||assetName.Contains("mpn"))
                        if (!assetName.StartsWith("\\engine\\"))
                            area.Assets.Add(assetId, assetName);
                }

                ////Paths
                //
                ////Schemes
                //br.BaseStream.Position = schemesOffset;
                //uint numSchemes = br.ReadUInt32();
                //for (uint i = 0; i < numSchemes; i++)
                //{
                //    uint nameLength = br.ReadUInt32();
                //    string schemeName = ReadString(br, nameLength);
                //    uint schemeLength = br.ReadUInt32();
                //    string scheme = ReadString(br, schemeLength);
                //    if (scheme.Contains("/"))
                //    {
                //        int idx = 0;
                //        while ((idx = scheme.IndexOf('/', idx)) != -1)
                //        {
                //            int end = scheme.IndexOf('|', idx);
                //            int len = end - idx;
                //            string final = scheme.Substring(idx, len).ToLower();
                //            fileNames.Add(String.Format("/resources{0}.tex", final));
                //            fileNames.Add(String.Format("/resources{0}.dds", final));
                //            fileNames.Add(String.Format("/resources{0}.tiny.dds", final));
                //            idx = end;
                //        }
                //    }
                //}

                ////TERRAINTEXTURES
                //br.BaseStream.Position = terTexOffset;
                //uint numTerTex = br.ReadUInt32();
                //for (uint i = 0; i < numTerTex; i++)
                //{
                //    ulong texId = br.ReadUInt64();
                //    uint nameLength = br.ReadUInt32();
                //    string terTexName = ReadString(br, nameLength);

                //    fileNames.Add(String.Format("/resources/art/shaders/materials/{0}.mat", terTexName.ToLower()));
                //    fileNames.Add(String.Format("/resources/art/shaders/environmentmaterials/{0}.emt", terTexName.ToLower()));
                //}

                ////TERRAINTEXTURES
                //br.BaseStream.Position = DydTexOffset;
                //uint numDydTex = br.ReadUInt32();
                //for (uint i = 0; i < numDydTex; i++)
                //{
                //    uint texId = br.ReadUInt32();
                //    uint nameLength = br.ReadUInt32();
                //    string terTexName = ReadString(br, nameLength);

                //    fileNames.Add(String.Format("/resources/art/shaders/materials/{0}.mat", terTexName.ToLower()));
                //    fileNames.Add(String.Format("/resources/art/shaders/environmentmaterials/{0}.emt", terTexName.ToLower()));
                //}

                ////DYDCHANNELPARAMS

                ////SETTINGS

            }
            else
                return null; //invalid file type

            br.Dispose();
            return area;
        }

        private static string ReadString(BinaryReader br, uint length)
        {
            long curpos = br.BaseStream.Position;
            long endpos = curpos + length;
            char c = br.ReadChar();
            StringBuilder builder = new StringBuilder();
            while (c != '\0' && br.BaseStream.Position < endpos)
            {
                builder.Append(c);
                c = br.ReadChar();
            }
            return builder.ToString();
        }
    }
}
