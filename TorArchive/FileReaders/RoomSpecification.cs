using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TorLib.FileReaders
{
    public class RoomSpecification
    {
        private enum FileRegion
        {
            Base,
            Instances,
            Visible,
            Settings,
            Unknown
        }

        public const string INSTANCE_ID_KEY = "_Id";
        public const string INSTANCE_ASSET_ID_KEY = "_AssetId";
        public const string INSTANCE_ROOM_KEY = "_Room";

        private static Regex sectionParser = new Regex("\\[(\\w+)\\]");
        private static Regex instanceHeaderParser = new Regex("(\\d+)=(\\d+)");
        private static Regex instanceValueParser = new Regex("\\.([^=]+)=([^\\s]*)");

        private File File { get; set; }
        public string RoomName { get; private set; }
        public AreaSpecification Area { get; private set; }
        // public Dictionary<long, Dictionary<string, string>> InstancesById { get; set; }
        // public Dictionary<long, List<Dictionary<string, string>>> InstancesByAssetId { get; set; }
        // public Dictionary<long, List<Dictionary<string, string>>> InstancesByParentId { get; set; }
        public Dictionary<long, AssetInstance> InstancesById { get; set; }
        public Dictionary<long, List<AssetInstance>> InstancesByAssetId { get; set; }
        public Dictionary<long, List<AssetInstance>> InstancesByParentId { get; set; }

        public RoomSpecification(File roomDat, string roomName, AreaSpecification area)
        {
            this.Area = area;
            this.RoomName = roomName;
            if (roomDat == null) { throw new ArgumentNullException("roomDat", "File cannot be null"); }
            File = roomDat;
            // Instances = new List<Dictionary<string, string>>();
            InstancesByAssetId = new Dictionary<long, List<AssetInstance>>();
            InstancesById = new Dictionary<long, AssetInstance>();
            InstancesByParentId = new Dictionary<long, List<AssetInstance>>();
        }

        public IEnumerable<AssetInstance> FindInstances(Func<AssetInstance, bool> func)
        {
            return InstancesById.Values.Where(func);
        }

        public IEnumerable<AssetInstance> FindByAssetId(long assetId)
        {
            if (InstancesByAssetId.ContainsKey(assetId))
            {
                return InstancesByAssetId[assetId];
            }
            else
            {
                return new List<AssetInstance>();
            }
        }

        public IEnumerable<AssetInstance> FindByParentInstanceId(long parentInstanceId)
        {
            if (InstancesByParentId.ContainsKey(parentInstanceId))
            {
                return InstancesByParentId[parentInstanceId];
            }
            else
            {
                return new List<AssetInstance>();
            }
        }

        public void Read()
        {
            FileRegion currentSection = FileRegion.Base;
            AssetInstance currentInstance = null;
            string currentLine;
            string trimmedLine;
            using (var fileStream = File.Open())
            using (var reader = new System.IO.StreamReader(fileStream))
            {
                while (!reader.EndOfStream)
                {
                    currentLine = reader.ReadLine();
                    trimmedLine = currentLine.Trim();
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
                            case "instances": currentSection = FileRegion.Instances; break;
                            case "visible": currentSection = FileRegion.Visible; break;
                            case "settings": currentSection = FileRegion.Settings; break;
                            default: currentSection = FileRegion.Unknown; break;
                        }
                        continue;
                    }

                    switch (currentSection)
                    {
                        case FileRegion.Instances:
                            {
                                if (trimmedLine.StartsWith("."))
                                {
                                    // This is a value for the currentInstance
                                    if (trimmedLine.StartsWith(".Position="))
                                    {
                                        var match = instanceValueParser.Match(currentLine);
                                        if (match.Success)
                                        {
                                            currentInstance.HasPosition = true;
                                            string posStr = match.Groups[2].Value;
                                            posStr = posStr.Substring(1, posStr.Length - 2);
                                            string[] coords = posStr.Split(',');

                                            currentInstance.PositionX = float.Parse(coords[0]);
                                            currentInstance.PositionY = float.Parse(coords[1]);
                                            currentInstance.PositionZ = float.Parse(coords[2]);
                                        }
                                    }
                                    else if (trimmedLine.StartsWith(".ParentInstance="))
                                    {
                                        var match = instanceValueParser.Match(currentLine);
                                        if (match.Success)
                                        {
                                            long parentInstanceId = long.Parse(match.Groups[2].Value);
                                            if (parentInstanceId != 0)
                                            {
                                                currentInstance.ParentInstanceId = parentInstanceId;
                                                List<AssetInstance> parentAssetInstances;
                                                if (!InstancesByParentId.TryGetValue(parentInstanceId, out parentAssetInstances))
                                                {
                                                    parentAssetInstances = new List<AssetInstance>();
                                                    InstancesByParentId[parentInstanceId] = parentAssetInstances;
                                                }
                                                parentAssetInstances.Add(currentInstance);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    currentInstance = new AssetInstance();

                                    // Parse Instance Id and Asset Id
                                    var match = instanceHeaderParser.Match(trimmedLine);
                                    if (match.Success)
                                    {
                                        long instanceId = long.Parse(match.Groups[1].Value);
                                        long assetId = long.Parse(match.Groups[2].Value);
                                        currentInstance.Id = instanceId;
                                        currentInstance.AssetId = assetId;
                                        currentInstance.Room = this;
                                        //Instances.Add(currentInstance);
                                        InstancesById.Add(instanceId, currentInstance);
                                        List<AssetInstance> assetInstances;
                                        if (!InstancesByAssetId.TryGetValue(assetId, out assetInstances))
                                        {
                                            assetInstances = new List<AssetInstance>();
                                            InstancesByAssetId[assetId] = assetInstances;
                                        }
                                        assetInstances.Add(currentInstance);
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}
