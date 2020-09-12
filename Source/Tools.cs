using GomLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using MessageBox = System.Windows.Forms.MessageBox;

namespace PugTools
{
    public partial class Tools : Form
    {
        private const bool V = true;
        bool sql = false;
        public static bool verbose = true;
        static string outputTypeName = "XML";
        static bool removeUnchangedElements = true;
        static readonly string prefix = ""; //"Verbose";
        static bool Loaded = false;

        public Tools()
        {
            Config.Load();
            InitializeComponent();
            textBoxAssetsFolder.Text = Config.AssetsPath;
            usePTSAssets.Checked = Config.AssetsUsePTS;
            textBoxPrevAssetsFolder.Text = Config.PrevAssetsPath;
            prevUsePTSAssets.Checked = Config.PrevAssetsUsePTS;
            textBoxExtractFolder.Text = Config.ExtractPath;
            CrossLinkDomCheckBox.Checked = Config.CrossLinkDOM;
            comboBoxExtractTypes.Items.AddRange(new object[]
            { "Abilities",
                "Codex",
                "NPCs",
                "Quests",
                "Areas",
                "Collections",
                "Achievements",
                "Cartel Market",
                "Companions",
                "GSF Ships",
                "Items",
                "Item Appearances",
                "Raw GOM",
                "Icons",
                "(Everything)",
                "Conversations",
                // "Filenames",  // Disabled by SWTOR_Miner
                "Talents",
                "String Tables",
                "Schematics",
                "Decorations",
                "Strongholds",
                "Conquests",
                "Advanced Classes",
                "Find New MTX Images",
                "Achievement Categories",
                "Verify Hashes",
                "Tooltips",
                "Set Bonuses",
                "Codex Category Totals",
                "Schematic Variations",
                // "Build Bnk ID Dict",  // Broken, issue passed to Markus for investigation.
                // "Dulfy"  // Dulfy's no longer active so...
            });
            comboBoxExtractTypes.SelectedIndex = 0;
            cbxExtractFormat.SelectedIndex = 3;
            FormOpen = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            textBox5.Enabled = false;
        }

        #region Output Functions
        public static string PrepExtractPath(string filename)
        {
            string subPath = "";
            if (filename.Contains('\\'))
                subPath = filename.Substring(0, filename.LastIndexOf('\\'));
            if (!Directory.Exists(Config.ExtractPath + prefix + subPath)) { Directory.CreateDirectory(Config.ExtractPath + prefix + subPath); }
            return Config.ExtractPath + prefix + filename;
        }

        public static void WriteFile(string content, string filename, bool append)
        {
            string subPath = "";
            if (filename.Contains('\\'))
                subPath = filename.Substring(0, filename.LastIndexOf('\\'));
            if (!Directory.Exists(Config.ExtractPath + prefix + subPath)) { Directory.CreateDirectory(Config.ExtractPath + prefix + subPath); }
            using (StreamWriter file2 = new StreamWriter(Config.ExtractPath + prefix + filename, append))
            {
                file2.Write(content);
            }
            //MessageBox.Show("Operation Complete. File: " + Config.ExtractPath + filename);
        }
        private void WriteFile(XDocument content, string filename, bool append)
        {
            if (!content.Root.IsEmpty) // skip outputting empty XDocuments
            {
                WriteFile(content, filename, append, false);
            }
        }
        private void WriteFile(XDocument content, string filename, bool append, bool trimEmpty)
        {
            if (trimEmpty)
            {
                content.Descendants()
                    .Where(e => e.IsEmpty || string.IsNullOrWhiteSpace(e.Value))
                    .Remove();
            }
            if (content.Root.IsEmpty) return;
            switch (content.Root.Name.ToString())
            {
                case "AdvancedClasses":
                    foreach (var child in content.Root.Elements())
                    {
                        if (child.Descendants().Count() >= 3 || (child.Descendants().Attributes("Status").Count() != 0 || child.Descendants().Attributes("OldValue").Count() != 0))
                            WriteFile(new XDocument(child), string.Format("AdvancedClasses\\{0}{1}.xml", (chkBuildCompare.Checked ? "Changed" : ""), child.Element("Name").Value.Replace(" ", "_")), append, trimEmpty);
                    }
                    return;
            }
            string subPath = "";
            filename = filename.Replace('/', '.');
            if (filename.Contains('\\'))
                subPath = filename.Substring(0, filename.LastIndexOf('\\'));
            if (!Directory.Exists(Config.ExtractPath + prefix + subPath)) { Directory.CreateDirectory(Config.ExtractPath + prefix + subPath); }
            using (StreamWriter file2 = new StreamWriter(Config.ExtractPath + prefix + filename, append))
            {
                content.Save(file2, SaveOptions.None);
            }
            //MessageBox.Show("Operation Complete. File: " + Config.ExtractPath + filename);
        }
        public static void WriteFile(MemoryStream content, string filename)
        {
            string subPath = "";
            filename = filename.Replace('/', '\\');
            if (filename.Contains('\\'))
                subPath = filename.Substring(0, filename.LastIndexOf('\\'));
            if (!Directory.Exists(Config.ExtractPath + prefix + subPath)) { Directory.CreateDirectory(Config.ExtractPath + prefix + subPath); }
            using (FileStream file2 = new FileStream(Config.ExtractPath + prefix + filename, FileMode.OpenOrCreate))
            {
                content.Position = 0;
                content.CopyTo(file2); //this works when writeto doesn't for some streams.
            }
            //MessageBox.Show("Operation Complete. File: " + Config.ExtractPath + filename);
        }

        public void CreateGzip(string filename)
        {
            string filepath = string.Join("", Config.ExtractPath, prefix, filename);
            FlushTempTables(); //need to clear out memory
            Clearlist2();
            Addtolist2("Writing compressed data file");
            if (!File.Exists(filepath))
                return;
            using (FileStream readstream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                if (readstream == null) return;
                if (readstream.Length == 0) return;
                //byte[] byteArray = new byte[readstream.Length - 1];
                //readstream.Read(byteArray, 0, byteArray.Length);     // Reading a 300mb+ files into memory caused issues
                using (FileStream outFileStream = new FileStream(string.Join("", filepath, ".gz"), FileMode.Create, FileAccess.Write))
                using (System.IO.Compression.GZipStream gzip = new System.IO.Compression.GZipStream(outFileStream, System.IO.Compression.CompressionMode.Compress))
                {
                    //gzip.Write(byteArray, 0, byteArray.Length);
                    readstream.CopyTo(gzip);
                }
            }
            File.Delete(filepath);
            Addtolist2("Done.");
        }

        public void DeleteEmptyFile(string filename, int count) //deletes empty files that were created for streaming output
        {
            string filepath = string.Join("", Config.ExtractPath, prefix, filename);
            FileInfo fInfo = new FileInfo(filepath);
            if (fInfo != null)
                if (fInfo.Length == 0 || count == 0)
                    File.Delete(filepath);
        }

        #endregion

        #region Search Box Functions

        public void GetFqn(string itemid)
        {
            Clearlist2();

            LoadData();
            var itmList = currentDom.GetObjectsStartingWith("").Where(obj => obj.Name.Contains(itemid));
            double i = 0;

            double ttl = itmList.Count();

            var append = false;
            string cleanedQuery = string.Join("_", itemid.Split(Path.GetInvalidFileNameChars())).TrimEnd('.');
            var filename = "searchOutput-" + cleanedQuery + ".xml";
            XElement root = new XElement("Root");
            ProcessList(ref itmList, ref root);
            XDocument xmlDoc = new XDocument(root);
            WriteFile(xmlDoc, filename, append);

            if (MessageBox.Show("Output GOM files for analysis?", "GOm file Output", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (var gomItm in itmList)
                {
                    string path = Config.ExtractPath;
                    string file = "\\GOM\\" + gomItm.ToString().Replace("/", ".") + ".xml";
                    if (!Directory.Exists(path + "GOM\\")) { Directory.CreateDirectory(path + "GOM\\"); }
                    WriteFile(new XDocument(gomItm.Print()), file, false, true);
                    i++;
                }
            }
            itmList = null;
            Addtolist("the xml files have been generated there were " + ttl + " objects that matched your criteria");
            MessageBox.Show("the xml files have been generated there were " + ttl + " objects that matched your criteria");
            EnableButtons();
        }

        private void ProcessList(ref IEnumerable<GomObject> itmList, ref XElement root)
        {

            root.Add(new XElement("Abilities"),
                    new XElement("Achievements"),
                    new XElement("CodexEntries"),
                    new XElement("Conversations"),
                    new XElement("Items"),
                    new XElement("Npcs"),
                    new XElement("Quests"),
                    new XElement("Schematics"),
                    new XElement("Talents"));
            foreach (var gomItm in itmList)
            {
                XElement convertedElement = ConvertToXElement(gomItm);
                if (convertedElement != null)
                {
                    switch (convertedElement.Name.LocalName)
                    {
                        case "Item":
                            root.Element("Items").Add(convertedElement);
                            break;
                        case "Ability":
                            root.Element("Abilities").Add(convertedElement);
                            break;
                        case "Codex":
                            root.Element("CodexEntries").Add(convertedElement);
                            break;
                        case "Npc":
                            root.Element("Npcs").Add(convertedElement);
                            break;
                        case "Quest":
                            root.Element("Quests").Add(convertedElement);
                            break;
                        case "Talent":
                            root.Element("Talents").Add(convertedElement);
                            break;
                        case "Achievement":
                            root.Element("Achievements").Add(convertedElement);
                            break;
                        case "Conversation":
                            root.Element("Conversations").Add(convertedElement);
                            break;
                        case "Schematic":
                            root.Element("Schematics").Add(convertedElement);
                            break;
                        default:
                            break;
                    }
                }
            }
            Addtolist(root.Element("Items").Elements("Item").Count() + " matching Items found.");
            root.Element("Items").ReplaceWith(Sort(root.Element("Items")));
            Addtolist(root.Element("Abilities").Elements("Ability").Count() + " matching Abilities found.");
            Addtolist(root.Element("CodexEntries").Elements("Codex").Count() + " matching CodexEntries found.");
            Addtolist(root.Element("Npcs").Elements("Npc").Count() + " matching Npcs found.");
            Addtolist(root.Element("Quests").Elements("Quest").Count() + " matching Quests found.");
            Addtolist(root.Element("Talents").Elements("Talent").Count() + " matching Talents found.");
            Addtolist(root.Element("Achievements").Elements("Achievement").Count() + " matching Achievements found.");
            Addtolist(root.Element("Conversations").Elements("Conversation").Count() + " matching Conversations found.");

            Clearlist2();
        }

        #endregion

        #region Misc Functions

        private GomLib.Models.GameObject LoadGameObject(DataObjectModel dom, GomObject gObject, bool classOverride)
        {
            GomLib.Models.GameObject obj;
            string gomPrefix = gObject.Name.Substring(0, 4);
            switch (gomPrefix)
            {
                case "itm.":
                    obj = new GomLib.Models.Item();
                    dom.itemLoader.Load(obj, gObject);
                    break;
                case "abl.":
                    obj = new GomLib.Models.Ability();
                    if (!gObject.Name.Contains("/"))
                    {
                        dom.abilityLoader.Load(obj, gObject);
                    }
                    break;
                case "npc.":
                    obj = new GomLib.Models.Npc();
                    dom.npcLoader.Load(obj, gObject);
                    break;
                case "qst.":
                    obj = new GomLib.Models.Quest();
                    dom.questLoader.Load(obj, gObject);
                    break;
                case "cdx.":
                    obj = new GomLib.Models.Codex();
                    dom.codexLoader.Load(obj, gObject);
                    break;
                case "cnv.":
                    obj = new GomLib.Models.Conversation();
                    dom.conversationLoader.Load(obj, gObject);
                    break;
                case "ach.":
                    obj = new GomLib.Models.Achievement();
                    dom.achievementLoader.Load(obj, gObject);
                    break;
                case "tal.":
                    obj = new GomLib.Models.Talent();
                    dom.talentLoader.Load(obj, gObject);
                    break;
                case "sche":
                    obj = new GomLib.Models.Schematic();
                    dom.schematicLoader.Load((GomLib.Models.Schematic)obj, gObject);
                    break;
                case "ipp.":
                    //obj = new GomLib.Models.ItemAppearance();
                    obj = (GomLib.Models.ItemAppearance)dom.appearanceLoader.Load(gObject);
                    break;
                case "npp.":
                    //obj = new GomLib.Models.ItemAppearance();
                    obj = dom.appearanceLoader.LoadNpp(gObject);
                    break;
                case "dec.":
                    obj = new GomLib.Models.Decoration();
                    dom.decorationLoader.Load(obj, gObject);
                    break;
                case "apt.":
                    obj = new GomLib.Models.Stronghold();
                    dom.strongholdLoader.Load(obj, gObject);
                    break;
                case "apc.":
                    //obj = new GomLib.Models.AbilityPackage();
                    obj = dom.abilityPackageLoader.Load(gObject);
                    break;
                case "clas":
                    if (classOverride && gObject.Name.StartsWith("class.pc.advanced."))
                        obj = dom.advancedClassLoader.Load(gObject);
                    else
                        // do something else here.
                        obj = dom.classSpecLoader.Load(gObject);
                    //throw new NotImplementedException();
                    break;
                case "nco.":
                    obj = dom.newCompanionLoader.Load(gObject);
                    break;
                case "apn.":
                    obj = dom.abilityPackageLoader.Load(gObject);
                    break;
                case "spn.":
                    obj = dom.spawnerLoader.Load(gObject);
                    break;
                default:
                    throw new NotImplementedException();
            }

            gObject.Unload();
            return obj;
        }

        public void GetAll()
        {
            Clearlist2();

            ExtractCheckForm testFile = new ExtractCheckForm();
            DialogResult result = testFile.ShowDialog();
            if (result == DialogResult.OK)
            {
                //try
                //{ 
                LoadData();

                DisableButtons();
                List<string> extensions = testFile.GetTypes();

                ExportICONS1 = extensions.Contains("ICONS");
                if (extensions.Contains("CDXCAT"))
                {
                    DisableButtons();
                    GetPrototypeObjects("CodexCategoryTotals", "cdxCategoryTotalsPrototype", "cdxFactionToClassToPlanetToTotalLookupList");
                }
                if (extensions.Contains("SBN"))
                {
                    DisableButtons();
                    GetPrototypeObjects("SetBonuses", "itmSetBonusesPrototype", "itmSetBonuses");
                }
                if (extensions.Contains("TORC"))
                {
                    DisableButtons();
                    GetTorc();
                }
                if (extensions.Contains("MISC"))
                {
                    DisableButtons();
                    FindNewMtxImages();
                }
                if (extensions.Contains("STB"))
                {
                    DisableButtons();
                    Addtolist("Getting String Tables.");
                    GetStrings();
                }
                if (extensions.Contains("GOM"))
                {
                    DisableButtons();
                    Addtolist("Getting Raw GOM.");
                    ExportGOM = extensions.Contains("EXP");
                    GetRaw();
                }
                if (extensions.Contains("AC"))
                {
                    DisableButtons();
                    GetObjects("class.pc.advanced", "AdvancedClasses");
                }
                if (extensions.Contains("CNQ"))
                {
                    DisableButtons();
                    GetPrototypeObjects("Conquests", "wevConquestInfosPrototype", "wevConquestTable");
                }
                if (extensions.Contains("ABL"))
                {
                    DisableButtons();
                    GetObjects("abl.", "Abilities");
                }
                if (extensions.Contains("APN"))
                {
                    DisableButtons();
                    GetObjects("apn.", "AbilityPackages");
                }
                if (extensions.Contains("ACH"))
                {
                    DisableButtons();
                    GetObjects("ach.", "Achievements");
                }
                if (extensions.Contains("APT"))
                {
                    DisableButtons();
                    GetObjects("apt.", "Strongholds");
                }
                if (extensions.Contains("SPN"))
                {
                    DisableButtons();
                    GetObjects("spn.", "Spawners");
                }
                if (extensions.Contains("AREA"))
                {
                    DisableButtons();
                    Addtolist("Getting Areas.");
                    GetPrototypeObjects("Areas", "mapAreasDataProto", "mapAreasDataObjectList"); // getAreas();
                }
                if (extensions.Contains("CDX"))
                {
                    DisableButtons();
                    GetObjects("cdx.", "CodexEntries");
                }
                if (extensions.Contains("CLASS"))
                {
                    DisableButtons();
                    GetObjects("class.", "Classes");
                }
                if (extensions.Contains("CNV"))
                {
                    DisableButtons();
                    GetObjects("cnv.", "Conversations");
                }
                if (extensions.Contains("DEC"))
                {
                    DisableButtons();
                    GetObjects("dec.", "Decorations");
                }
                if (extensions.Contains("ABLEFF"))
                {
                    DisableButtons();
                    GetObjects("eff.", "Effects");
                }
                if (extensions.Contains("ITM"))
                {
                    DisableButtons();
                    GetObjects("itm.", "Items");
                }
                if (extensions.Contains("NPC"))
                {
                    DisableButtons();
                    ExportNPP1 = extensions.Contains("NPP");
                    GetObjects("npc.", "Npcs");
                }
                if (extensions.Contains("QST"))
                {
                    DisableButtons();
                    GetObjects("qst.", "Quests");
                }
                if (extensions.Contains("SCHEM"))
                {
                    DisableButtons();
                    GetObjects("schem.", "Schematics");
                }
                if (extensions.Contains("TAL"))
                {
                    DisableButtons();
                    GetObjects("tal.", "Talents");
                }
                if (extensions.Contains("COL"))
                {
                    DisableButtons();
                    GetPrototypeObjects("Collections", "colCollectionItemsPrototype", "colCollectionItemsData");

                    TorLib.HashDictionaryInstance.Instance.Unload();
                    TorLib.HashDictionaryInstance.Instance.Load();
                    TorLib.HashDictionaryInstance.Instance.dictionary.CreateHelpers();
                }
                if (extensions.Contains("CMP"))
                {
                    DisableButtons();
                    GetObjects("nco.", "NewCompanions");
                    GetPrototypeObjects("Companions", "chrCompanionInfo_Prototype", "chrCompanionInfoData");
                }
                if (extensions.Contains("MTX"))
                {
                    DisableButtons();
                    GetPrototypeObjects("MtxStoreFronts", "mtxStorefrontInfoPrototype", "mtxStorefrontData");

                    //Reload hash dict.
                    TorLib.HashDictionaryInstance.Instance.Unload();
                    TorLib.HashDictionaryInstance.Instance.Load();
                    TorLib.HashDictionaryInstance.Instance.dictionary.CreateHelpers();
                }
                if (extensions.Contains("GSF"))
                {
                    DisableButtons();
                    GetPrototypeObjects("Ships", "scFFShipsDataPrototype", "scFFShipsData");
                }
                if (extensions.Contains("IPP"))
                {
                    DisableButtons();
                    Addtolist("Getting Appearances.");
                    GetObjects("ipp.", "ItemAppearances");
                    GetObjects("npp.", "NpcAppearances");
                    //getItemApps();
                }
                if (extensions.Contains("SCHVARI"))
                {
                    DisableButtons();
                    Addtolist("Getting Schematic Variations");
                    GetPrototypeObjects("SchematicVariations", "prfSchematicVariationsPrototype", "prfSchematicVariationMasterList");
                }
                Addtolist("Completed extraction of all supported objects.");
                //}
                //catch (Exception ex)
                //{
                //    //do something here
                //    MessageBox.Show(String.Format("An error occured while loading data. ({0})", ex.HResult));
                //}
            }
            GC.Collect();
            EnableButtons();
        }
        public void GetRaw()
        {
            Clearlist2();
            double i = 0;
            string n = Environment.NewLine;

            LoadData();
            currentDom.OutputTypeNames(Config.ExtractPath + prefix);

            var itmList = currentDom.GetObjectsStartingWith("");//.Where(obj => obj.Name.Contains("."));
            bool append = false;
            string changed = "";
            if (chkBuildCompare.Checked)
            {
                changed = "Changed";
            }

            if (outputTypeName == "Text")
            {
                string filename = "GOM_Items.txt";
                var txtFile = new StringBuilder();
                foreach (var gomItm in itmList)
                {
                    txtFile.Append(gomItm + n);
                    i++;
                }
                WriteFile(txtFile.ToString(), changed + filename, append);
            }
            else
            {
                ProcessGom();
                if (chkBuildCompare.Checked && ExportGOM)
                {
                    ProcessEffectChanges();
                }
                //ProcessGomFields();
                //ProcessGomFields(); //this is providing no useful info currently, and takes a fuckton of time to run.
            }

            //addtolist("The GOM Item List has been generated there are " + i + " GOM Items");
            //MessageBox.Show("the raw list has been generated there are " + i + " Objects");
            EnableButtons();
        }

        private void VerifyHashes()
        {
            Clearlist2();

            LoadData();
            Addtolist("Verifying Game Object Hashes");
            Dictionary<string, bool> gameObjects = new Dictionary<string, bool>
            {
                {"mpn.", true },
                {"ach.", true},
                {"abl.", true},
                {"apn.", true},
                {"cdx.", true},
                {"cnv.", true},
                {"npc.", true},
                {"qst.", true},
                {"tal.", true},
                {"sche", true},
                {"dec.", true},
                {"itm.", true},
                {"apt.", true},
                {"apc.", true},
                {"class.",true},
                {"ipp.",true},
                {"npp.",true},

            };
            for (int f = 0; f < gameObjects.Count; f++)
            {
                var gameObj = gameObjects.ElementAt(f);
                ClearProgress();
                var gomList = currentDom.GetObjectsStartingWith(gameObj.Key).Where(x => !x.Name.Contains("/"));
                var count = gomList.Count();
                int i = 0;
                Addtolist2(string.Format("Checking {0}", gameObj.Key));
                foreach (var gom in gomList)
                {
                    ProgressUpdate(i, count);
                    var itm = GomLib.Models.GameObject.Load(gom);
                    var itm2 = GomLib.Models.GameObject.Load(gom);
                    if (itm == null) continue;
                    if (itm.GetHashCode() != itm2.GetHashCode())
                    {
                        gameObjects[gameObj.Key] = false;
                        Addtolist2(string.Format("Failed: {0}", gameObj.Key));
                        break; //break inner loop
                    }
                    i++;
                }
                if (gameObjects[gameObj.Key])
                    Addtolist2(string.Format("Passed: {0}", gameObj.Key));
            }
            string completeString = "Failed.";
            if (gameObjects.Values.ToList().TrueForAll(x => x))
                completeString = "Passed.";
            Addtolist(completeString);

            Addtolist("Verifying Prototype Game Object Hashes");
            bool failed = false;
            Dictionary<string, string> protoGameObjects = new Dictionary<string, string>
            {
                {"mtxStorefrontInfoPrototype", "mtxStorefrontData"},
                {"colCollectionItemsPrototype", "colCollectionItemsData"},
                {"chrCompanionInfo_Prototype", "chrCompanionInfoData"},
                {"scFFShipsDataPrototype", "scFFShipsData"},
                {"wevConquestInfosPrototype", "wevConquestTable"},
                {"achCategoriesTable_Prototype", "achCategoriesData"}
            };

            for (int f = 0; f < protoGameObjects.Count; f++)
            {
                var gameObj = protoGameObjects.ElementAt(f);
                Dictionary<object, object> currentDataProto = new Dictionary<object, object>();
                GomObject currentDataObject = currentDom.GetObject(gameObj.Key);
                if (currentDataObject != null) //fix to ensure old game assets don't throw exceptions.
                {
                    currentDataProto = currentDataObject.Data.Get<Dictionary<object, object>>(gameObj.Value);
                    currentDataObject.Unload();
                }

                ClearProgress();
                var count = currentDataProto.Count();
                int i = 0;
                Addtolist2(string.Format("Checking {0}", gameObj.Key));
                bool localfail = false;
                foreach (var gom in currentDataProto)
                {
                    ProgressUpdate(i, count);
                    var itm = GomLib.Models.PseudoGameObject.LoadFromProtoName(gameObj.Key, currentDom, gom.Key, (GomObjectData)gom.Value);
                    var itm2 = GomLib.Models.PseudoGameObject.LoadFromProtoName(gameObj.Key, currentDom, gom.Key, (GomObjectData)gom.Value);
                    if (itm == null) continue;
                    if (itm.GetHashCode() != itm2.GetHashCode())
                    {
                        Addtolist2(string.Format("Failed: {0}", gameObj.Key));
                        failed = true;
                        localfail = true;
                        break; //break inner loop
                    }
                    i++;
                }
                if (!localfail)
                    Addtolist2(string.Format("Passed: {0}", gameObj.Key));
            }
            completeString = "Passed.";
            if (failed)
                completeString = "Failed.";
            Addtolist(completeString);

            EnableButtons();
        }

        #region Sorting
        private XElement Sort(XElement element)
        {
            string root = element.Name.ToString();
            switch (root)
            {
                case "Items": return SortItems(element);
                case "Abilities": return SortAbilities(element);
                case "Achievements": return SortAchievements(element);
                case "Npcs": return SortNpcs(element);
                case "CodexEntries": return SortCodices(element);
                case "Conversations": return SortConversations(element);
                case "Quests": return SortQuests(element);
                case "Talents": return SortTalents(element);
                case "MtxStoreFronts": return SortMtxStoreFronts(element);
                case "Collections": return SortCollections(element);
                case "Companions": return SortCompanions(element);
                case "Ships": return SortShips(element);
                case "Schematics": return SortSchematics(element);
                case "Decorations": return SortDefault(element);
                case "ItemAppearances": return SortDefault(element);
                default: return element;
            }
        }

        private XElement SortDefault(XElement element)
        {
            element.ReplaceNodes(element.Elements()
                .OrderBy(x => (string)x.Attribute("Status"))
                .ThenBy(x => (string)x.Element("Fqn")));
            return element;
        }
        #endregion
        #region Load/Unload
        public void Unload()
        {
            if (Loaded)
            {
                Clearlist();
                Addtolist("Unloading Data Object Model.");
                Loaded = false;
                currentAssets = null;
                currentDom = null;
                previousAssets = null;
                previousDom = null;
                Clearlist2();
                Clearlist();
                Addtolist("Unloading Data Object Model. - Done");
            }
        }

        public TorLib.Assets currentAssets;
        public DataObjectModel currentDom;

        public TorLib.Assets previousAssets;
        public DataObjectModel previousDom;

        public static bool FormOpen { get; set; } = V;

        public static List<string> Localizations { get; } = new List<string> {
                "enMale",
                "enFemale",
                "frMale",
                "frFemale",
                "deMale",
                "deFemale"};

        public void LoadData()
        {
            if (!Loaded)
            {
                Clearlist();
                ContinuousProgress();
                Addtolist("Loading Current Data Object Model.");
                bool usePTS = usePTSAssets.Checked;
                currentAssets = TorLib.AssetHandler.Instance.GetCurrentAssets(textBoxAssetsFolder.Text, usePTS);
                //currentAssets = currentAssets.getCurrentAssets(textBoxAssetsFolder.Text, usePTS);                
                currentDom = DomHandler.Instance.GetCurrentDOM(currentAssets);
                currentDom.Version = patchVersion;
                Clearlist();
                Addtolist("Loading Current Data Object Model. - Done");
                if (chkBuildCompare.Checked && textBoxPrevAssetsFolder.Text != "")
                {
                    Addtolist("Loading Previous Data Object Model.");
                    previousAssets = TorLib.AssetHandler.Instance.GetPreviousAssets(textBoxPrevAssetsFolder.Text, prevUsePTSAssets.Checked);
                    //previousAssets = previousAssets.getPreviousAssets(textBoxPrevAssetsFolder.Text, prevUsePTSAssets.Checked);                    
                    previousDom = DomHandler.Instance.GetPreviousDOM(previousAssets);
                    Clearlist();
                    Addtolist("Loading Current Data Object Model. - Done");
                    Addtolist("Loading Previous Data Object Model. - Done");
                }

                if (CrossLinkDomCheckBox.Checked)// MessageBox.Show("Do you want to Cross-link the Data Object Model? - It can be a slow process.\n\nIt will scan each object in the Data Object Model for references to other objects, and store these connections in the referenced object.", "Select Yes or No.", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    Addtolist("Crosslinking Current Data Object Model.");
                    currentDom.CrossLink();
                    //CrossLink(currentDom);
                    Clearlist();
                    Addtolist("Crosslinking Data Object Model. - Done");
                    if (chkBuildCompare.Checked)
                    {
                        Addtolist("Crosslinking Previous Data Object Model.");
                        previousDom.CrossLink();
                        //CrossLink(previousDom);
                        ProcessGomFields();
                        Clearlist();
                        Addtolist("Crosslinking Data Object Model. - Done");
                        Addtolist("Crosslinking Previous Data Object Model. - Done");
                    }
                }
                else if (SmartLinkDomCheckBox.Checked)// MessageBox.Show("Do you want to Cross-link the Data Object Model? - It can be a slow process.\n\nIt will scan each object in the Data Object Model for references to other objects, and store these connections in the referenced object.", "Select Yes or No.", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    Addtolist("Crosslinking Current Data Object Model.");
                    //Smart smart = new Smart(addtolist2);
                    //smart.Link(currentDom);
                    Smart.Link(currentDom, Addtolist2);
                    Clearlist();
                    Addtolist("Crosslinking Data Object Model. - Done");
                    if (chkBuildCompare.Checked)
                    {
                        Addtolist("Crosslinking Previous Data Object Model.");
                        //SmartLink(previousDom);
                        ProcessGomFields();
                        Clearlist();
                        Addtolist("Crosslinking Data Object Model. - Done");
                        Addtolist("Crosslinking Previous Data Object Model. - Done");
                    }
                }
                Loaded = true;
                ClearProgress();
            }
        }


        #endregion

        private void FlushTempTables()
        {
            currentDom.stringTable.Flush();
            currentDom.decorationLoader.Flush();
            currentDom.ami.Flush();
            currentDom.collectionLoader.Flush();
            currentDom.mtxStorefrontEntryLoader.Flush();
            currentDom.companionLoader.Flush();
            if (previousDom != null)
            {
                previousDom.stringTable.Flush();
                previousDom.decorationLoader.Flush();
                previousDom.ami.Flush();
                previousDom.collectionLoader.Flush();
                previousDom.mtxStorefrontEntryLoader.Flush();
                previousDom.companionLoader.Flush();
            }
        }
        #endregion

        private void UnloadCurrent()
        {
            TorLib.AssetHandler.Instance.UnloadCurrentAssets();
            DomHandler.Instance.UnloadCurrentDOM();
            Loaded = false;
        }

        private void UnloadPrevious()
        {
            TorLib.AssetHandler.Instance.UnloadPreviousAssets();
            DomHandler.Instance.UnloadPreviousDOM();
            Loaded = false;
        }

        private void UnloadAll()
        {
            TorLib.AssetHandler.Instance.UnloadAllAssets();
            DomHandler.Instance.UnloadAllDOM();
            Loaded = false;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            SqlCreate();
        }

        public bool PathContainsLiveAssets(string path)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }

            string[] fileList = Directory.GetFiles(path, "swtor_main*.tor");
            if (fileList.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool PathContainsPTSAssets(string path)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }

            string[] fileList = Directory.GetFiles(path, "swtor_test*.tor");
            if (fileList.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }

    public static class DocumentExtensions
    {
        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }
    }
}
