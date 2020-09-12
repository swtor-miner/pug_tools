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
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using GomLib;
using TorLib;

namespace PugTools
{
    public partial class Tools
    {
        public void BuildBnkIdDict()
        {
            StringBuilder outputFile = new StringBuilder();
            Dictionary<uint, string> eventNameDict = new Dictionary<uint, string>();
            HashSet<string> eventNames = new HashSet<string>();

            LoadData();
            Addtolist("Building BNK ID Dictionary...");
            Addtolist2("Getting Data...");

            GomObject utlhydramusicevents = currentDom.GetObject("utlhydramusicevents");
            List<object> hmeUtlDatatableRows = utlhydramusicevents.Data.ValueOrDefault<List<object>>("utlDatatableRows", null);
            if (hmeUtlDatatableRows != null)
            {
                foreach (List<object> row in hmeUtlDatatableRows)
                {
                    if (row.Count >= 3)
                        eventNames.Add(row[2].ToString().ToLower());
                }
                hmeUtlDatatableRows.Clear();
            }

            GomObject sndAmbienceRegionsTablePrototype = currentDom.GetObject("sndAmbienceRegionsTablePrototype");
            Dictionary<object, object> sndAmbienceRegionsMap = sndAmbienceRegionsTablePrototype.Data.ValueOrDefault<Dictionary<object, object>>("sndAmbienceRegionsMap", null);
            if (sndAmbienceRegionsMap != null)
            {
                foreach (var row in sndAmbienceRegionsMap)
                {
                    eventNames.Add(row.Value.ToString().ToLower());
                }
                sndAmbienceRegionsMap.Clear();
            }

            GomObject sndareasoundbanks = currentDom.GetObject("sndareasoundbanks");
            List<object> asbUtlDatatableRows = sndareasoundbanks.Data.ValueOrDefault<List<object>>("utlDatatableRows", null);
            if (asbUtlDatatableRows != null)
            {
                foreach (List<object> row in asbUtlDatatableRows)
                {
                    if (row.Count >= 7)
                        eventNames.Add(row[6].ToString().ToLower());
                }
                asbUtlDatatableRows.Clear();
            }

            GomObject sndaudioregions = currentDom.GetObject("sndaudioregions");
            List<object> arUtlDatatableRows = sndaudioregions.Data.ValueOrDefault<List<object>>("utlDatatableRows", null);
            if (arUtlDatatableRows != null)
            {
                foreach (List<object> row in arUtlDatatableRows)
                {
                    if (row.Count >= 4)
                    {
                        eventNames.Add(row[2].ToString().ToLower());
                        eventNames.Add(row[3].ToString().ToLower());
                    }
                }
                arUtlDatatableRows.Clear();
            }

            GomObject vehsoundpackage = currentDom.GetObject("vehsoundpackage");
            List<object> vehUtlDatatableRows = vehsoundpackage.Data.ValueOrDefault<List<object>>("utlDatatableRows", null);
            if (vehUtlDatatableRows != null)
            {
                foreach (List<object> row in vehUtlDatatableRows)
                {
                    if (row.Count >= 6)
                    {
                        eventNames.Add(row[2].ToString().ToLower());
                        eventNames.Add(row[5].ToString().ToLower());
                    }
                }
                vehUtlDatatableRows.Clear();
            }

            foreach (var lib in currentAssets.libraries)
            {
                string path = lib.Location;
                //if (!lib.Loaded)
                //lib.Load();
                foreach (var arch in lib.archives)
                {
                    foreach (var file in arch.Value.files)
                    {
                        HashFileInfo hashInfo = new HashFileInfo(file.FileInfo.PrimaryHash, file.FileInfo.SecondaryHash, file);
                        if (hashInfo.IsNamed)
                        {
                            if (hashInfo.Extension != "epp")
                                continue;
                            Stream assetStream = hashInfo.File.OpenCopyInMemory();

                            XmlDocument doc = new XmlDocument();
                            doc.Load(assetStream);

                            XmlNodeList elemList = doc.GetElementsByTagName("dynamicData");

                            foreach (XmlNode node in elemList)
                            {
                                if (node.InnerText.Contains("@audio"))
                                {
                                    string eventName = node.InnerText.Replace("@audio=", "");
                                    //if (eventName.Contains(';'))
                                    //Console.WriteLine("pause here");
                                    eventNames.Add(eventName);
                                }
                            }
                        }
                    }
                }
            }

            if (eventNames.Count > 0)
            {
                foreach (var name in eventNames)
                {
                    uint id = FileFormats.File_Helpers.GetFNV1Hash(name);
                    if (id != 0)
                    {
                        if (!eventNameDict.Keys.Contains(id))
                        {
                            eventNameDict.Add(id, name);
                        }
                    }
                }
            }

            Addtolist2("Building Output...");

            if (eventNameDict.Count > 0)
            {
                foreach (var kvp in eventNameDict)
                {
                    outputFile.Append(kvp.Key.ToString() + " : " + kvp.Value + Environment.NewLine);
                }
                using (StreamWriter file2 = new StreamWriter(Config.ExtractPath + "bnk_id_dictionary.txt", false))
                {
                    file2.Write(outputFile.NullSafeToString());
                }
            }
            Addtolist2("Output Complete");
            EnableButtons();
        }
    }

    public class BnkIdDict
    {
        private static BnkIdDict instance;
        public bool Loaded = false;
        public Dictionary<uint, string> Data = new Dictionary<uint, string>();

        private BnkIdDict() { }

        public static BnkIdDict Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BnkIdDict();
                }
                if (!instance.Loaded)
                    instance.Load();
                return instance;
            }
        }

        public void Load()
        {
            if (Loaded) return;

            if (System.IO.File.Exists("bnk_id_dictionary.txt"))
            {
                string[] lines = System.IO.File.ReadAllLines("bnk_id_dictionary.txt");
                if (lines.Count() > 0)
                {
                    foreach (string line in lines)
                    {
                        if (line.Contains('#'))
                        {
                            string[] split = line.Split('#');
                            Data.Add(uint.Parse(split[0]), split[1]);
                        }
                    }
                }
            }
            Loaded = true;
        }

        public void Unload()
        {
            Data = new Dictionary<uint, string>();
            Loaded = false;
            GC.Collect();
        }
    }
}
