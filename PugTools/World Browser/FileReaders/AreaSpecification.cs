using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TorLib;

namespace FileFormats
{
    public class AreaSpecification
    {
        enum FileRegion
        {
            Base,
            Rooms,
            Assets,
            Paths,
            Schemes,
            TerrainTextures,
            DydTextures,
            DydChannelParams,
            Settings,
            Unknown
        }

        private static Regex sectionParser = new Regex("\\[(\\w+)\\]");
        private static Regex assetParser = new Regex("^\\s*(\\d+)=\\\\(?:([A-Za-z]):)?(.+)\\.([^\\s]+)\\s*$", RegexOptions.Multiline);

        private File File { get; set; }
        //public Dictionary<long, string> AssetIdMap { get; set; }
        public Dictionary<long, AreaAsset> AssetIdMap { get; set; }
        public Dictionary<string, List<AreaAsset>> AssetsByExtension { get; set; }
        public List<RoomSpecification> Rooms { get; set; }
        public ulong Id;
        public string Path;

        private Assets _assets;

        public AreaSpecification(HashFileInfo info, Assets assets)
        {
            _assets = assets;
            File areaDat = info.file;
            if (areaDat == null) { throw new ArgumentNullException("areaDat", "File cannot be null"); }
            File = areaDat;
            AssetIdMap = new Dictionary<long, AreaAsset>();
            AssetsByExtension = new Dictionary<string, List<AreaAsset>>();
            Rooms = new List<RoomSpecification>();
            Id = ulong.Parse(info.Directory.Split('/').Last());
            Path = info.Directory;
        }

        public IEnumerable<AssetInstance> FindInstances(Func<AssetInstance, bool> predicate)
        {
            List<AssetInstance> results = new List<AssetInstance>();
            foreach (var room in this.Rooms)
            {
                results.AddRange(room.FindInstances(predicate));
            }
            return results;
        }

        public IEnumerable<AssetInstance> FindByAssetId(long assetId)
        {
            List<AssetInstance> results = new List<AssetInstance>();
            foreach (var room in this.Rooms)
            {
                results.AddRange(room.FindByAssetId(assetId));
            }
            return results;
        }

        public void Read()
        {
            FileRegion currentSection = FileRegion.Base;
            string currentLine;
            using (var fileStream = File.Open())
            using (var reader = new System.IO.StreamReader(fileStream))
            {
                while (!reader.EndOfStream)
                {
                    currentLine = reader.ReadLine();
                    if (currentLine.StartsWith("!")) { continue; } // This line is a comment
                    if (currentLine.StartsWith("["))
                    {
                        var match = sectionParser.Match(currentLine);
                        string sectionName = null;
                        if (match.Success)
                        {
                            sectionName = match.Groups[1].Value;
                        }
                        if (sectionName != null) { sectionName = sectionName.ToLower(); }

                        switch (sectionName)
                        {
                            case "assets": currentSection = FileRegion.Assets; break;
                            case "paths": currentSection = FileRegion.Paths; break;
                            case "rooms": currentSection = FileRegion.Rooms; break;
                            case "schemes": currentSection = FileRegion.Schemes; break;
                            case "terraintextures": currentSection = FileRegion.TerrainTextures; break;
                            case "dydtextures": currentSection = FileRegion.DydTextures; break;
                            case "dydchannelparams": currentSection = FileRegion.DydChannelParams; break;
                            case "settings": currentSection = FileRegion.Settings; break;
                            default: currentSection = FileRegion.Unknown; break;
                        }
                        continue;
                    }

                    switch (currentSection)
                    {
                        case FileRegion.Assets:
                            {
                                var assetInfo = assetParser.Match(currentLine);
                                if (!assetInfo.Success)
                                {
                                    continue;
                                }

                                AreaAsset asset = new AreaAsset();
                                asset.Id = long.Parse(assetInfo.Groups[1].Value);
                                if (!String.IsNullOrEmpty(assetInfo.Groups[2].Value))
                                {
                                    asset.EncounterIndex = assetInfo.Groups[2].Value.ToLower();
                                }
                                asset.Path = assetInfo.Groups[3].Value;
                                string ext = assetInfo.Groups[4].Value;
                                AssetIdMap.Add(asset.Id, asset);
                                List<AreaAsset> pathList;
                                if (!AssetsByExtension.TryGetValue(ext, out pathList))
                                {
                                    pathList = new List<AreaAsset>();
                                    AssetsByExtension.Add(ext, pathList);
                                }
                                pathList.Add(asset);
                            }
                            break;
                        case FileRegion.Rooms:
                            var roomFileName = currentLine.Trim();                            
                            string roomFilePath = String.Format("{0}/{1}.dat", Path, roomFileName);
                            File roomFile = _assets.FindFile(roomFilePath);
                            if (roomFile == null)
                            {
                                Console.WriteLine("Cannot find area room file: {0}", roomFilePath);
                                break;
                            }

                            RoomSpecification roomSpec = new RoomSpecification(roomFile, roomFileName, this);
                            roomSpec.Read();
                            this.Rooms.Add(roomSpec);
                            break;
                    }
                }
            }
        }
    }
}
