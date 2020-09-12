using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TorLib;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;
using GomLib.ModelLoader;

namespace GomLib
{
    public class DataObjectModel : IDisposable
    {
        public string Version { get; set; }
        private Dictionary<int, DomTypeLoaders.IDomTypeLoader> typeLoaderMap = new Dictionary<int, DomTypeLoaders.IDomTypeLoader>();
        public Dictionary<Type, Dictionary<string, DomType>> nodeLookup = new Dictionary<Type, Dictionary<string, DomType>>();
        public Dictionary<ulong, DomType> DomTypeMap { get; set; }
        private DomTypeLoaders.FileInstanceLoader prototypeLoader = new DomTypeLoaders.FileInstanceLoader();
        private Dictionary<string, ulong> StoredNameMap { get; set; }
        private Dictionary<ulong, string> StoredIdMap { get; set; }
        public Dictionary<string, HashSet<string>> NamedMap { get; set; }
        public Dictionary<string, HashSet<ulong>> UnNamedMap { get; set; }

        private List<string> BucketFiles { get; set; }

        private bool Loaded = false;
        private bool disposed = false;

        private void AddTypeLoader(DomTypeLoaders.IDomTypeLoader loader)
        {
            var type = loader.SupportedType;
            typeLoaderMap.Add(type, loader);
        }

        public Assets _assets;

        public DataObjectModel(Assets assets)
        {
            _assets = assets;

            BucketFiles = new List<string>();
            DomTypeMap = new Dictionary<ulong, DomType>();
            AddTypeLoader(new DomTypeLoaders.EnumLoader());
            AddTypeLoader(new DomTypeLoaders.FieldLoader());
            AddTypeLoader(new DomTypeLoaders.AssociationLoader());
            AddTypeLoader(new DomTypeLoaders.ClassLoader());
            AddTypeLoader(new DomTypeLoaders.InstanceLoader());

            StoredNameMap = new Dictionary<string, ulong>();
            StoredIdMap = new Dictionary<ulong, string>();
            NamedMap = new Dictionary<string, HashSet<string>>();
            UnNamedMap = new Dictionary<string, HashSet<ulong>>();
        }

        public Data data;
        public StringTable stringTable;
        public AMI ami;

        public GomTypeLoader gomTypeLoader;
        public ScriptObjectReader scriptObjectReader;

        //ADD NEW MODELS HERE
        public AbilityLoader abilityLoader;
        public AbilityPackageLoader abilityPackageLoader;
        public AchievementLoader achievementLoader;
        public AchievementCategoryLoader achievementCategoryLoader;
        public AdvancedClassLoader advancedClassLoader;
        public AppearanceLoader appearanceLoader;
        public AreaLoader areaLoader;
        public ClassSpecLoader classSpecLoader;
        public CodexLoader codexLoader;
        public CollectionLoader collectionLoader;
        public CompanionLoader companionLoader;
        public NewCompanionLoader newCompanionLoader;
        public ConquestLoader conquestLoader;
        public ConversationLoader conversationLoader;
        public DecorationLoader decorationLoader;
        public DisciplineLoader disciplineLoader;
        public EncounterLoader encounterLoader;
        public ItemLoader itemLoader;
        public MapNoteLoader mapNoteLoader;
        public MtxStorefrontEntryLoader mtxStorefrontEntryLoader;
        public NpcLoader npcLoader;
        public PackageAbilityLoader packageAbilityLoader;
        public PlaceableLoader placeableLoader;
        public QuestBranchLoader questBranchLoader;
        public QuestLoader questLoader;
        public QuestStepLoader questStepLoader;
        public QuestTaskLoader questTaskLoader;
        public SCFFColorOptionLoader scFFColorOptionLoader;
        public SCFFComponentLoader scFFComponentLoader;
        public SCFFPatternLoader scFFPatternLoader;
        public SCFFShipLoader scFFShipLoader;
        public SchematicLoader schematicLoader;
        public SpawnerLoader spawnerLoader;
        public Models.StatData statData;
        public Models.FactionData factionData;
        //public Models.StatExtensions statExtensions;
        public StrongholdLoader strongholdLoader;
        public TalentLoader talentLoader;
        public SetBonusLoader setBonusLoader;
        public CodexCatByFactionLoader cdxCatTotalsLoader;
        public SchematicVariationLoader schemVariationLoader;
        public LegacyTitleLoader legacyTitleLoader;
        public ReputationGroupLoader reputationGroupLoader;
        public ReputationRankLoader reputationRankLoader;
        public DetailedAppearanceColorLoader detailedAppearanceColorLoader;
        public Models.EnhancementData enhanceData;
        public Models.SocialTierData socialTierData;
        public Models.AlignmentData alignmentData;
        public Models.GroupFinderContentData groupFinderContentData;
        public PlayerTitleLoader playerTitleLoader;
        public FileLoaders.AreaDatLoader areaDatLoader;
        public FileLoaders.RoomDatLoader roomDatLoader;

        private void InitializeModelLoaders()
        {
            scriptObjectReader = new ScriptObjectReader(this);
            data = new Data(this);
            stringTable = new StringTable(this);
            ami = new AMI(this);
            //ami.Load();

            //ADD NEW MODELS HERE
            abilityLoader = new AbilityLoader(this);
            abilityPackageLoader = new AbilityPackageLoader(this);
            achievementLoader = new AchievementLoader(this);
            achievementCategoryLoader = new AchievementCategoryLoader(this);
            advancedClassLoader = new AdvancedClassLoader(this);
            appearanceLoader = new AppearanceLoader(this);
            areaLoader = new AreaLoader(this);
            classSpecLoader = new ClassSpecLoader(this);
            codexLoader = new CodexLoader(this);
            mtxStorefrontEntryLoader = new MtxStorefrontEntryLoader(this);
            collectionLoader = new CollectionLoader(this);
            companionLoader = new CompanionLoader(this);
            newCompanionLoader = new NewCompanionLoader(this);
            conquestLoader = new ConquestLoader(this);
            conversationLoader = new ConversationLoader(this);
            decorationLoader = new DecorationLoader(this);
            disciplineLoader = new DisciplineLoader(this);
            encounterLoader = new EncounterLoader(this);
            itemLoader = new ItemLoader(this);
            mapNoteLoader = new MapNoteLoader(this);
            npcLoader = new NpcLoader(this);
            packageAbilityLoader = new PackageAbilityLoader(this);
            placeableLoader = new PlaceableLoader(this);
            questBranchLoader = new QuestBranchLoader(this);
            questLoader = new QuestLoader(this);
            questStepLoader = new QuestStepLoader(this);
            questTaskLoader = new QuestTaskLoader(this);
            scFFColorOptionLoader = new SCFFColorOptionLoader(this);
            scFFComponentLoader = new SCFFComponentLoader(this);
            scFFPatternLoader = new SCFFPatternLoader(this);
            scFFShipLoader = new SCFFShipLoader(this);
            schematicLoader = new SchematicLoader(this);
            spawnerLoader = new SpawnerLoader(this);

            //statExtensions = new Models.StatExtensions();
            strongholdLoader = new StrongholdLoader(this);
            talentLoader = new TalentLoader(this);
            setBonusLoader = new SetBonusLoader(this);
            cdxCatTotalsLoader = new CodexCatByFactionLoader(this);
            schemVariationLoader = new SchematicVariationLoader(this);
            legacyTitleLoader = new LegacyTitleLoader(this);
            reputationGroupLoader = new ReputationGroupLoader(this);
            reputationRankLoader = new ReputationRankLoader(this);
            detailedAppearanceColorLoader = new DetailedAppearanceColorLoader(this);
            playerTitleLoader = new PlayerTitleLoader(this);

            areaDatLoader = new FileLoaders.AreaDatLoader(this);
            roomDatLoader = new FileLoaders.RoomDatLoader(this);

            Models.Tooltip.Flush();
        }

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
                typeLoaderMap.Clear();
                nodeLookup.Clear();
                DomTypeMap = null;
                StoredNameMap.Clear();
                StoredNameMap = null;
                UnNamedMap.Clear();
                UnNamedMap = null;
                BucketFiles.Clear();
                if (data != null)
                    data.Dispose();
            }
            disposed = true;
        }

        ~DataObjectModel()
        {
            Dispose(false);
        }

        public string GetStoredTypeName(ulong id)
        {
            if (StoredIdMap.TryGetValue(id, out string result))
            {
                return result;
            }

            return null;
        }
        public ulong GetStoredTypeId(string name)
        {
            StoredNameMap.TryGetValue(name, out ulong id);

            return id;
        }

        private void LoadTypeNames()
        {
            var inFilePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "gom_type_names.xml");
            using (var fs = System.IO.File.OpenRead(inFilePath))
            {
                var doc = new XmlDocument();
                doc.Load(fs);
                var nav = doc.DocumentElement.CreateNavigator();
                foreach (System.Xml.XPath.XPathNavigator node in nav.Select("//gom_type"))
                {
                    node.MoveToAttribute("Id", "");
                    ulong id = ulong.Parse(node.Value);
                    node.MoveToParent();
                    node.MoveToAttribute("name", "");
                    string name = node.Value;

                    StoredNameMap.Add(name, id);
                    StoredIdMap.Add(id, name);
                }
            }
        }

        public void OutputTypeNames(string path)
        {
            // Create XML mapping GomType IDs to names
            var outFilePath = System.IO.Path.Combine(path + "Gom_Fields.xml");
            using (XmlTextWriter writer = new XmlTextWriter(outFilePath, Encoding.UTF8))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Gom_Fields");

                foreach (var nodeTypeMap in nodeLookup)
                {
                    Type domType = nodeTypeMap.Key;
                    if (domType == typeof(GomObject)) { continue; }

                    foreach (var kvp in nodeTypeMap.Value)
                    {
                        DomType t = kvp.Value;
                        string name = kvp.Key;

                        writer.WriteStartElement("Gom_Field");
                        writer.WriteAttributeString("Id", t.Id.ToString());
                        writer.WriteString(name);
                        writer.WriteEndElement();
                        writer.WriteString(Environment.NewLine);
                    }
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public XDocument ReturnTypeNames()
        {
            XElement typeNames = new XElement("Gom_Fields");

            foreach (var nodeTypeMap in nodeLookup)
            {
                Type domType = nodeTypeMap.Key;
                if (domType == typeof(GomObject)) { continue; }

                foreach (var kvp in nodeTypeMap.Value)
                {
                    DomType t = kvp.Value;
                    string name = kvp.Key;

                    typeNames.Add(new XElement("Gom_Field",
                        new XAttribute("Id", t.Id.ToString()),
                        name));
                }
            }
            //typeNames.ReplaceNodes(typeNames.Elements("Gom_Field")
            //.OrderBy(x => (string)x.Attribute("Id")));

            XElement fieldUseInDomClass = new XElement("FieldUseInDomClass");

            CrossLink(); //need to scan nodes to find all values
            foreach (var kvp in UnNamedMap)
            {
                fieldUseInDomClass.Add(new XElement("DomClass",
                    new XAttribute("Id", kvp.Key.ToString()),
                    new XElement("Gom_Fields", new XAttribute("Id", "UnNamed"), kvp.Value.ToList().Select(x => new XElement("Gom_Field", new XAttribute("Id", x))))));
            }
            foreach (var kvp in NamedMap)
            {
                var xe = fieldUseInDomClass.Elements().Where(x => x.Attribute("Id").Value == kvp.Key);
                var ta = new XElement("Gom_Fields", new XAttribute("Id", "Named"), kvp.Value.ToList().Select(x => new XElement("Gom_Field", new XAttribute("Id", x))));
                if (xe.Count() == 0)
                    fieldUseInDomClass.Add(new XElement("DomClass",
                    new XAttribute("Id", kvp.Key.ToString()),
                    ta));
                else
                    xe.First().Add(ta);
            }

            return new XDocument(new XElement("Wrapper", typeNames, fieldUseInDomClass));
        }

        public void Load()
        {
            if (!Loaded)
            {
                LoadTypeNames();
                gomTypeLoader = new GomTypeLoader(this);
                LoadClientGom();
                LoadBuckets();
                LoadPrototypes();
                //Console.WriteLine("Warning: loading of individual (non-bucketed) prototype files is currently disabled");

                Loaded = true;

                statData = new Models.StatData(this);
                factionData = new Models.FactionData(this);
                enhanceData = new Models.EnhancementData(this);
                socialTierData = new Models.SocialTierData(this);
                alignmentData = new Models.AlignmentData(this);
                groupFinderContentData = new Models.GroupFinderContentData(this);
                foreach (DomType t in DomTypeMap.Values)
                {
                    //Console.WriteLine(t.Name);
                    t.Link(this);
                }
                InitializeModelLoaders();
            }
        }

        public void AddCrossLink(long id, string type, ulong reference)
        {
            ulong id2;
            if (Math.Sign(id) == -1) //need to convert negative id to positive id
            {
                id2 = ulong.MaxValue - (ulong)Math.Abs(id) + 1UL;
            }
            else
            {
                id2 = (ulong)id;
            }
            AddCrossLink(id2, type, reference);
        }
        public void AddCrossLink(ulong id, string type, ulong reference)
        {
            GomObject testNode = GetObjectNoLoad(id);
            if (testNode != null)
            {
                if (testNode.References == null) testNode.References = new Dictionary<string, SortedSet<ulong>>();
                if (!testNode.References.ContainsKey(type)) testNode.References.Add(type, new SortedSet<ulong>());
                if (!testNode.References[type].Contains(reference)) testNode.References[type].Add(reference);
            }
        }
        public void AddCrossLink(string name, string type, ulong reference)
        {
            GomObject testNode = GetObjectNoLoad(name);
            if (testNode != null)
            {
                if (testNode.References == null) testNode.References = new Dictionary<string, SortedSet<ulong>>();
                if (!testNode.References.ContainsKey(type)) testNode.References.Add(type, new SortedSet<ulong>());
                if (!testNode.References[type].Contains(reference)) testNode.References[type].Add(reference);
            }
        }
        public void AddCrossLinkRange(ulong id, string type, List<ulong> reference)
        {
            GomObject testNode = GetObjectNoLoad(id);
            if (testNode != null)
            {
                if (testNode.References == null) testNode.References = new Dictionary<string, SortedSet<ulong>>();
                if (!testNode.References.ContainsKey(type)) testNode.References.Add(type, new SortedSet<ulong>());
                testNode.References[type].UnionWith(reference);
                testNode.References[type].Remove(id); //remove self references
            }
        }
        public void AddProtoCrossLink(ulong protoId, ulong id, string type, ulong reference)
        {
            GomObject testNode = GetObjectNoLoad(protoId);
            if (testNode != null)
            {
                if (testNode.ProtoReferences == null) testNode.ProtoReferences = new Dictionary<ulong, Dictionary<string, SortedSet<ulong>>>();
                if (!testNode.ProtoReferences.ContainsKey(id)) testNode.ProtoReferences.Add(id, new Dictionary<string, SortedSet<ulong>>());
                if (!testNode.ProtoReferences[id].ContainsKey(type)) testNode.ProtoReferences[id].Add(type, new SortedSet<ulong>());
                if (!testNode.ProtoReferences[id][type].Contains(reference)) testNode.ProtoReferences[id][type].Add(reference);
            }
        }

        public bool CrossLinked = false;
        public void CrossLink()
        {
            if (CrossLinked) return;

            //foreach (DomType t in DomTypeMap.Values)
            Parallel.ForEach(DomTypeMap.Values, t =>
            {
                if (t.GetType() == typeof(GomObject))
                {
                    GomObject tG = t as GomObject;
                    tG.FindReferences();
                    tG.Unload();
                }
            });
            CrossLinked = true;
        }

        public void Unload() //This unloads Assets allowing the loading of different assets without relaunching
        {
            if (Loaded)
            {
                typeLoaderMap = new Dictionary<int, DomTypeLoaders.IDomTypeLoader>();
                nodeLookup = new Dictionary<Type, Dictionary<string, DomType>>();
                DomTypeMap = new Dictionary<ulong, DomType>();
                prototypeLoader = new DomTypeLoaders.FileInstanceLoader();
                StoredNameMap = new Dictionary<string, ulong>();
                StoredIdMap = new Dictionary<ulong, string>();

                BucketFiles = new List<string>();

                gomTypeLoader = null;
                statData = null;
                factionData = null;
                enhanceData = null;
                socialTierData = null;
                alignmentData = null;
                groupFinderContentData = null;

                //Flush the ModelLoader Stored entries
                scriptObjectReader.Flush();
                data.Flush();
                stringTable.Flush();

                abilityLoader.Flush();
                abilityPackageLoader.Flush();
                achievementLoader.Flush();
                advancedClassLoader.Flush();
                appearanceLoader.Flush();
                areaLoader.Flush();
                classSpecLoader.Flush();
                codexLoader.Flush();
                companionLoader.Flush();
                newCompanionLoader.Flush();
                decorationLoader.Flush();
                disciplineLoader.Flush();
                encounterLoader.Flush();
                itemLoader.Flush();
                mapNoteLoader.Flush();
                mtxStorefrontEntryLoader.Flush();
                npcLoader.Flush();
                packageAbilityLoader.Flush();
                placeableLoader.Flush();
                questLoader.Flush();
                scFFComponentLoader.Flush();
                scFFPatternLoader.Flush();
                scFFShipLoader.Flush();
                schematicLoader.Flush();
                strongholdLoader.Flush();
                talentLoader.Flush();
                setBonusLoader.Flush();
                schemVariationLoader.Flush();
                legacyTitleLoader.Flush();
                reputationGroupLoader.Flush();
                reputationRankLoader.Flush();
                detailedAppearanceColorLoader.Flush();
                playerTitleLoader.Flush();

                /*foreach (var DomEntry in DomTypeMap)
                {
                    if (DomEntry.Value.GetType() == typeof(GomObject))
                    {
                        ((GomObject)DomEntry.Value).Unload();
                    }
                }*/
                GC.Collect();
                Loaded = false;
            }
        }

        private void LoadClientGom()
        {
            File gomFile = _assets.FindFile("/resources/systemgenerated/client.gom");
            using (var fs = gomFile.Open())
            using (var br = new GomBinaryReader(fs, Encoding.UTF8, this))
            {
                // Check DBLB
                int magicNum = br.ReadInt32();
                if (magicNum != 0x424C4244)
                {
                    throw new InvalidOperationException("client.gom does not begin with DBLB");
                }

                br.ReadInt32(); // Skip 4 bytes

                ReadAllItems(br, 8);
            }
        }

        private void LoadBuckets()
        {
            LoadBucketList();
            LoadBucketFiles();
        }

        private void LoadPrototypes()
        {
            File prototypeList = _assets.FindFile("/resources/systemgenerated/prototypes.info");
            using (var fs = prototypeList.Open())
            using (var br = new GomBinaryReader(fs, Encoding.UTF8, this))
            {
                // Check PINF
                int magicNum = br.ReadInt32();
                if (magicNum != 0x464E4950)
                {
                    throw new InvalidOperationException("prototypes.info does not begin with PINF");
                }

                br.ReadInt32(); // Skip 4 bytes

                int numPrototypes = (int)br.ReadNumber();
                int protoLoaded = 0;
                for (var i = 0; i < numPrototypes; i++)
                {
                    ulong protId = br.ReadNumber();
                    byte flag = br.ReadByte();

                    if (flag == 1)
                    {
                        LoadPrototype(protId);
                        protoLoaded++;
                    }
                }

                Console.WriteLine("Loaded {0} prototype files", protoLoaded);
            }
        }

        private void LoadPrototype(ulong id)
        {
            string path = string.Format("/resources/systemgenerated/prototypes/{0}.node", id);
            File protoFile = _assets.FindFile(path);
            if (protoFile == null)
            {
                Console.WriteLine("Unable to find {0}", path);
            }

            using (var fs = protoFile.Open())
            using (var br = new GomBinaryReader(fs, Encoding.UTF8, this))
            {
                // Check PROT
                int magicNum = br.ReadInt32();
                if (magicNum != 0x544F5250)
                {
                    throw new InvalidOperationException(string.Format("{0} does not begin with PROT", path));
                }

                br.ReadInt32(); // Skip 4 bytes

                var proto = (GomObject)prototypeLoader.Load(br);
                proto.Dom_ = this;
                proto.Checksum = protoFile.FileInfo.Checksum;
                if (!DomTypeMap.ContainsKey(proto.Id))
                {
                    DomTypeMap.Add(proto.Id, proto);
                    AddToNameLookup(proto);
                }
            }
        }

        private void LoadBucketList()
        {
            File gomFile = _assets.FindFile("/resources/systemgenerated/buckets.info");
            using (var fs = gomFile.Open())
            using (var br = new GomBinaryReader(fs, Encoding.UTF8, this))
            {
                br.ReadBytes(8); // Skip 8 header bytes

                var c9 = br.ReadByte();
                if (c9 != 0xC9)
                {
                    throw new InvalidOperationException(string.Format("Unexpected character in buckets.info @ offset 0x8 - expected 0xC9 found {0:X2}", c9));
                }

                short numEntries = br.ReadInt16(Endianness.BigEndian);

                for (var i = 0; i < numEntries; i++)
                {
                    string fileName = br.ReadLengthPrefixString();
                    BucketFiles.Add(fileName);
                }
            }
        }

        private void LoadBucketFiles()
        {
            //var bucketFileName = BucketFiles[0];
            foreach (var bucketFileName in BucketFiles)
            {
                string path = string.Format("/resources/systemgenerated/buckets/{0}", bucketFileName);
                File bucketFile = _assets.FindFile(path);
                using (var fs = bucketFile.Open())
                using (var br = new GomBinaryReader(fs, Encoding.UTF8, this))
                {
                    br.ReadBytes(0x24); // Skip 24 header bytes

                    ReadAllItems(br, 0x24);
                }
            }
        }

        public void ReadAllItems(GomBinaryReader br, long offset)
        {

            while (true)
            {
                // Begin Reading Gom Definitions

                int defLength = br.ReadInt32();

                // Length == 0 means we've read them all!
                if (defLength == 0)
                {
                    break;
                }

                //short defFlags = br.ReadInt16();
                //int defType = (defFlags >> 3) & 0x7;
                byte[] defBuffer = new byte[defLength];
                _ = br.ReadInt32(); // 4 blank bytes
                ulong defId = br.ReadUInt64(); // 8-byte type ID
                short defFlags = br.ReadInt16(); // 16-bit flag field
                int defType = (defFlags >> 3) & 0x7;

                //var defData = br.ReadBytes(defLength - 6);
                var defData = br.ReadBytes(defLength - 18);
                Buffer.BlockCopy(defData, 0, defBuffer, 18, defData.Length);

                using (var memStream = new System.IO.MemoryStream(defBuffer))
                using (var defReader = new GomBinaryReader(memStream, Encoding.UTF8, this))
                {
                    if (typeLoaderMap.TryGetValue(defType, out DomTypeLoaders.IDomTypeLoader loader))
                    {
                        var domType = loader.Load(defReader);
                        domType.Dom_ = this;
                        domType.Id = defId;
                        // if (defId == 16141050636868461855)
                        // {
                        //     string sfiino = "";
                        // }
                        if (!DomTypeMap.ContainsKey(domType.Id))
                        {
                            DomTypeMap.Add(domType.Id, domType);
                            string type = domType.GetType().ToString();

                            if (string.IsNullOrEmpty(domType.Name))
                            {
                                if (StoredIdMap.TryGetValue(domType.Id, out string storedTypeName))
                                {
                                    domType.Name = storedTypeName;
                                }
                            }

                            /*if (type != "GomLib.DomEnum" && type != "GomLib.DomAssociation" && type != "GomLib.DomField" && type != "GomLib.DomClass")
                            {
                                var dat = ((GomObject)domType).Data;
                                string pausehere = "";
                            }*/
                            AddToNameLookup(domType);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format("No loader for DomType 0x{1:X} as offset 0x{0:X}", offset, defType));
                    }
                }

                // Read the required number of padding bytes
                int padding = ((8 - (defLength & 0x7)) & 0x7);
                if (padding > 0)
                {
                    br.ReadBytes(padding);
                }

                offset = offset + defLength + padding;
            }
        }

        private void AddToNameLookup(DomType type)
        {
            if (string.IsNullOrEmpty(type.Name)) { return; }

            Type typeType = type.GetType();
            if (!nodeLookup.TryGetValue(typeType, out _))
            {
                Dictionary<string, DomType> nameMap = new Dictionary<string, DomType>();
                nodeLookup[typeType] = nameMap;
            }

            nodeLookup[typeType].Add(type.Name, type);
        }

        public T Get<T>(ulong typeId) where T : DomType
        {
            T result;
            if (!DomTypeMap.TryGetValue(typeId, out DomType t))
            {
                //throw new InvalidOperationException(String.Format("Cannot find DOM type with ID {0}", typeId));
                // Console.WriteLine("Cannot find DOM type with ID 0x{0:X}", typeId);
                return null;
            }

            result = t as T;
            if (result == null)
            {
                //throw new InvalidOperationException(String.Format("Type {0} is not of type {1}", t, typeof(T)));
                Console.WriteLine("Type 0x{0:X} is not of type {1}", t.Id, typeof(T));
            }

            return result;
        }

        public T Get<T>(string name) where T : DomType
        {
            if (name == null)
                return null;
            Type resultType = typeof(T);
            if (!nodeLookup.TryGetValue(resultType, out Dictionary<string, DomType> nameMap))
            {
                return null;
            }

            if (!nameMap.TryGetValue(name, out DomType t))
            {
                return null;
            }

            return t as T;
        }

        public GomObject GetObject(string name)
        {
            GomObject result = Get<GomObject>(name);
            if (result != null) { result.Load(); }
            return result;
        }

        public GomObject GetObject(ulong id)
        {
            GomObject result = Get<GomObject>(id);
            if (result != null) { result.Load(); }
            return result;
        }

        public GomObject GetObjectNoLoad(string name)
        {
            GomObject result = Get<GomObject>(name);
            return result;
        }
        public GomObject GetObjectNoLoad(ulong id)
        {
            GomObject result = Get<GomObject>(id);
            return result;
        }

        public ulong GetObjectId(string name)
        {
            GomObject result = Get<GomObject>(name);
            if (result != null)
                return result.Id;
            return 0;
        }

        public SortedDictionary<string, long> GetAllInstanceNames()
        {
            var results = new SortedDictionary<string, long>();

            Type resultType = typeof(GomObject);
            if (!nodeLookup.TryGetValue(resultType, out Dictionary<string, DomType> nameMap))
            {
                return results;
            }

            foreach (var kvp in nameMap)
            {
                GomObject val = (GomObject)kvp.Value;
                results.Add(kvp.Key, val.Checksum);
            }

            return results;
        }

        public List<GomObject> GetObjectsStartingWith(string txt)
        {
            List<GomObject> results = new List<GomObject>();

            Type resultType = typeof(GomObject);
            if (!nodeLookup.TryGetValue(resultType, out Dictionary<string, DomType> nameMap))
            {
                return results;
            }

            foreach (var kvp in nameMap)
            {
                if (kvp.Key.StartsWith(txt))
                {
                    results.Add((GomObject)kvp.Value);
                }
            }

            return results;
        }

        public void PrintStats(System.IO.TextWriter outStream)
        {
            outStream.WriteLine("Found {0} DOM Types", DomTypeMap.Count);
            foreach (var kvp in DomTypeMap)
            {
                kvp.Value.Print(outStream);
            }
        }

        public void PrintStats()
        {
            PrintStats(Console.Out);
        }
    }
}
