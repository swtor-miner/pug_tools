using File = TorLib.File;
using SlimDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace FileFormats
{
    public class Room
    {
        private File File { get; set; }
        public string RoomName { get; private set; }
        public Area Area { get; private set; }

        public Dictionary<ulong, AssetInstance> InstancesById { get; set; }
        public Dictionary<ulong, List<AssetInstance>> InstancesByAssetId { get; set; }
        public Dictionary<ulong, List<AssetInstance>> InstancesByParentId { get; set; }

        public Room(File roomDat, string roomName, Area area)
        {
            this.Area = area;
            RoomName = roomName;
            File = roomDat ?? throw new ArgumentNullException("roomDat", "File cannot be null");

            // Instances = new List<Dictionary<string, string>>();

            InstancesByAssetId = new Dictionary<ulong, List<AssetInstance>>();
            InstancesById = new Dictionary<ulong, AssetInstance>();
            InstancesByParentId = new Dictionary<ulong, List<AssetInstance>>();
        }

        public void Read()
        {
            Stream fileStream = File.OpenCopyInMemory();
            BinaryReader br = new BinaryReader(fileStream);

            Console.WriteLine(File.FilePath);

            // Move to 0x1C
            br.BaseStream.Seek(0x1C, SeekOrigin.Begin);
            uint instancesOffset = br.ReadUInt32();
            uint visibleOffset = br.ReadUInt32();
            uint settingsOffset = br.ReadUInt32();

            // INSTANCES ==========================================================================
            // Move to instancesOffset
            br.BaseStream.Seek(instancesOffset, SeekOrigin.Begin);

            uint numInstances = br.ReadUInt32();

            for (int i = 0; i < numInstances; i++)
            {
                // Move 0x05 bytes ahead
                br.BaseStream.Seek(0x05, SeekOrigin.Current);

                ulong instanceId = br.ReadUInt64();
                ulong assetId = br.ReadUInt64();

                AssetInstance currentInstance = new AssetInstance(instanceId, assetId, Area);

                br.ReadByte();
                br.ReadUInt32(); // var numProperties = br.ReadUInt32();

                var propertiesLength = br.ReadUInt32();
                var propertiesEnd = br.BaseStream.Position + propertiesLength;

                br.ReadByte();

                while (br.BaseStream.Position < propertiesEnd)
                {
                    uint type = br.ReadByte();
                    uint name = br.ReadUInt32();

                    currentInstance.AddProperty(ref br, name, type);
                }

                // currentInstance.ReadHeightMap();

                currentInstance.CalculateTransform();

                InstancesById.Add(instanceId, currentInstance);

                if (!InstancesByAssetId.TryGetValue(assetId, out List<AssetInstance> assetInstances))
                {
                    assetInstances = new List<AssetInstance>();
                    InstancesByAssetId[assetId] = assetInstances;
                }

                assetInstances.Add(currentInstance);
            }

            // VISIBLE ============================================================================
            // Move to visibleOffset
            br.BaseStream.Seek(visibleOffset, SeekOrigin.Begin);

            uint numVisible = br.ReadUInt32();

            for (int i = 0; i < numVisible; i++)
            {
                var visibleLength = br.ReadUInt32();
                var visible = ReadWString(br, br.BaseStream.Position, visibleLength);
                _ = visible;
            }

            // SETTINGS ===========================================================================
            // Move to settngsOffset
            br.BaseStream.Seek(settingsOffset, SeekOrigin.Begin);
        }

        private string ReadWString(BinaryReader br, long off, uint len)
        {
            br.BaseStream.Seek(off, SeekOrigin.Begin);

            ushort s;
            string str = "";

            while (len > 0 && (s = br.ReadUInt16()) != 0x00)
            {
                str += BitConverter.GetBytes(s).First();
                len--;
            }

            return str;
        }
    }
}
