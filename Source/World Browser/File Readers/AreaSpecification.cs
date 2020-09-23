using SlimDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TorLib;

namespace FileFormats
{
    public class Area
    {
        // enum FileRegion
        // {
        //     Base,
        //     Rooms,
        //     Assets,
        //     Paths,
        //     Schemes,
        //     TerrainTextures,
        //     DydTextures,
        //     DydChannelParams,
        //     Settings,
        //     Unknown
        // }

        // Binary Format File Regions
        uint roomsOffset;
        uint assetsOffset;
        uint pathsOffset;
        uint schemesOffset;
        uint terrainTexOffset;
        uint dynDetailTexOffset;
        uint dydChnlParamsOffset;
        uint settingsOffset;
        uint guidOffset;

        // Totals
        uint numRooms;
        uint numAssets;

        // RegEx
        private static readonly Regex pathParser = new Regex("\\\\(?:([A-Za-z]):)?(.+)\\.([^\\s]+)");

        // private static readonly Regex sectionParser = new Regex("\\[(\\w+)\\]");
        // private static readonly Regex assetParser = new Regex("^\\s*(\\d+)=\\\\(?:([A-Za-z]):)?(.+)\\.([^\\s]+)\\s*$", RegexOptions.Multiline);

        private readonly Assets assets;
        public Dictionary<ulong, AreaAsset> AssetIdMap { get; set; }
        public Dictionary<string, List<AreaAsset>> AssetsByExtension { get; set; }
        private TorLib.File File { get; set; }
        public ulong Id;
        public string Path;

#pragma warning disable IDE1006
        public List<Room> roomList { get; set; }
#pragma warning restore IDE1006


        public Area(HashFileInfo info, Assets assets)
        {
            this.assets = assets;
            TorLib.File areaDat = info.File;
            File = areaDat ?? throw new ArgumentNullException("areaDat", "File cannot be null");
            AssetIdMap = new Dictionary<ulong, AreaAsset>();
            AssetsByExtension = new Dictionary<string, List<AreaAsset>>();
            roomList = new List<Room>();
            Id = ulong.Parse(info.Directory.Split('/').Last());
            Path = info.Directory;
        }

        public void Read()
        {
            Stream fileStream = File.OpenCopyInMemory();
            BinaryReader br = new BinaryReader(fileStream);

            // 28 header bytes

            // Move to 0x1C
            br.BaseStream.Seek(0x1C, SeekOrigin.Begin);

            // Offsets
            roomsOffset = br.ReadUInt32();
            assetsOffset = br.ReadUInt32();
            pathsOffset = br.ReadUInt32();
            schemesOffset = br.ReadUInt32();
            terrainTexOffset = br.ReadUInt32();
            dynDetailTexOffset = br.ReadUInt32();
            dydChnlParamsOffset = br.ReadUInt32();
            settingsOffset = br.ReadUInt32();
            guidOffset = br.ReadUInt32();

            // 22 unknown bytes

            // Move to roomsOffset
            br.BaseStream.Seek(roomsOffset, SeekOrigin.Begin);

            numRooms = br.ReadUInt32();

            for (int i = 0; i < numRooms; i++)
            {
                uint roomNameLength = br.ReadUInt32();
                string roomFileName = File_Helpers.ReadString(br, (uint)br.BaseStream.Position);
                string roomFilePath = string.Format("{0}/{1}.dat", Path, roomFileName);

                TorLib.File roomFile = assets.FindFile(roomFilePath);

                if (roomFile == null)
                {
                    Console.WriteLine("Cannot find area room file: {0}", roomFilePath);
                    break;
                }

                Room Room = new Room(roomFile, roomFileName, this);
                Room.Read();

                roomList.Add(Room);

                // Move to beginning of next room
                br.BaseStream.Seek(roomNameLength, SeekOrigin.Current);
            }

            // Move to assetsOffset
            br.BaseStream.Seek(assetsOffset, SeekOrigin.Begin);

            numAssets = br.ReadUInt32();

            for (int i = 0; i < numAssets; i++)
            {
                var assetId = br.ReadUInt64();
                var assetNameLength = br.ReadUInt32();
                var assetInfo = pathParser.Match(File_Helpers.ReadString(br, (uint)br.BaseStream.Position));

                string assetPath = assetInfo.Groups[2].Value;
                string assetExtn = assetInfo.Groups[3].Value;

                AreaAsset asset = new AreaAsset
                {
                    Id = assetId,
                    Path = assetPath
                };

                if (!string.IsNullOrEmpty(assetInfo.Groups[1].Value))
                {
                    asset.EncounterIndex = assetInfo.Groups[1].Value.ToLower();
                }

                AssetIdMap.Add(asset.Id, asset);

                if (!AssetsByExtension.TryGetValue(assetExtn, out List<AreaAsset> pathList))
                {
                    pathList = new List<AreaAsset>();
                    AssetsByExtension.Add(assetExtn, pathList);
                }

                pathList.Add(asset);

                // Move to beginning of next asset
                br.BaseStream.Seek(assetNameLength, SeekOrigin.Current);
            }

            // Move to pathsOffset
            br.BaseStream.Seek(pathsOffset, SeekOrigin.Begin);

            // Move to schemesOffset
            br.BaseStream.Seek(schemesOffset, SeekOrigin.Begin);

            // Move to terrainTexOffset
            br.BaseStream.Seek(terrainTexOffset, SeekOrigin.Begin);

            // Move to dynDetailTexOffset
            br.BaseStream.Seek(dynDetailTexOffset, SeekOrigin.Begin);

            // Move to dydChnlParamsOffset
            br.BaseStream.Seek(dydChnlParamsOffset, SeekOrigin.Begin);

            // Move to settingsOffset
            br.BaseStream.Seek(settingsOffset, SeekOrigin.Begin);

            // Move to guidOffset
            br.BaseStream.Seek(guidOffset, SeekOrigin.Begin);
        }
    }
}
