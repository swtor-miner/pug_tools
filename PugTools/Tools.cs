using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using GomLib;
using MessageBox = System.Windows.Forms.MessageBox;

namespace tor_tools
{
    public partial class Tools : Form
    {
        /* code moved to Tools_UI.cs */
        /* code moved to Tools_DB.cs */

        bool sql = false;
        static bool verbose = true;
        static string outputTypeName = "XML";
        static bool removeUnchangedElements = true;
        static string prefix = ""; //"Verbose";
        static bool Loaded = false;
        static bool formOpen = true;

        static List<string> localizations = new List<string> {
                "enMale",
                "enFemale",
                "frMale",
                "frFemale",
                "deMale",
                "deFemale"};
                
        public Tools()
        {
            Config.Load();
            InitializeComponent();
            textBoxAssetsFolder.Text = Config.AssetsPath;
            textBoxExtractFolder.Text = Config.ExtractPath;
            textBoxPrevAssetsFolder.Text = Config.PrevAssetsPath;            
            CrossLinkDomCheckBox.Checked = Config.CrossLinkDOM;
            comboBoxExtractTypes.Items.AddRange(new object[]
            { "Abilities",
                "Codex",
                "Npcs",
                "Quests",
                "Areas",
                "Collections",
                "Achievements",
                "Cartel Market",
                "Companions",
                "Starfighter Ships",
                "Items",
                "Item Appearances",
                "Raw GOM",
                "Icons",
                "(Everything)",
                "Conversations",
                //"Filenames",
                "Talents",
                "String Tables",
                "Schematics",
                "Decorations",
                "Strongholds",
                "Conquests",                
                "Advanced Classes",
                "FindNewMtxImages",
                "AchCategories",
                "testHashes",
                "Tooltips",
                "Set Bonuses",
                "Codex Category Totals",
                "Schematic Variations",
				"BuildBnkIdDict"
            });
            comboBoxExtractTypes.SelectedIndex = 0;
            cbxExtractFormat.SelectedIndex = 0;
            formOpen = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            textBox5.Enabled = false;
        }

        /* code moved to Tools_UI.cs */
        /* code moved to Tools_DB.cs */
        #region Output Functions
        public static string PrepExtractPath(string filename)
        {
            string subPath = "";
            if (filename.Contains('\\'))
                subPath = filename.Substring(0, filename.LastIndexOf('\\'));
            if (!System.IO.Directory.Exists(Config.ExtractPath + prefix + subPath)) { System.IO.Directory.CreateDirectory(Config.ExtractPath + prefix + subPath); }
            return Config.ExtractPath + prefix + filename;
        }

        public static void WriteFile(string content, string filename, bool append)
        {
            string subPath = "";
            if (filename.Contains('\\'))
                subPath = filename.Substring(0, filename.LastIndexOf('\\'));
            if (!System.IO.Directory.Exists(Config.ExtractPath + prefix + subPath)) { System.IO.Directory.CreateDirectory(Config.ExtractPath + prefix + subPath); }
            using (System.IO.StreamWriter file2 = new System.IO.StreamWriter(Config.ExtractPath + prefix + filename, append))
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
                    .Where(e => e.IsEmpty || String.IsNullOrWhiteSpace(e.Value))
                    .Remove();
            }
            if (content.Root.IsEmpty) return;
            switch(content.Root.Name.ToString())
            {
                case "AdvancedClasses":
                    foreach (var child in content.Root.Elements())
                    {
                        if (child.Descendants().Count() >= 3 || (child.Descendants().Attributes("Status").Count() != 0 || child.Descendants().Attributes("OldValue").Count() != 0))
                            WriteFile(new XDocument(child), String.Format("AdvancedClasses\\{0}{1}.xml", (chkBuildCompare.Checked ? "Changed" : ""), child.Element("Name").Value.Replace(" ", "_")), append, trimEmpty);
                    }
                    return;
            }
            string subPath = "";
            filename = filename.Replace('/', '.');
            if (filename.Contains('\\'))
                subPath = filename.Substring(0, filename.LastIndexOf('\\'));
            if (!System.IO.Directory.Exists(Config.ExtractPath + prefix + subPath)) { System.IO.Directory.CreateDirectory(Config.ExtractPath + prefix + subPath); }
            using (System.IO.StreamWriter file2 = new System.IO.StreamWriter(Config.ExtractPath + prefix + filename, append))
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
            if (!System.IO.Directory.Exists(Config.ExtractPath + prefix + subPath)) { System.IO.Directory.CreateDirectory(Config.ExtractPath + prefix + subPath); }
            using (System.IO.FileStream file2 = new System.IO.FileStream(Config.ExtractPath + prefix + filename, FileMode.OpenOrCreate))
            {
                content.Position = 0;
                content.CopyTo(file2); //this works when writeto doesn't for some streams.
            }
            //MessageBox.Show("Operation Complete. File: " + Config.ExtractPath + filename);
        }

        public void CreateGzip(string filename)
        {
            string filepath = String.Join("", Config.ExtractPath, prefix, filename);
            FlushTempTables(); //need to clear out memory
            Clearlist2();
            addtolist2("Writing compressed data file");
            if (!System.IO.File.Exists(filepath))
                return;
            using (FileStream readstream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                if (readstream == null) return;
                if (readstream.Length == 0) return;
                //byte[] byteArray = new byte[readstream.Length - 1];
                //readstream.Read(byteArray, 0, byteArray.Length);     // Reading a 300mb+ files into memory caused issues
                using (FileStream outFileStream = new FileStream(String.Join("", filepath, ".gz"), FileMode.Create, FileAccess.Write))
                using (System.IO.Compression.GZipStream gzip = new System.IO.Compression.GZipStream(outFileStream, System.IO.Compression.CompressionMode.Compress))
                {
                    //gzip.Write(byteArray, 0, byteArray.Length);
                    readstream.CopyTo(gzip);
                }
            }
            addtolist2("Done.");
        }

        public void DeleteEmptyFile(string filename) //deletes empty files that were created for streaming output
        {
            string filepath = String.Join("", Config.ExtractPath, prefix, filename);
            FileInfo fInfo = new FileInfo(filepath);
            if (fInfo != null)
                if (fInfo.Length == 0)
                    System.IO.File.Delete(filepath);
        }

        #endregion

        /* code moved to Tools_Process.cs */

        #region Search Box Functions

        public void getFqn(string itemid)
        {
            Clearlist2();

            LoadData();
            var itmList = currentDom.GetObjectsStartingWith("").Where(obj => obj.Name.Contains(itemid));
            double i = 0;

            double ttl = itmList.Count();

            var append = false;
            string cleanedQuery = String.Join("_", itemid.Split(Path.GetInvalidFileNameChars())).TrimEnd('.');
            var filename = "searchOutput-" + cleanedQuery + ".xml";
            XElement root = new XElement("Root");
            ProcessList(ref itmList, ref root);
            XDocument xmlDoc = new XDocument(root);
            WriteFile(xmlDoc, filename, append);

            if (MessageBox.Show("Output GOM files for analysis?", "GOm file Output", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (var gomItm in itmList)
                {
                    string path = Config.ExtractPath;
                    string file = "\\GOM\\" + gomItm.ToString().Replace("/", ".") + ".xml";
                    if (!System.IO.Directory.Exists(path + "GOM\\")) { System.IO.Directory.CreateDirectory(path + "GOM\\"); }
                    WriteFile(new XDocument(gomItm.Print()), file, false, true);
                    i++;
                }
            }
            itmList = null;
            addtolist("the xml files have been generated there were " + ttl + " objects that matched your criteria");
            MessageBox.Show("the xml files have been generated there were " + ttl + " objects that matched your criteria");
            EnableButtons();
        }

        private void ProcessList(ref IEnumerable<GomLib.GomObject> itmList, ref XElement root)
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
            addtolist(root.Element("Items").Elements("Item").Count() + " matching Items found.");
            root.Element("Items").ReplaceWith(Sort(root.Element("Items")));
            addtolist(root.Element("Abilities").Elements("Ability").Count() + " matching Abilities found.");
            addtolist(root.Element("CodexEntries").Elements("Codex").Count() + " matching CodexEntries found.");
            addtolist(root.Element("Npcs").Elements("Npc").Count() + " matching Npcs found.");
            addtolist(root.Element("Quests").Elements("Quest").Count() + " matching Quests found.");
            addtolist(root.Element("Talents").Elements("Talent").Count() + " matching Talents found.");
            addtolist(root.Element("Achievements").Elements("Achievement").Count() + " matching Achievements found.");
            addtolist(root.Element("Conversations").Elements("Conversation").Count() + " matching Conversations found.");
            
            //This entire function is getting rewritten now that the individual extractors support added/changed elements via the new radio button.
            /*var itemsList = itmList.Where(fqn => fqn.Name.StartsWith("itm.") && !fqn.Name.StartsWith("itm.test.") && !fqn.Name.StartsWith("itm.npc."));
            addtolist("Geting all the Items");
            root.Add(ItemDataFromFqnListAsXElement(itemsList, chkBuildCompare.Checked));
            itemsList = null;
            Clearlist();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            var abilityList = itmList.Where(fqn => fqn.Name.StartsWith("abl.") && !fqn.Name.Contains("/"));
            addtolist("Geting all the Abilities");
            root.Add(AbilityDataFromFqnListAsXElement(abilityList, chkBuildCompare.Checked));
            abilityList = null;
            Clearlist();
            GC.Collect();
            GC.WaitForPendingFinalizers();


            var codexList = itmList.Where(fqn => fqn.Name.StartsWith("cdx."));
            addtolist("Geting all the Codices");
            root.Add(CodexDataFromFqnListAsXElement(codexList, chkBuildCompare.Checked));
            codexList = null;
            Clearlist();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            var npcList = itmList.Where(fqn => fqn.Name.StartsWith("npc."));
            addtolist("Geting all the Npcs");
            root.Add(NpcDataFromFqnListAsXElement(npcList, chkBuildCompare.Checked));
            npcList = null;
            Clearlist();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            var questList = itmList.Where(fqn => fqn.Name.StartsWith("qst."));
            addtolist("Geting all the Quests");
            root.Add(QuestDataFromFqnListAsXElement(questList, chkBuildCompare.Checked));
            questList = null;
            Clearlist();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            var talentList = itmList.Where(fqn => fqn.Name.StartsWith("tal."));
            addtolist("Geting all the Talents");
            root.Add(TalentDataFromFqnListAsXElement(talentList, chkBuildCompare.Checked));
            talentList = null;
            Clearlist();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            var achievementList = itmList.Where(fqn => fqn.Name.StartsWith("ach."));
            addtolist("Geting all the Achievements");
            root.Add(AchievementDataFromFqnListAsXElement(achievementList, chkBuildCompare.Checked));
            achievementList = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            var conversationList = itmList.Where(fqn => fqn.Name.StartsWith("cnv."));
            addtolist("Geting all the Conversations");
            root.Add(ConversationDataFromFqnListAsXElement(conversationList, chkBuildCompare.Checked));
            achievementList = null;*/
            Clearlist2();
        }

        #endregion

        /* code moved to Tools_Process.cs */

        #region Misc Functions

        private GomLib.Models.GameObject LoadGameObject(DataObjectModel dom, GomObject gObject)
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
                    if (gObject.Name.StartsWith("class.pc.advanced."))
                        obj = dom.advancedClassLoader.Load(gObject);
                    else
                        // do something else here.
                        throw new NotImplementedException();
                    break;
                case "nco.":
                    obj = dom.newCompanionLoader.Load(gObject);
                    break;
                default:
                    throw new NotImplementedException();
            }

            gObject.Unload();
            return obj;
        }
        private GomLib.Models.PseudoGameObject LoadProtoObject(string xmlRoot, DataObjectModel dom, object id, object gomObjectData)
        {
            switch (xmlRoot)
            {
                case "MtxStoreFronts":
                    GomLib.Models.MtxStorefrontEntry mtx = new GomLib.Models.MtxStorefrontEntry();
                    dom.mtxStorefrontEntryLoader.Load(mtx, (long)id, (GomObjectData)gomObjectData);
                    return mtx;
                case "Collections":
                    GomLib.Models.Collection col = new GomLib.Models.Collection();
                    dom.collectionLoader.Load(col, (long)id, (GomObjectData)gomObjectData);
                    return col;
                case "Companions":
                    GomLib.Models.Companion cmp = new GomLib.Models.Companion();
                    dom.companionLoader.Load(cmp, (ulong)id, (GomObjectData)gomObjectData);
                    return cmp;
                case "Ships":
                    GomLib.Models.scFFShip ship = new GomLib.Models.scFFShip();
                    dom.scFFShipLoader.Load(ship, (long)id, (GomObjectData)gomObjectData);
                    return ship;
                case "Conquests":
                    GomLib.Models.Conquest cnq = new GomLib.Models.Conquest();
                    dom.conquestLoader.Load(cnq, (long)id, (GomObjectData)gomObjectData);
                    return cnq;
                case "SetBonuses":
                    GomLib.Models.SetBonusEntry sbEntry = new GomLib.Models.SetBonusEntry();
                    dom.setBonusLoader.Load(sbEntry, (long)id, (GomObjectData)gomObjectData);
                    return sbEntry;
                /*case "AdvancedClasses":
                    GomLib.Models.PlayerClass dis = new GomLib.Models.PlayerClass();
                    dom.disciplineLoader.LoadClass(dis, (ulong)id, ((List<Object>)gomObjectData).ToList().ConvertAll(x => (GomObjectData)x));
                    return dis;*/
                default:
                    throw new IndexOutOfRangeException();
            }
        }
        
        public void getAll()
        {
            Clearlist2();

            ExtractCheckForm testFile = new ExtractCheckForm();
            DialogResult result = testFile.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                //try
                //{ 
                    LoadData();

                    DisableButtons();
                    List<string> extensions = new List<string>();
                    extensions = testFile.getTypes();

                    ExportICONS = extensions.Contains("ICONS");
                    if (extensions.Contains("CDXCAT"))
                    {
                        DisableButtons();
                        getPrototypeObjects("CodexCategoryTotals", "cdxCategoryTotalsPrototype", "cdxFactionToClassToPlanetToTotalLookupList");
                    }
                    if(extensions.Contains("SBN"))
                    {
                        DisableButtons();
                        getPrototypeObjects("SetBonuses", "itmSetBonusesPrototype", "itmSetBonuses");
                    }
                    if (extensions.Contains("TORC"))
                    {
                        DisableButtons();
                        getTorc();
                    }
                    if (extensions.Contains("MISC"))
                    {
                        DisableButtons();
                        FindNewMtxImages();
                    }
                    if (extensions.Contains("STB"))
                    {
                        DisableButtons();
                        addtolist("Getting String Tables.");
                        getStrings();
                    }
                    if (extensions.Contains("GOM"))
                    {
                        DisableButtons();
                        addtolist("Getting Raw GOM.");
                        ExportGOM = extensions.Contains("EXP");
                        getRaw();
                    }
                    if (extensions.Contains("AC"))
                    {
                        DisableButtons();
                        getObjects("class.pc.advanced", "AdvancedClasses");
                    }
                    if (extensions.Contains("CNQ"))
                    {
                        DisableButtons();
                        getPrototypeObjects("Conquests", "wevConquestInfosPrototype", "wevConquestTable");
                    }
                    if (extensions.Contains("ABL"))
                    {
                        DisableButtons();
                        getObjects("abl.", "Abilities");
                    }
                    if (extensions.Contains("ACH"))
                    {
                        DisableButtons();
                        getObjects("ach.", "Achievements");
                    }
                    if (extensions.Contains("APT"))
                    {
                        DisableButtons();
                        getObjects("apt.", "Strongholds");
                    }
                    if (extensions.Contains("AREA"))
                    {
                        DisableButtons();
                        addtolist("Getting Areas.");
                        getPrototypeObjects("Areas", "mapAreasDataProto", "mapAreasDataObjectList"); // getAreas();
                    }
                    if (extensions.Contains("CDX"))
                    {
                        DisableButtons();
                        getObjects("cdx.", "CodexEntries");
                    }
                    if (extensions.Contains("CNV"))
                    {
                        DisableButtons();
                        getObjects("cnv.", "Conversations");
                    }
                    if (extensions.Contains("DEC"))
                    {
                        DisableButtons();
                        getObjects("dec.", "Decorations");
                    }
                    if (extensions.Contains("ABLEFF"))
                    {
                        DisableButtons();
                        getObjects("eff.", "Effects");
                    }
                    if (extensions.Contains("ITM"))
                    {
                        DisableButtons();
                        getObjects("itm.", "Items");
                    }
                    if (extensions.Contains("NPC"))
                    {
                        DisableButtons();
                        ExportNPP = extensions.Contains("NPP");
                        getObjects("npc.", "Npcs");
                    }
                    if (extensions.Contains("QST"))
                    {
                        DisableButtons();
                        getObjects("qst.", "Quests");
                    }
                    if (extensions.Contains("SCHEM"))
                    {
                        DisableButtons();
                        getObjects("schem.", "Schematics");
                    }
                    if (extensions.Contains("TAL"))
                    {
                        DisableButtons();
                        getObjects("tal.", "Talents");
                    }
                    if (extensions.Contains("COL"))
                    {
                        DisableButtons();
                        getPrototypeObjects("Collections", "colCollectionItemsPrototype", "colCollectionItemsData");

                        TorLib.HashDictionaryInstance.Instance.Unload();
                        TorLib.HashDictionaryInstance.Instance.Load();
                        TorLib.HashDictionaryInstance.Instance.dictionary.CreateHelpers();
                    }
                    if (extensions.Contains("CMP"))
                    {
                        DisableButtons();
                        getObjects("nco.", "NewCompanions");
                        getPrototypeObjects("Companions", "chrCompanionInfo_Prototype", "chrCompanionInfoData");
                    }
                    if (extensions.Contains("MTX"))
                    {
                        DisableButtons();
                        getPrototypeObjects("MtxStoreFronts", "mtxStorefrontInfoPrototype", "mtxStorefrontData");
                        
                        //Reload hash dict.
                        TorLib.HashDictionaryInstance.Instance.Unload();
                        TorLib.HashDictionaryInstance.Instance.Load();
                        TorLib.HashDictionaryInstance.Instance.dictionary.CreateHelpers();
                    }
                    if (extensions.Contains("GSF"))
                    {
                        DisableButtons();
                        getPrototypeObjects("Ships", "scFFShipsDataPrototype", "scFFShipsData");
                    }
                    if (extensions.Contains("IPP"))
                    {
                        DisableButtons();
                        addtolist("Getting Item Appearances.");
                        getObjects("ipp.", "ItemAppearances");
                        //getItemApps();
                    }
                    if (extensions.Contains("SCHVARI"))
                    {
                        DisableButtons();
                        addtolist("Getting Schematic Variations");
                        getPrototypeObjects("SchematicVariations", "prfSchematicVariationsPrototype", "prfSchematicVariationMasterList");
                    }
                    addtolist("Completed extraction of all supported objects.");
                //}
                //catch (Exception e)
                //{
                //    //do something here
                //    MessageBox.Show(String.Format("An error occured while loading data. ({0})", e.HResult));
                //}
            }
            GC.Collect();
            EnableButtons();
        }
        public void getRaw()
        {
            Clearlist2();
            double i = 0;
            string n = Environment.NewLine;

            LoadData();
            currentDom.OutputTypeNames(Config.ExtractPath + prefix);

            var itmList = currentDom.GetObjectsStartingWith("");//.Where(obj => obj.Name.Contains("."));
            double ttl = itmList.Count();
            bool append = false;
            string changed = "";
            if(chkBuildCompare.Checked)
            {
                changed = "Changed";
            }
            var filename = "GOM_Items.xml";            
            if(outputTypeName == "Text")
            {
                filename = "GOM_Items.txt";
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
                if(chkBuildCompare.Checked && ExportGOM)
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

        private void testGom()  //This function verifies that the GOM is being read properly.
        {
            Clearlist2();

            LoadData();
            addtolist("Testing GOM loading");
            var itm = currentDom.GetObject("tst_CowFields");

            EnableButtons();
        }

        private void testHashes()
        {
            Clearlist2();

            LoadData();
            addtolist("Verifying GameObject Hashes");
            Dictionary<string, bool> gameObjects = new Dictionary<string, bool>
            {
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
                {"npp.",true}
            };
            for(int f = 0; f < gameObjects.Count; f++)
            {
                var gameObj = gameObjects.ElementAt(f);
                ClearProgress();
                var gomList = currentDom.GetObjectsStartingWith(gameObj.Key).Where(x => !x.Name.Contains("/"));
                var count = gomList.Count();
                int i = 0;
                addtolist2(String.Format("Checking {0}", gameObj.Key));
                foreach (var gom in gomList)
                {
                    progressUpdate(i, count);
                    var itm = new GomLib.Models.GameObject().Load(gom);
                    var itm2 = new GomLib.Models.GameObject().Load(gom);
                    if (itm == null) continue;
                    if (itm.GetHashCode() != itm2.GetHashCode())
                    {
                        gameObjects[gameObj.Key] = false;
                        addtolist2(String.Format("Failed: {0}", gameObj.Key));
                        break; //break inner loop
                    }
                    i++;
                }
                if (gameObjects[gameObj.Key])
                    addtolist2(String.Format("Passed: {0}", gameObj.Key));
            }
            string completeString = "Failed.";
            if(gameObjects.Values.ToList().TrueForAll(x => x))
                completeString = "Passed.";
            addtolist(completeString);
            
            addtolist("Verifying GameObject Hashes");
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
                addtolist2(String.Format("Checking {0}", gameObj.Key));
                bool localfail = false;
                foreach (var gom in currentDataProto)
                {
                    progressUpdate(i, count);
                    var itm = GomLib.Models.PseudoGameObject.LoadFromProtoName(gameObj.Key, currentDom, gom.Key, (GomObjectData)gom.Value);
                    var itm2 = GomLib.Models.PseudoGameObject.LoadFromProtoName(gameObj.Key, currentDom, gom.Key, (GomObjectData)gom.Value);
                    if (itm == null) continue;
                    if (itm.GetHashCode() != itm2.GetHashCode())
                    {
                        addtolist2(String.Format("Failed: {0}", gameObj.Key));
                        failed = true;
                        localfail = true;
                        break; //break inner loop
                    }
                    i++;
                }
                if (!localfail)
                    addtolist2(String.Format("Passed: {0}", gameObj.Key));
            }
            completeString = "Passed.";
            if (failed)
                completeString = "Failed.";                
            addtolist(completeString);

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
                addtolist("Unloading Data Object Model.");
                //currentDom.Unload();
                Loaded = false;
                /*currentAssets.Unload();
                if (previousAssets != null)
                {
                    previousDom.Unload();
                    previousAssets.Unload();
                }*/
                currentAssets = null;
                currentDom = null;
                previousAssets = null;
                previousDom = null;
                Clearlist2();
                Clearlist();
                addtolist("Unloading Data Object Model. - Done");
            }
        }

        public TorLib.Assets currentAssets;
        public DataObjectModel currentDom;

        public TorLib.Assets previousAssets;
        public DataObjectModel previousDom;

        public void LoadData()
        {
            if (!Loaded)
            {
               Clearlist();
                ContinuousProgress();
                addtolist("Loading Current Data Object Model.");
                bool usePTS = usePTSAssets.Checked;
                currentAssets = TorLib.AssetHandler.Instance.getCurrentAssets(textBoxAssetsFolder.Text, usePTS);
                //currentAssets = currentAssets.getCurrentAssets(textBoxAssetsFolder.Text, usePTS);                
                currentDom = DomHandler.Instance.getCurrentDOM(currentAssets);
                Clearlist();
                addtolist("Loading Current Data Object Model. - Done");
                if (chkBuildCompare.Checked && textBoxPrevAssetsFolder.Text != "")
                {
                    addtolist("Loading Previous Data Object Model.");
                    previousAssets = TorLib.AssetHandler.Instance.getPreviousAssets(textBoxPrevAssetsFolder.Text, prevUsePTSAssets.Checked);
                    //previousAssets = previousAssets.getPreviousAssets(textBoxPrevAssetsFolder.Text, prevUsePTSAssets.Checked);                    
                    previousDom = DomHandler.Instance.getPreviousDOM(previousAssets);
                    Clearlist();
                    addtolist("Loading Current Data Object Model. - Done");
                    addtolist("Loading Previous Data Object Model. - Done");
                }

                if (CrossLinkDomCheckBox.Checked)// MessageBox.Show("Do you want to Cross-link the Data Object Model? - It can be a slow process.\n\nIt will scan each object in the Data Object Model for references to other objects, and store these connections in the referenced object.", "Select Yes or No.", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    addtolist("Crosslinking Current Data Object Model.");
                    currentDom.CrossLink();
                    //CrossLink(currentDom);
                    Clearlist();
                    addtolist("Crosslinking Data Object Model. - Done");
                    if (chkBuildCompare.Checked)
                    {
                        addtolist("Crosslinking Previous Data Object Model.");
                        previousDom.CrossLink();
                        //CrossLink(previousDom);
                        ProcessGomFields();
                        Clearlist();
                        addtolist("Crosslinking Data Object Model. - Done");
                        addtolist("Crosslinking Previous Data Object Model. - Done");
                    }
                }
                else if (SmartLinkDomCheckBox.Checked)// MessageBox.Show("Do you want to Cross-link the Data Object Model? - It can be a slow process.\n\nIt will scan each object in the Data Object Model for references to other objects, and store these connections in the referenced object.", "Select Yes or No.", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    addtolist("Crosslinking Current Data Object Model.");
                    SmartLink(currentDom);
                    Clearlist();
                    addtolist("Crosslinking Data Object Model. - Done");
                    if (chkBuildCompare.Checked)
                    {
                        addtolist("Crosslinking Previous Data Object Model.");
                        SmartLink(previousDom);
                        ProcessGomFields();
                        Clearlist();
                        addtolist("Crosslinking Data Object Model. - Done");
                        addtolist("Crosslinking Previous Data Object Model. - Done");
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

        private void unloadCurrent()
        {
            TorLib.AssetHandler.Instance.unloadCurrentAssets();
            DomHandler.Instance.unloadCurrentDOM();
            Loaded = false;
        }

        private void unloadPrevious()
        {
            TorLib.AssetHandler.Instance.unloadPreviousAssets();
            DomHandler.Instance.unloadPreviousDOM();
            Loaded = false;
        }

        private void unloadAll()
        {
            TorLib.AssetHandler.Instance.unloadAllAssets();
            DomHandler.Instance.unloadAllDOM();
            Loaded = false;
        }

        private void button1_Click(object sender, EventArgs e)
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
            if(fileList.Length > 0)
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
            if(!Directory.Exists(path))
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