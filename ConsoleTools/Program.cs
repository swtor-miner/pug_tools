using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using GomLib;
using GomLib.Models;

namespace ConsoleTools
{
    static class Program
    {
        public static string patch;
        public static string patchDir;
        public static string outputDir;
        public static DataObjectModel dom;

        static void Main(string[] args)
        {
            if(args == null)
            {
                Console.WriteLine("Missing Arguments");
                return;
            }
            else
            {
                Console.WriteLine(String.Join(", ", args));
                if(args.Count() == 0)
                {
                    args = new string[]{ "5.0P4", "J:\\swtor_db\\", "J:\\swtor_db\\processed\\"};
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
                    Smart smart = new Smart(Console.WriteLine);
                    smart.Link(dom);

                    getObjects("class.pc.advanced", "AdvancedClasses");
                    getPrototypeObjects("Conquests", "wevConquestInfosPrototype", "wevConquestTable");
                    getObjects("abl.", "Abilities");
                    getObjects("pkg.abilities", "AbilityPackages");
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
                }
                else
                    Console.WriteLine("Assets not found.");
            }
            //Console.ReadLine();
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
                    ProcessGameObjects(prefix, elementName);
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
                    ProcessGameObjects(prefix, elementName);
                    break;
            }
        }
        public static void getPrototypeObjects(string xmlRoot, string prototype, string dataTable)
        {
            Console.Write(String.Format("Getting {0} ", xmlRoot));
            ProcessProtoData(xmlRoot, prototype, dataTable);
        }

        public static void ProcessGameObjects(string gomPrefix, string xmlRoot)
        {
            bool classOverride = (xmlRoot == "AdvancedClasses");
            IEnumerable<GomObject> curItmList = getMatchingGomObjects(dom, gomPrefix);

            var curItmNames = curItmList.Select(x => x.Name).ToList();

            Dictionary<string, List<GomLib.Models.GameObject>> ObjectLists = new Dictionary<string, List<GomLib.Models.GameObject>>();
            var newItems = new List<GomLib.Models.GameObject>();

            int i = 0;
            int count = 0;
            
            i = 0;
            count = curItmList.Count();
            using (var progress = new ProgressBar())
            {

                foreach (var curObject in curItmList)
                {
                    progress.Report((double)i / count);
                    var obj = GameObject.Load(curObject, classOverride);
                    if (obj != null && obj.Id != 0) //apparently if the loader passes null back, sometimes data comes, too.....
                        newItems.Add(obj);
                    i++;
                }
            }
            ObjectLists.Add("Full", newItems);

            Console.Write(" - Generating Output ");
            count = 0;
            i = 0;
            foreach (var itmList in ObjectLists)
            {
                count += itmList.Value.Count;
            }

            foreach (var itmList in ObjectLists)
            {
                GameObjectListAsJSON(String.Join("", itmList.Key, xmlRoot), itmList.Value);
                //GetTips(xmlRoot, itmList.Value);
            }
            Console.WriteLine();
        }

        public static void ProcessProtoData(string xmlRoot, string prototype, string dataTable)
        {
            int i = 0;
            int count = 0;

            Dictionary<object, object> currentDataProto = new Dictionary<object, object>();
            GomObject currentDataObject = dom.GetObject(prototype);
            if (currentDataObject != null) //fix to ensure old game assets don't throw exceptions.
            {
                currentDataProto = currentDataObject.Data.Get<Dictionary<object, object>>(dataTable);
                currentDataObject.Unload();
            }

            var curIds = currentDataProto.Keys;
            Dictionary<string, List<GomLib.Models.PseudoGameObject>> ObjectLists = new Dictionary<string, List<GomLib.Models.PseudoGameObject>>();
            var newItems = new List<GomLib.Models.PseudoGameObject>();

            i = 0;
            count = curIds.Count();
            using (var progress = new ProgressBar())
            {
                foreach (var id in curIds)
                {
                    progress.Report((double)i / count);
                    object curData;
                    currentDataProto.TryGetValue(id, out curData);
                    GomLib.Models.PseudoGameObject curObj = PseudoGameObject.Load(xmlRoot, dom, id, curData);
                    newItems.Add(curObj);
                    i++;
                }
            }
            ObjectLists.Add("Full", newItems);

            Console.Write(" - Generating Output ");
            count = 0;
            i = 0;
            foreach (var itmList in ObjectLists)
            {
                count += itmList.Value.Count;
            }


            foreach (var itmList in ObjectLists)
            {
                PseudoGameObjectListAsJSON(String.Join("", itmList.Key, xmlRoot), itmList.Value);
            }
            Console.WriteLine();
        }

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
        private static void GameObjectListAsJSON(string prefix, List<GomLib.Models.GameObject> itmList)
        {
            int i = 0;
            short e = 0;
            string n = Environment.NewLine;
            var txtFile = new StringBuilder();
            string filename = String.Format("\\json\\{0}{1}", prefix, ".json");
            WriteFile(String.Format("{0}{1}", patch, n), filename, false);
            var count = itmList.Count();
            HashTableHashing.MurmurHash2Unsafe jsonHasher = new HashTableHashing.MurmurHash2Unsafe();
            //var blocksize = 100;
            //var blocks = count % blocksize;
            //if (blocks > 0)
            //{
            //    progressUpdate(i, count);
            //    for (int b = blocks - 1; b >= 0; b--) //go backwards so we can delete items :)
            //    {
            //        System.Collections.Concurrent.BlockingCollection<string> blockCollection = new System.Collections.Concurrent.BlockingCollection<string>();
            //        var block = itmList.GetRange(b * blocksize, blocksize);
            //        Parallel.ForEach(block, itm =>
            //        {
            //            string jsonBlock = itm.ToJSON();
            //            blockCollection.Add(jsonBlock);
            //        });
            //        i += blocksize;
            //        txtFile.Append(String.Join(Environment.NewLine, blockCollection));
            //        itmList.RemoveRange(b * blocksize, blocksize);
            //    }
            //}
            using (var progress = new ProgressBar())
            {
                for (int b = itmList.Count - 1; b >= 0; b--) //go backwards so we can delete values
                {
                    progress.Report((double)i / count);
                    if (e % 1000 == 1)
                    {
                        WriteFile(txtFile.ToString(), filename, true);
                        txtFile.Clear();
                        e = 0;
                    }

                    string jsonString = itmList[b].ToJSON(); // ConvertToJson(itm); //added method in Tools.cs
                    uint hash = jsonHasher.Hash(Encoding.ASCII.GetBytes(jsonString));
                    txtFile.Append(String.Format("{0},{1},{2}{3}", itmList[b].Base62Id, hash, jsonString, Environment.NewLine)); //Append it with a newline to the output.
                    itmList[b] = null;
                    i++;
                    e++;
                }
            }
            //foreach (var itm in itmList) //clean up what's left
            //{
            //    progressUpdate(i, count);
            //    if (e % 1000 == 1)
            //    {
            //        WriteFile(txtFile.ToString(), filename, true);
            //        txtFile.Clear();
            //        e = 0;
            //    }

            //    addtolist2(String.Format("{0}: {1}", prefix, itm.Fqn));

            //    string jsonString = itm.ToJSON(); // ConvertToJson(itm); //added method in Tools.cs
            //    txtFile.Append(jsonString + Environment.NewLine); //Append it with a newline to the output.
            //    i++;
            //    e++;
            //}
            Console.Write(String.Format(" - Done. ({0})", i));
            WriteFile(txtFile.ToString(), filename, true);
            DeleteEmptyFile(filename, i);
            itmList = null;
            GC.Collect();
            CreateGzip(filename);
        }
        private static void PseudoGameObjectListAsJSON(string prefix, List<GomLib.Models.PseudoGameObject> itmList)
        {
            int i = 0;
            short e = 0;
            string n = Environment.NewLine;
            var txtFile = new StringBuilder();
            string filename = String.Format("\\json\\{0}{1}", prefix, ".json");
            WriteFile(String.Format("{0}{1}", patch, n), filename, false);
            var count = itmList.Count();
            HashTableHashing.MurmurHash2Unsafe jsonHasher = new HashTableHashing.MurmurHash2Unsafe();
            using (var progress = new ProgressBar())
            {
                for (int c = 0; c < count; c++)
                {
                    progress.Report((double)i / count);
                    if (e % 1000 == 1)
                    {
                        WriteFile(txtFile.ToString(), filename, true);
                        txtFile.Clear();
                        e = 0;
                    }

                    string jsonString = itmList[c].ToJSON();
                    uint hash = jsonHasher.Hash(Encoding.ASCII.GetBytes(jsonString));
                    txtFile.Append(String.Format("{0},{1},{2}{3}", itmList[c].Base62Id, hash, jsonString, Environment.NewLine)); //Append it with a newline to the output.
                    itmList[c] = null;
                    i++;
                    e++;
                }
            }
            Console.Write(String.Format(" - Done. ({0})", i));
            WriteFile(txtFile.ToString(), filename, true);
            DeleteEmptyFile(filename, i);
            itmList = null;
            GC.Collect();
            CreateGzip(filename);
        }
        private static void GomObjectListAsJSON(string prefix, IEnumerable<GomLib.GomObject> itmList)
        {
            int i = 0;
            short e = 0;
            string n = Environment.NewLine;
            var txtFile = new StringBuilder();
            string filename = String.Join("", prefix, ".json");
            WriteFile("", filename, false);
            var count = itmList.Count();
            using (var progress = new ProgressBar())
            {
                foreach (var itm in itmList)
                {
                    progress.Report((double)i / count);
                    if (e % 1000 == 1)
                    {
                        WriteFile(txtFile.ToString(), filename, true);
                        txtFile.Clear();
                        e = 0;
                    }

                    //addtolist2(String.Format("{0}: {1}", prefix, itm.Name));

                    string jsonString = GameObject.Load(itm, false).ToJSON(); // ConvertToJson(itm); //added method in Tools.cs
                    txtFile.Append(jsonString + Environment.NewLine); //Append it with a newline to the output.
                    i++;
                    e++;
                }
            }
            Console.Write(String.Format(" - Done. ({0})", i));
            WriteFile(txtFile.ToString(), filename, true);
            DeleteEmptyFile(filename, i);
            itmList = null;
            GC.Collect();
            CreateGzip(filename);
        }
        private static void GetTooltips(string prefix, IEnumerable<GomLib.Models.GameObject> itmList)
        {
            
        }
        #endregion
    }
}
