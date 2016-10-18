using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using GomLib;
using GomLib.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.IO.Compression;
using System.Drawing;
using System.Drawing.Imaging;

namespace ConsoleTools
{
    static class Program
    {
        public static string patch;
        public static string patchDir;
        public static string outputDir;
        public static DataObjectModel dom;
        public static MongoClient _client;
        public static IMongoDatabase _database;
        public static Dictionary<string, string> DBLookup = new Dictionary<string, string>
        {
            { "Abilities", "ability"},
            { "AbilityPackages", "abilitypackage"},
            { "Achievements", "achievement"},
            { "AdvancedClasses", "advclass"},
            { "Areas", "area"},
            { "Classes", "classspec"},
            { "CodexEntries", "codex"},
            { "Collections", "collection"},
            { "Conquests", "conquest"},
            { "Conversations", "conversation"},
            { "Decorations", "decoration"},
            { "Ships", "gsf"},
            { "Items", "item"},
            { "Quests", "mission"},
            { "MtxStoreFronts", "mtx"},
            { "NewCompanions", "newcompanion"},
            { "Npcs", "npc"},
            { "Schematics", "schematic"},
            { "SetBonuses", "setbonus"},
            { "Strongholds", "stronghold"},
            { "Talents", "talent"}
        };
        
        static void Main(string[] args)
        {
            if(args == null)
            {
                Console.WriteLine("Missing Arguments");
                return;
            }
            else
            {
                _client = new MongoClient();
                _database = _client.GetDatabase("torc_db");

                Console.WriteLine(String.Join(", ", args));
                if(args.Count() == 0)
                {
                    args = new string[]{ "5.0P6", "J:\\swtor_db\\", "J:\\swtor_db\\processed\\"};
                }
                patch = args[0];
                patchDir = args[1];
                outputDir = String.Format("{0}{1}\\", args[2], patch);

                TorLib.Assets assets = TorLib.AssetHandler.Instance.getCurrentAssets(String.Format("{0}{1}\\Assets\\", patchDir, patch));
                if (assets != null)
                {
                    dom = new DataObjectModel(assets);
                    dom.version = patch;
                    Console.WriteLine("Loading Assets");
                    dom.Load();
                    Smart.Link(dom, Console.WriteLine);

                    getObjects("class.pc.advanced", "AdvancedClasses");
                    getPrototypeObjects("Conquests", "wevConquestInfosPrototype", "wevConquestTable");
                    getObjects("abl.", "Abilities");
                    if (dom.GetObjectsStartingWith("pkg.abilities").Count > 0)
                        getObjects("pkg.abilities", "AbilityPackages");
                    else
                        getObjects("apn.", "AbilityPackages");
                    getObjects("ach.", "Achievements");
                    getObjects("apt.", "Strongholds");
                    getObjects("spn.", "Spawners");
                    getPrototypeObjects("Areas", "mapAreasDataProto", "mapAreasDataObjectList"); // getAreas();
                    getObjects("cdx.", "CodexEntries");
                    getObjects("class.", "Classes");
                    getObjects("cnv.", "Conversations");
                    getObjects("dec.", "Decorations");
                    getObjects("eff.", "Effects");
                    getObjects("itm.", "Items");
                    getObjects("npc.", "Npcs");
                    getObjects("qst.", "Quests");
                    getObjects("schem.", "Schematics");
                    getPrototypeObjects("SetBonuses", "itmSetBonusesPrototype", "itmSetBonuses");
                    getObjects("tal.", "Talents");
                    getPrototypeObjects("Collections", "colCollectionItemsPrototype", "colCollectionItemsData");

                    TorLib.HashDictionaryInstance.Instance.Unload();
                    TorLib.HashDictionaryInstance.Instance.Load();
                    TorLib.HashDictionaryInstance.Instance.dictionary.CreateHelpers();
                    getObjects("nco.", "NewCompanions");
                    getPrototypeObjects("Companions", "chrCompanionInfo_Prototype", "chrCompanionInfoData");
                    getPrototypeObjects("MtxStoreFronts", "mtxStorefrontInfoPrototype", "mtxStorefrontData");

                    //Reload hash dict.
                    TorLib.HashDictionaryInstance.Instance.Unload();
                    TorLib.HashDictionaryInstance.Instance.Load();
                    TorLib.HashDictionaryInstance.Instance.dictionary.CreateHelpers();
                    getPrototypeObjects("Ships", "scFFShipsDataPrototype", "scFFShipsData");
                    getIcons();
                }
                else
                    Console.WriteLine("Assets not found.");
            }
            Console.WriteLine("Finished.");
            Console.ReadLine();
        }

        #region File Functions
        public static string PrepExtractPath(string filename)
        {
            string subPath = "";
            if (filename.Contains('\\'))
                subPath = filename.Substring(0, filename.LastIndexOf('\\'));
            if (!System.IO.Directory.Exists(outputDir + subPath)) { System.IO.Directory.CreateDirectory(outputDir + subPath); }
            return outputDir + filename;
        }
        public static void WriteFile(string content, string filename, bool append)
        {
            string subPath = "";
            if (filename.Contains('\\'))
                subPath = filename.Substring(0, filename.LastIndexOf('\\'));
            if (!System.IO.Directory.Exists(outputDir + subPath)) { System.IO.Directory.CreateDirectory(outputDir + subPath); }
            using (System.IO.StreamWriter file2 = new System.IO.StreamWriter(outputDir + filename, append))
            {
                file2.Write(content);
            }
        }
        public static void WriteFile(MemoryStream content, string filename)
        {
            string subPath = "";
            filename = filename.Replace('/', '\\');
            if (filename.Contains('\\'))
                subPath = filename.Substring(0, filename.LastIndexOf('\\'));
            if (!System.IO.Directory.Exists(outputDir + subPath)) { System.IO.Directory.CreateDirectory(outputDir + subPath); }
            using (System.IO.FileStream file2 = new System.IO.FileStream(outputDir + filename, FileMode.OpenOrCreate))
            {
                content.Position = 0;
                content.CopyTo(file2);
            }
        }
        public static void CreateGzip(string filename)
        {
            string filepath = String.Join("", outputDir, filename);
            if (!System.IO.File.Exists(filepath))
                return;
            using (FileStream readstream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                if (readstream == null) return;
                if (readstream.Length == 0) return;
                using (FileStream outFileStream = new FileStream(String.Join("", filepath, ".gz"), FileMode.Create, FileAccess.Write))
                using (System.IO.Compression.GZipStream gzip = new System.IO.Compression.GZipStream(outFileStream, System.IO.Compression.CompressionMode.Compress))
                {
                    readstream.CopyTo(gzip);
                }
            }
            File.Delete(filepath);
        }
        public static void DeleteEmptyFile(string filename, int count) //deletes empty files that were created for streaming output
        {
            string filepath = String.Join("", outputDir, filename);
            FileInfo fInfo = new FileInfo(filepath);
            if (fInfo != null)
                if (fInfo.Length == 0 || count == 0)
                    System.IO.File.Delete(filepath);
        }
        #endregion
        #region Processing Functions
        public static void getObjects(string prefix, string elementName)
        {
            Console.Write(String.Format("Getting {0}", elementName));
            switch (elementName)
            {
                case "Abilities": //This section is for exploring Ability Effects
                    FullyProcessGameObjects(prefix, elementName);
                    //dom.abilityLoader.effKeys.Sort();
                    //string effKeyList = String.Join(Environment.NewLine, dom.abilityLoader.effKeys);
                    //WriteFile(effKeyList, "effKeys.txt", false);

                    //dom.abilityLoader.effWithUnknowns = dom.abilityLoader.effWithUnknowns.Distinct().OrderBy(o => o).ToList();
                    //string effUnknowns = String.Join(Environment.NewLine, dom.abilityLoader.effWithUnknowns);
                    //WriteFile(effUnknowns, "effUnknowns.txt", false);

                    //dom.abilityLoader.effWithUnknowns = new List<string>();
                    //dom.abilityLoader.effKeys = new List<string>();
                    break;
                default:
                    FullyProcessGameObjects(prefix, elementName);
                    break;
            }
        }
        public static void getPrototypeObjects(string xmlRoot, string prototype, string dataTable)
        {
            Console.Write(String.Format("Getting {0} ", xmlRoot));
            FullyProcessProcessProtoData(xmlRoot, prototype, dataTable);
        }
        public static void getIcons()
        {
            Console.Write(String.Format("Getting {0} ", "Icons"));
            TorLib.HashDictionaryInstance.Instance.Unload();
            TorLib.HashDictionaryInstance.Instance.Load();
            TorLib.HashDictionaryInstance.Instance.dictionary.CreateHelpers();
            //try
            //{
                var libs = dom._assets.libraries.Where(x => x.Name.Contains("gfx_assets"));
                var archive = libs.First().archives.First();
                List<TorLib.HashFileInfo> iconsToOutput = new List<TorLib.HashFileInfo>();
                foreach(var file in archive.Value.files)
                {
                    TorLib.HashFileInfo hashInfo = new TorLib.HashFileInfo(file.FileInfo.ph, file.FileInfo.sh, file);
                    if(hashInfo.FileState != TorLib.HashFileInfo.State.Unchanged)
                    {
                        using (BinaryReader bin = new BinaryReader(new MemoryStream(file.PeakBytes(20))))
                        {
                            int magic = bin.ReadInt32();
                            if (magic != 0x20534444)
                                continue;
                            bin.ReadInt32(); //header size
                            bin.ReadInt32(); //flags
                            int height = bin.ReadInt32();
                            int width = bin.ReadInt32();
                            bin.Close();
                            if(height == 52 && width == 52)
                            {
                                iconsToOutput.Add(hashInfo);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
            CreatCompressedIconOutput(iconsToOutput);
            return;
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.InnerException);
            //}
        }

        public static void FullyProcessGameObjects(string gomPrefix, string xmlRoot)
        {
            bool classOverride = (xmlRoot == "AdvancedClasses");
            IEnumerable<GomObject> curItmList = getMatchingGomObjects(dom, gomPrefix);

            short e = 0;
            int ie = 0;
            string n = Environment.NewLine;
            var txtFile = new StringBuilder();
            string filename = String.Format("\\json\\Full{0}.json", xmlRoot);
            WriteFile(String.Format("{0}{1}", patch, n), filename, false);
            HashTableHashing.MurmurHash2Unsafe jsonHasher = new HashTableHashing.MurmurHash2Unsafe();

            int i = 0;
            int count = 0;
            bool found_items = false;
            count = curItmList.Count();
            List<GomLib.Models.Tooltip> iList = new List<GomLib.Models.Tooltip>();
            bool frLoaded = dom._assets.loadedFileGroups.Contains("fr-fr");
            bool deLoaded = dom._assets.loadedFileGroups.Contains("de-de");
            using (var progress = new ProgressBar())
            {
                string db_table;
                DBLookup.TryGetValue(xmlRoot, out db_table);
                Dictionary<ulong, uint> current_hashes = new Dictionary<ulong, uint>();
                if (db_table != null)
                {
                    var filter = new BsonDocument("name", db_table);
                    //filter by collection name
                    var collections = _database.ListCollections(new ListCollectionsOptions { Filter = filter });
                    //check for existence
                    if ((collections.ToList()).Any())
                    {
                        IMongoCollection<BsonDocument> collection = _database.GetCollection<BsonDocument>(db_table);

                        var cursor = collection.Find(Builders<BsonDocument>.Filter.Empty);
                        var fields = Builders<BsonDocument>.Projection.Include("Id").Include("hash");
                        var result = cursor.Project(fields).ToList();
                        current_hashes = result.ToDictionary(x => UInt64.Parse(x["Id"].AsString), x => UInt32.Parse(x["hash"].AsString));
                    }
                }
                foreach (var curObject in curItmList)
                {
                    progress.Report((double)i / count);

                    if (e % 1000 == 1)
                    {
                        found_items = true;
                        WriteFile(txtFile.ToString(), filename, true);
                        txtFile.Clear();
                        e = 0;
                    }

                    var obj = GameObject.Load(curObject, classOverride);
                    if (obj != null && obj.Id != 0) //apparently if the loader passes null back, sometimes data comes, too.....
                    {
                        string jsonString = obj.ToJSON();
                        uint hash = jsonHasher.Hash(Encoding.ASCII.GetBytes(jsonString));
                        uint oldhash;
                        if(current_hashes.TryGetValue(obj.Id, out oldhash))
                        {
                            if(hash != oldhash)
                            {
                                // The MongoDB c# Driver is so fucking awful that we're going to output the json for php insertion.
                                // Because fucking trying to deserialized uint64s using a deserializer that doesn't support them, and won't just make them strings
                                txtFile.Append(String.Format("{0},{1},{2}{3}", obj.Base62Id, hash, jsonString, Environment.NewLine)); //Append it with a newline to the output.
                                e++; ie++;
                                Tooltip t = new Tooltip(obj);
                                if (t.IsImplemented())
                                    iList.Add(t);
                            }
                            //else //do nothing
                        }
                        else // new
                        {
                            txtFile.Append(String.Format("{0},{1},{2}{3}", obj.Base62Id, hash, jsonString, Environment.NewLine)); //Append it with a newline to the output.
                            Tooltip t = new Tooltip(obj);
                            e++; ie++;
                            if (t.IsImplemented())
                                iList.Add(t);
                        }
                    }
                    i++;
                }
                WriteFile(txtFile.ToString(), filename, true);
                txtFile.Clear();
                if (e != 0)
                    found_items = true;
            }
            if (!found_items)
                DeleteEmptyFile(filename, 0);
            curItmList = null;
            GC.Collect();
            CreateGzip(filename);
            Dictionary<string, string> gameObjects = new Dictionary<string, string>
            {

                {"ach.", "Achievement"},
                {"abl.", "Ability"},
                {"cdx.", "Codex"},
                {"itm.", "Item"},
                {"nco.", "Companion" },
                {"npc.", "Npc" },
                {"qst.", "Mission"},
                {"tal.", "Talent"},
                {"sche", "Schematic"},
            };
            if (iList.Count > 0)
            {
                using (var progress = new ProgressBar())
                {
                    string gameObj;
                    gameObjects.TryGetValue(iList.First().Fqn.Substring(0,4), out gameObj);
                    if (!String.IsNullOrWhiteSpace(gameObj)) {
                        Console.Write(String.Format(" - Generating Tooltips. ({0})", ie));
                        progress.Report(0);
                        CreatCompressedTooltipOutput(gameObj, iList, "en-us");
                        progress.Report(.3333);
                        if (frLoaded)
                            CreatCompressedTooltipOutput(gameObj, iList, "fr-fr");
                        progress.Report(.6666);
                        if (deLoaded)
                            CreatCompressedTooltipOutput(gameObj, iList, "de-de");
                        Console.Write(" - Done.");
                    }
                    else
                        Console.Write(String.Format(" - Done. ({0})", ie));
                }
            }
            
            Console.WriteLine();
        }
        public static void FullyProcessProcessProtoData(string xmlRoot, string prototype, string dataTable)
        {
            Dictionary<object, object> currentDataProto = new Dictionary<object, object>();
            GomObject currentDataObject = dom.GetObject(prototype);
            if (currentDataObject != null) //fix to ensure old game assets don't throw exceptions.
            {
                currentDataProto = currentDataObject.Data.Get<Dictionary<object, object>>(dataTable);
                currentDataObject.Unload();
            }

            var curIds = currentDataProto.Keys;
            var curItems = new List<GomLib.Models.PseudoGameObject>();

            short e = 0;
            int ie = 0;
            string n = Environment.NewLine;
            var txtFile = new StringBuilder();
            string filename = String.Format("\\json\\Full{0}.json", xmlRoot);
            WriteFile(String.Format("{0}{1}", patch, n), filename, false);
            HashTableHashing.MurmurHash2Unsafe jsonHasher = new HashTableHashing.MurmurHash2Unsafe();

            int i = 0;
            int count = 0;
            bool found_items = false;
            count = curIds.Count();
            List<GomLib.Models.Tooltip> iList = new List<GomLib.Models.Tooltip>();
            bool frLoaded = dom._assets.loadedFileGroups.Contains("fr-fr");
            bool deLoaded = dom._assets.loadedFileGroups.Contains("de-de");
            using (var progress = new ProgressBar())
            {
                string db_table;
                DBLookup.TryGetValue(xmlRoot, out db_table);
                Dictionary<long, uint> current_hashes = new Dictionary<long, uint>();
                if (db_table != null)
                {
                    var filter = new BsonDocument("name", db_table);
                    //filter by collection name
                    var collections = _database.ListCollections(new ListCollectionsOptions { Filter = filter });
                    //check for existence
                    if ((collections.ToList()).Any())
                    {
                        IMongoCollection<BsonDocument> collection = _database.GetCollection<BsonDocument>(db_table);

                        var cursor = collection.Find(Builders<BsonDocument>.Filter.Empty);
                        var fields = Builders<BsonDocument>.Projection.Include("Id").Include("hash");
                        var result = cursor.Project(fields).ToList();
                        current_hashes = result.ToDictionary(
                            x => Int64.Parse(x["Id"].AsString),
                            x => UInt32.Parse(
                                (x.Contains("hash") ? x["hash"].AsString : "0")));
                    }
                }
                foreach (var id in curIds)
                {
                    progress.Report((double)i / count);

                    if (e % 1000 == 1)
                    {
                        found_items = true;
                        WriteFile(txtFile.ToString(), filename, true);
                        txtFile.Clear();
                        e = 0;
                    }

                    object curData;
                    currentDataProto.TryGetValue(id, out curData);
                    GomLib.Models.PseudoGameObject curObj = PseudoGameObject.Load(xmlRoot, dom, id, curData);
                    string jsonString = curObj.ToJSON();
                    uint hash = jsonHasher.Hash(Encoding.ASCII.GetBytes(jsonString));
                    uint oldhash;
                    if (current_hashes.TryGetValue(curObj.Id, out oldhash))
                    {
                        if (hash != oldhash)
                        {
                            // The MongoDB c# Driver is so fucking awful that we're going to output the json for php insertion.
                            // Because fucking trying to deserialized uint64s using a deserializer that doesn't support them, and won't just make them strings
                            txtFile.Append(String.Format("{0},{1},{2}{3}", curObj.Base62Id, hash, jsonString, Environment.NewLine)); //Append it with a newline to the output.
                            e++; ie++;
                            Tooltip t = new Tooltip(curObj);
                            if (t.IsImplemented())
                                iList.Add(t);
                        }
                        //else //do nothing
                    }
                    else // new
                    {
                        txtFile.Append(String.Format("{0},{1},{2}{3}", curObj.Base62Id, hash, jsonString, Environment.NewLine)); //Append it with a newline to the output.
                        Tooltip t = new Tooltip(curObj);
                        e++; ie++;
                        if (t.IsImplemented())
                            iList.Add(t);
                    }
                    i++;
                }
                WriteFile(txtFile.ToString(), filename, true);
                txtFile.Clear();
                if (e != 0)
                    found_items = true;
            }
            if (!found_items)
                DeleteEmptyFile(filename, 0);
            currentDataObject.Unload();
            curIds = null;
            GC.Collect();
            CreateGzip(filename);
            Dictionary<string, string> gameObjects = new Dictionary<string, string>
            {

                {"ach.", "Achievement"},
                {"abl.", "Ability"},
                {"cdx.", "Codex"},
                {"itm.", "Item"},
                {"nco.", "Companion" },
                {"npc.", "Npc" },
                {"qst.", "Mission"},
                {"tal.", "Talent"},
                {"sche", "Schematic"},
            };
            if (iList.Count > 0)
            {
                using (var progress = new ProgressBar())
                {
                    string gameObj;
                    gameObjects.TryGetValue(iList.First().Fqn.Substring(0, 4), out gameObj);
                    if (!String.IsNullOrWhiteSpace(gameObj))
                    {
                        Console.Write(String.Format(" - Generating Tooltips. ({0})", ie));
                        progress.Report(0);
                        CreatCompressedTooltipOutput(gameObj, iList, "en-us");
                        progress.Report(.3333);
                        if (frLoaded)
                            CreatCompressedTooltipOutput(gameObj, iList, "fr-fr");
                        progress.Report(.6666);
                        if (deLoaded)
                            CreatCompressedTooltipOutput(gameObj, iList, "de-de");
                        Console.Write(" - Done.");
                    }
                    else
                        Console.Write(String.Format(" - Done. ({0})", ie));
                }
            }

            Console.WriteLine();
        }

        //public static void ProcessGameObjects(string gomPrefix, string xmlRoot)
        //{
        //    bool classOverride = (xmlRoot == "AdvancedClasses");
        //    IEnumerable<GomObject> curItmList = getMatchingGomObjects(dom, gomPrefix);

        //    var curItmNames = curItmList.Select(x => x.Name).ToList();

        //    Dictionary<string, List<GomLib.Models.GameObject>> ObjectLists = new Dictionary<string, List<GomLib.Models.GameObject>>();
        //    var newItems = new List<GomLib.Models.GameObject>();

        //    int i = 0;
        //    int count = 0;
            
        //    i = 0;
        //    count = curItmList.Count();
        //    using (var progress = new ProgressBar())
        //    {

        //        foreach (var curObject in curItmList)
        //        {
        //            progress.Report((double)i / count);
        //            var obj = GameObject.Load(curObject, classOverride);
        //            if (obj != null && obj.Id != 0) //apparently if the loader passes null back, sometimes data comes, too.....
        //                newItems.Add(obj);
        //            i++;
        //        }
        //    }
        //    ObjectLists.Add("Full", newItems);

        //    Console.Write(" - Generating Output ");
        //    count = 0;
        //    i = 0;
        //    foreach (var itmList in ObjectLists)
        //    {
        //        count += itmList.Value.Count;
        //    }

        //    foreach (var itmList in ObjectLists)
        //    {
        //        GameObjectListAsJSON(String.Join("", itmList.Key, xmlRoot), itmList.Value);
        //        //GetTips(xmlRoot, itmList.Value);
        //    }
        //    Console.WriteLine();
        //}

        //public static void ProcessProtoData(string xmlRoot, string prototype, string dataTable)
        //{
        //    int i = 0;
        //    int count = 0;

        //    Dictionary<object, object> currentDataProto = new Dictionary<object, object>();
        //    GomObject currentDataObject = dom.GetObject(prototype);
        //    if (currentDataObject != null) //fix to ensure old game assets don't throw exceptions.
        //    {
        //        currentDataProto = currentDataObject.Data.Get<Dictionary<object, object>>(dataTable);
        //        currentDataObject.Unload();
        //    }

        //    var curIds = currentDataProto.Keys;
        //    Dictionary<string, List<GomLib.Models.PseudoGameObject>> ObjectLists = new Dictionary<string, List<GomLib.Models.PseudoGameObject>>();
        //    var newItems = new List<GomLib.Models.PseudoGameObject>();

        //    i = 0;
        //    count = curIds.Count();
        //    using (var progress = new ProgressBar())
        //    {
        //        foreach (var id in curIds)
        //        {
        //            progress.Report((double)i / count);
        //            object curData;
        //            currentDataProto.TryGetValue(id, out curData);
        //            GomLib.Models.PseudoGameObject curObj = PseudoGameObject.Load(xmlRoot, dom, id, curData);
        //            newItems.Add(curObj);
        //            i++;
        //        }
        //    }
        //    ObjectLists.Add("Full", newItems);

        //    Console.Write(" - Generating Output ");
        //    count = 0;
        //    i = 0;
        //    foreach (var itmList in ObjectLists)
        //    {
        //        count += itmList.Value.Count;
        //    }


        //    foreach (var itmList in ObjectLists)
        //    {
        //        PseudoGameObjectListAsJSON(String.Join("", itmList.Key, xmlRoot), itmList.Value);
        //    }
        //    Console.WriteLine();
        //}

        private static IEnumerable<GomObject> getMatchingGomObjects(DataObjectModel dom, string gomPrefix)
        {           //This function is meant to handle unique cases of loading a list of objects from the GOM
            IEnumerable<GomObject> itmList;
            switch (gomPrefix)
            {
                case "abl.":
                    itmList = dom.GetObjectsStartingWith(gomPrefix).Where(x => !x.Name.Contains("/")); //Abilities with a / in the name are Effects.
                    break;
                case "apn.":
                    itmList = dom.GetObjectsStartingWith(gomPrefix).Union(dom.GetObjectsStartingWith("apc.")); //Union APC/APN
                    break;
                case "eff.":
                    itmList = dom.GetObjectsStartingWith("abl.").Where(x => x.Name.Contains("/"));
                    break;
                default:
                    itmList = dom.GetObjectsStartingWith(gomPrefix); //No need for the extra Linq statement for non-unique cases
                    break;
            }
            return itmList;
        }
        //private static void GameObjectListAsJSON(string prefix, List<GomLib.Models.GameObject> itmList)
        //{
        //    int i = 0;
        //    short e = 0;
        //    string n = Environment.NewLine;
        //    var txtFile = new StringBuilder();
        //    string filename = String.Format("\\json\\{0}{1}", prefix, ".json");
        //    WriteFile(String.Format("{0}{1}", patch, n), filename, false);
        //    var count = itmList.Count();
        //    HashTableHashing.MurmurHash2Unsafe jsonHasher = new HashTableHashing.MurmurHash2Unsafe();
        //    //var blocksize = 100;
        //    //var blocks = count % blocksize;
        //    //if (blocks > 0)
        //    //{
        //    //    progressUpdate(i, count);
        //    //    for (int b = blocks - 1; b >= 0; b--) //go backwards so we can delete items :)
        //    //    {
        //    //        System.Collections.Concurrent.BlockingCollection<string> blockCollection = new System.Collections.Concurrent.BlockingCollection<string>();
        //    //        var block = itmList.GetRange(b * blocksize, blocksize);
        //    //        Parallel.ForEach(block, itm =>
        //    //        {
        //    //            string jsonBlock = itm.ToJSON();
        //    //            blockCollection.Add(jsonBlock);
        //    //        });
        //    //        i += blocksize;
        //    //        txtFile.Append(String.Join(Environment.NewLine, blockCollection));
        //    //        itmList.RemoveRange(b * blocksize, blocksize);
        //    //    }
        //    //}
        //    using (var progress = new ProgressBar())
        //    {
        //        for (int b = itmList.Count - 1; b >= 0; b--) //go backwards so we can delete values
        //        {
        //            progress.Report((double)i / count);
        //            if (e % 1000 == 1)
        //            {
        //                WriteFile(txtFile.ToString(), filename, true);
        //                txtFile.Clear();
        //                e = 0;
        //            }

        //            string jsonString = itmList[b].ToJSON(); // ConvertToJson(itm); //added method in Tools.cs
        //            uint hash = jsonHasher.Hash(Encoding.ASCII.GetBytes(jsonString));
        //            txtFile.Append(String.Format("{0},{1},{2}{3}", itmList[b].Base62Id, hash, jsonString, Environment.NewLine)); //Append it with a newline to the output.
        //            itmList[b] = null;
        //            i++;
        //            e++;
        //        }
        //    }
        //    //foreach (var itm in itmList) //clean up what's left
        //    //{
        //    //    progressUpdate(i, count);
        //    //    if (e % 1000 == 1)
        //    //    {
        //    //        WriteFile(txtFile.ToString(), filename, true);
        //    //        txtFile.Clear();
        //    //        e = 0;
        //    //    }

        //    //    addtolist2(String.Format("{0}: {1}", prefix, itm.Fqn));

        //    //    string jsonString = itm.ToJSON(); // ConvertToJson(itm); //added method in Tools.cs
        //    //    txtFile.Append(jsonString + Environment.NewLine); //Append it with a newline to the output.
        //    //    i++;
        //    //    e++;
        //    //}
        //    Console.Write(String.Format(" - Done. ({0})", i));
        //    WriteFile(txtFile.ToString(), filename, true);
        //    DeleteEmptyFile(filename, i);
        //    itmList = null;
        //    GC.Collect();
        //    CreateGzip(filename);
        //}
        //private static void PseudoGameObjectListAsJSON(string prefix, List<GomLib.Models.PseudoGameObject> itmList)
        //{
        //    int i = 0;
        //    short e = 0;
        //    string n = Environment.NewLine;
        //    var txtFile = new StringBuilder();
        //    string filename = String.Format("\\json\\{0}{1}", prefix, ".json");
        //    WriteFile(String.Format("{0}{1}", patch, n), filename, false);
        //    var count = itmList.Count();
        //    HashTableHashing.MurmurHash2Unsafe jsonHasher = new HashTableHashing.MurmurHash2Unsafe();
        //    using (var progress = new ProgressBar())
        //    {
        //        for (int c = 0; c < count; c++)
        //        {
        //            progress.Report((double)i / count);
        //            if (e % 1000 == 1)
        //            {
        //                WriteFile(txtFile.ToString(), filename, true);
        //                txtFile.Clear();
        //                e = 0;
        //            }

        //            string jsonString = itmList[c].ToJSON();
        //            uint hash = jsonHasher.Hash(Encoding.ASCII.GetBytes(jsonString));
        //            txtFile.Append(String.Format("{0},{1},{2}{3}", itmList[c].Base62Id, hash, jsonString, Environment.NewLine)); //Append it with a newline to the output.
        //            itmList[c] = null;
        //            i++;
        //            e++;
        //        }
        //    }
        //    Console.Write(String.Format(" - Done. ({0})", i));
        //    WriteFile(txtFile.ToString(), filename, true);
        //    DeleteEmptyFile(filename, i);
        //    itmList = null;
        //    GC.Collect();
        //    CreateGzip(filename);
        //}
        //private static void GomObjectListAsJSON(string prefix, IEnumerable<GomLib.GomObject> itmList)
        //{
        //    int i = 0;
        //    short e = 0;
        //    string n = Environment.NewLine;
        //    var txtFile = new StringBuilder();
        //    string filename = String.Join("", prefix, ".json");
        //    WriteFile("", filename, false);
        //    var count = itmList.Count();
        //    using (var progress = new ProgressBar())
        //    {
        //        foreach (var itm in itmList)
        //        {
        //            progress.Report((double)i / count);
        //            if (e % 1000 == 1)
        //            {
        //                WriteFile(txtFile.ToString(), filename, true);
        //                txtFile.Clear();
        //                e = 0;
        //            }

        //            //addtolist2(String.Format("{0}: {1}", prefix, itm.Name));

        //            string jsonString = GameObject.Load(itm, false).ToJSON(); // ConvertToJson(itm); //added method in Tools.cs
        //            txtFile.Append(jsonString + Environment.NewLine); //Append it with a newline to the output.
        //            i++;
        //            e++;
        //        }
        //    }
        //    Console.Write(String.Format(" - Done. ({0})", i));
        //    WriteFile(txtFile.ToString(), filename, true);
        //    DeleteEmptyFile(filename, i);
        //    itmList = null;
        //    GC.Collect();
        //    CreateGzip(filename);
        //}

        public static void CreatCompressedTooltipOutput(string xmlRoot, IEnumerable<GomLib.Models.Tooltip> itmList, string language)
        {
            string file = String.Format("tooltips\\{0}tips({1}).zip", xmlRoot, language);
            WriteFile("", file, false);
            HashSet<string> iconNames = new HashSet<string>();
            using (var compressStream = new MemoryStream())
            {
                //create the zip in memory
                using (var zipArchive = new ZipArchive(compressStream, ZipArchiveMode.Create, true))
                {
                    string compressedFolder = "tooltips/html/";
                    switch (language)
                    {
                        case "fr-fr":
                            compressedFolder = "tooltips/html/fr-fr/";
                            GomLib.Models.Tooltip.language = "frMale";
                            break;
                        case "de-de":
                            compressedFolder = "tooltips/html/de-de/";
                            GomLib.Models.Tooltip.language = "deMale";
                            break;
                    }
                    foreach (var t in itmList)
                    {
                        var torcEntry = zipArchive.CreateEntry(String.Format("{0}{1}.torctip", compressedFolder, t.Base62Id), CompressionLevel.Fastest);
                        using (StreamWriter writer = new StreamWriter(torcEntry.Open(), Encoding.UTF8)) //old method. Race conditions led to Central Directory corruption.
                            writer.Write(t.HTML);
                        /*using (MemoryStream htmlStream = new MemoryStream(Encoding.UTF8.GetBytes(t.HTML ?? ""))) //see if this solves the Central Directory corruption.
                        {
                            using (var html = torcEntry.Open())
                                htmlStream.WriteTo(html);
                        }*/
                        if (language != "en-us") continue;
                        string icon = "";
                        string secondaryicon = "";
                        if (t.obj != null)
                        {
                            switch (t.obj.GetType().ToString())
                            {
                                case "GomLib.Models.Item":
                                    icon = String.Format("icons/{0}", ((GomLib.Models.Item)t.obj).Icon);
                                    secondaryicon = String.Format("icons/{0}", ((GomLib.Models.Item)t.obj).RepublicIcon);
                                    break;
                                case "GomLib.Models.Ability":
                                    icon = String.Format("icons/{0}", ((GomLib.Models.Ability)t.obj).Icon);
                                    break;
                                case "GomLib.Models.Quest":
                                    icon = String.Format("codex/{0}", ((GomLib.Models.Quest)t.obj).Icon);
                                    break;
                                case "GomLib.Models.Talent":
                                    icon = String.Format("icons/{0}", ((GomLib.Models.Talent)t.obj).Icon);
                                    break;
                                case "GomLib.Models.Achievement":
                                    icon = String.Format("icons/{0}", ((GomLib.Models.Achievement)t.obj).Icon);
                                    break;
                                case "GomLib.Models.Codex":
                                    //break;
                                    icon = String.Format("codex/{0}", ((GomLib.Models.Codex)t.obj).Image);
                                    break;
                                case "GomLib.Models.NewCompanion":
                                    //break;
                                    icon = String.Format("portraits/{0}", ((GomLib.Models.NewCompanion)t.obj).Icon);
                                    break;
                            }
                        }
                        if (t.pObj != null)
                        {
                            switch (t.pObj.GetType().ToString())
                            {
                                case "GomLib.Models.Discipline":
                                    icon = String.Format("icons/{0}", ((GomLib.Models.Item)t.obj).Icon);
                                    break;
                                case "GomLib.Models.Collection":
                                    break;
                                    icon = String.Format("mtxstore/{0}_260x260", ((GomLib.Models.Collection)t.pObj).Icon);
                                    break;
                                case "GomLib.Models.MtxStorefrontEntry":
                                    icon = String.Format("mtxstore/{0}_260x260", ((GomLib.Models.MtxStorefrontEntry)t.pObj).Icon);
                                    break;
                            }
                        }
                        if (!String.IsNullOrEmpty(icon))
                        {
                            icon = icon.ToLower();
                            if (iconNames.Contains(icon))
                                continue;
                            else
                                iconNames.Add(icon);
                            IconParamter[] parms = new IconParamter[1];
                            if (icon.StartsWith("portraits/"))
                                parms[0] = IconParamter.PngFormat;
                            using (MemoryStream iconStream = GetIcon(icon, parms))
                            {
                                if (iconStream != null)
                                {
                                    ZipArchiveEntry iconEntry;
                                    if (icon.StartsWith("codex/"))
                                    {
                                        iconEntry = zipArchive.CreateEntry(String.Format("codex/{0}.jpg", GetIconFilename(icon)), CompressionLevel.Fastest);
                                    }
                                    else if (icon.StartsWith("portraits/"))
                                        iconEntry = zipArchive.CreateEntry(String.Format("portraits/{0}.png", GetIconFilename(icon)), CompressionLevel.Fastest);
                                    else if (icon.StartsWith("mtxstore/"))
                                        iconEntry = zipArchive.CreateEntry(String.Format("{0}.jpg", icon.ToLower()), CompressionLevel.Fastest);
                                    else
                                        iconEntry = zipArchive.CreateEntry(String.Format("icons/{0}.jpg", GetIconFilename(icon)), CompressionLevel.Fastest);
                                    using (var a = iconEntry.Open())
                                        iconStream.WriteTo(a);
                                    //using (Writer writer = new BinaryWriter(iconEntry.Open()))
                                    //writer.(iconStream);
                                }
                            }
                            if (icon.StartsWith("codex/"))
                            {
                                using (MemoryStream iconStream = GetIcon(icon, true))
                                {
                                    if (iconStream != null)
                                    {
                                        ZipArchiveEntry iconEntry;
                                        iconEntry = zipArchive.CreateEntry(String.Format("codex/{0}_thumb.jpg", GetIconFilename(icon)), CompressionLevel.Fastest);
                                        using (var a = iconEntry.Open())
                                            iconStream.WriteTo(a);
                                    }
                                }
                            }
                            else if (icon.StartsWith("portraits/"))
                            {
                                using (MemoryStream iconStream = GetIcon(icon, true, IconParamter.PngFormat))
                                {
                                    if (iconStream != null)
                                    {
                                        ZipArchiveEntry iconEntry;
                                        iconEntry = zipArchive.CreateEntry(String.Format("portraits/{0}_thumb.png", GetIconFilename(icon)), CompressionLevel.Fastest);
                                        using (var a = iconEntry.Open())
                                            iconStream.WriteTo(a);
                                    }
                                }
                            }
                            else if (icon.StartsWith("mtxstore/"))
                            {
                                List<string> sizes = new List<string>() { "120x120", "260x400", "400x400" };
                                foreach (string size in sizes)
                                {
                                    string sizedIcon = icon.Replace("260x260", size);
                                    using (MemoryStream iconStream = GetIcon(sizedIcon))
                                    {
                                        if (iconStream != null)
                                        {
                                            ZipArchiveEntry iconEntry;
                                            iconEntry = zipArchive.CreateEntry(String.Format("{0}.jpg", sizedIcon.ToLower()), CompressionLevel.Fastest);
                                            using (var a = iconEntry.Open())
                                                iconStream.WriteTo(a);
                                        }
                                    }
                                }
                            }

                        }
                        if (!String.IsNullOrEmpty(secondaryicon))
                        {
                            secondaryicon = secondaryicon.ToLower();
                            if (iconNames.Contains(secondaryicon))
                                continue;
                            else
                                iconNames.Add(secondaryicon);
                            using (MemoryStream iconStream = GetIcon(secondaryicon))
                            {
                                if (iconStream != null)
                                {
                                    ZipArchiveEntry iconEntry;
                                    if (icon.StartsWith("codex/"))
                                    {
                                        iconEntry = zipArchive.CreateEntry(String.Format("codex/{0}.jpg", GetIconFilename(secondaryicon)), CompressionLevel.Fastest);
                                    }
                                    else if (icon.StartsWith("mtxstore/"))
                                        iconEntry = zipArchive.CreateEntry(String.Format("{0}.jpg", secondaryicon.ToLower()), CompressionLevel.Fastest);
                                    else
                                        iconEntry = zipArchive.CreateEntry(String.Format("icons/{0}.jpg", GetIconFilename(secondaryicon)), CompressionLevel.Fastest);
                                    using (var a = iconEntry.Open())
                                        iconStream.WriteTo(a);
                                    //using (Writer writer = new BinaryWriter(iconEntry.Open()))
                                    //writer.(iconStream);
                                }
                            }
                            if (icon.StartsWith("codex/"))
                            {
                                using (MemoryStream iconStream = GetIcon(secondaryicon, true))
                                {
                                    if (iconStream != null)
                                    {
                                        ZipArchiveEntry iconEntry;
                                        iconEntry = zipArchive.CreateEntry(String.Format("codex/{0}_thumb.jpg", GetIconFilename(secondaryicon)), CompressionLevel.Fastest);
                                        using (var a = iconEntry.Open())
                                            iconStream.WriteTo(a);
                                    }
                                }
                            }
                        }
                    }
                }
                compressStream.Position = 0;
                WriteFile(compressStream, file);
            }
            GomLib.Models.Tooltip.language = "enMale";
        }
        public static void CreatCompressedIconOutput(IEnumerable<TorLib.HashFileInfo> iconList)
        {
            string file = "newIcons.zip";
            WriteFile("", file, false);
            using (var compressStream = new MemoryStream())
            {
                //create the zip in memory
                using (var zipArchive = new ZipArchive(compressStream, ZipArchiveMode.Create, true))
                {
                    foreach (var t in iconList)
                    {
                        IconParamter[] parms = new IconParamter[1];
                        var filename = String.Join("_", t.file.FileInfo.ph, t.file.FileInfo.sh);
                        using (MemoryStream iconStream = GetIcon(filename, t.file, false, parms))
                        {
                            if (iconStream != null)
                            {
                                ZipArchiveEntry iconEntry = zipArchive.CreateEntry(String.Format("icons/{0}.jpg", filename), CompressionLevel.Fastest);
                                using (var a = iconEntry.Open())
                                    iconStream.WriteTo(a);
                            }
                        }
                    }
                }
                compressStream.Position = 0;
                WriteFile(compressStream, file);
            }
        }

        private static MemoryStream GetIcon(string icon, params IconParamter[] encodingParams)
        {
            return GetIcon(icon, false, encodingParams);
        }
        private static MemoryStream GetIcon(string icon, bool generateThumb, params IconParamter[] encodingParams)
        {
            if (icon == null) return null;
            using (var file = dom._assets.FindFile(String.Format("/resources/gfx/{0}.dds", icon)))
            {
                if (file == null) return null;
                var filename = String.Join("_", file.FileInfo.ph, file.FileInfo.sh);
                return GetIcon(filename, file, generateThumb, encodingParams);
            }
        }

        private static MemoryStream GetIcon(string filename, TorLib.File file, bool generateThumb, params IconParamter[] encodingParams)
        {
            if (file == null) return null;
            /*using (*/
            MemoryStream outputStream = new MemoryStream();//)
            //{
            DevIL.ImageImporter imp = new DevIL.ImageImporter();

            DevIL.Image dds;
            using (MemoryStream iconStream = (MemoryStream)file.OpenCopyInMemory())
                dds = imp.LoadImageFromStream(DevIL.ImageType.Dds, iconStream);
            var myparams = new EncoderParameters(1);
            ImageCodecInfo encoder;
            if (encodingParams.Contains(IconParamter.PngFormat))
            {
                myparams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                encoder = GetEncoder(ImageFormat.Png);
            }
            else
            {
                myparams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                encoder = GetEncoder(ImageFormat.Jpeg);
            }

            DevIL.ImageExporter exp = new DevIL.ImageExporter();
            if (dds.Width == 52 && dds.Height == 52) // needs cropped
            {
                var iconData = dds.GetImageData(0);

                System.Drawing.Bitmap iconBM = new System.Drawing.Bitmap(iconData.Width, iconData.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                for (int k = 0; k < iconData.Height * iconData.Width; k++) // loop through image data
                {
                    Color iconPixel = Color.FromArgb(iconData.Data[k * 4 + 3], // copy pixel values
                                iconData.Data[k * 4 + 0],
                                iconData.Data[k * 4 + 1],
                                iconData.Data[k * 4 + 2]);

                    iconBM.SetPixel(k % iconData.Width, (int)k / iconData.Width, iconPixel); //save pixel in new bitmap
                }

                Bitmap croppedIconBM = iconBM.Clone(new Rectangle(0, 0, 50, 50), iconBM.PixelFormat); // crop Bitmap
                //croppedIconBM.Save(outputStream, System.Drawing.Imaging.ImageFormat.Png); //Bitmap to PNG Stream
                croppedIconBM.Save(outputStream, encoder, myparams); //Bitmap to PNG Stream
            }
            else
            {
                var iconData = dds.GetImageData(0);

                //System.Drawing.Bitmap iconBM = new System.Drawing.Bitmap(iconData.Width, iconData.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                //for (int k = 0; k < iconData.Height * iconData.Width; k++) // loop through image data
                //{
                //    Color iconPixel = Color.FromArgb(iconData.Data[k * 4 + 3], // copy pixel values
                //                iconData.Data[k * 4 + 0],
                //                iconData.Data[k * 4 + 1],
                //                iconData.Data[k * 4 + 2]);

                //    iconBM.SetPixel(k % iconData.Width, (int)k / iconData.Width, iconPixel); //save pixel in new bitmap
                //}

                //iconBM.Save(outputStream, jpgEncoder, myparams); //Bitmap to PNG Stream
                if (encodingParams.Contains(IconParamter.PngFormat))
                {

                    if (generateThumb)
                    {
                        dds.Resize(dds.Width / 4, dds.Height / 4, 4, DevIL.SamplingFilter.ScaleLanczos3, true);
                    }
                    exp.SaveImageToStream(dds, DevIL.ImageType.Png, outputStream);
                }
                else
                {
                    using (MemoryStream taco = new MemoryStream())
                    {

                        exp.SaveImageToStream(dds, DevIL.ImageType.Bmp, taco); //save DDS to stream in jpg format
                        System.Drawing.Bitmap iconBM = new System.Drawing.Bitmap(taco);
                        if (generateThumb)
                        {
                            Bitmap resized = new Bitmap(iconBM, new System.Drawing.Size(iconBM.Width / 4, iconBM.Height / 4));
                            resized.Save(outputStream, encoder, myparams);
                        }
                        else
                            iconBM.Save(outputStream, encoder, myparams); //Bitmap to JPG Stream
                    }
                }
            }

            //WriteFile(outputStream, String.Format("/{0}/Images/{1}.png", directory, filename));
            return outputStream;
            //}
        }
        private static string GetIconFilename(string icon)
        {
            if (icon == null) return "";
            using (var file = dom._assets.FindFile(String.Format("/resources/gfx/{0}.dds", icon)))
            {
                if (file == null)
                    using (var file2 = dom._assets.FindFile(String.Format("/resources/gfx/icons/{0}.dds", icon)))
                    {
                        if (file2 == null)
                            return "";
                        return String.Join("_", file2.FileInfo.ph, file2.FileInfo.sh);
                    }
                return String.Join("_", file.FileInfo.ph, file.FileInfo.sh);
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        public enum IconParamter
        {
            PngFormat,
        }
        #endregion
    }
}
