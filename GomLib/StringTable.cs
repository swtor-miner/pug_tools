using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace GomLib
{
    public class StringTable : IEquatable<StringTable>
    {
        private Dictionary<string, StringTable> fqnMap;
        public Dictionary<string, string> failedFqns;

        DataObjectModel _dom;

        public StringTable(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            fqnMap = new Dictionary<string, StringTable>();
            failedFqns = new Dictionary<string, string>();
        }

        public StringTable Find(string fqn)
        {
            if (String.IsNullOrEmpty(fqn)) { return null; }

            StringTable result;
            if (failedFqns.ContainsKey(fqn)) { return null; }

            if (fqnMap.TryGetValue(fqn, out result))
            {
                return result;
            }

            result = new StringTable(fqn, _dom);
            if (result.StbFileExists())
            {
                try
                {
                    result.Load();
                    fqnMap.Add(fqn, result);
                    return result;
                }
                catch (Exception ex)
                {
                    failedFqns.Add(fqn, ex.Message);
                    return null;
                }
            }

            return null;
        }

        public Dictionary<long, StringTableEntry> data;
        public string Fqn { get; private set; }
        public int Version { get; private set; }
        public long Guid { get; private set; }
        public string OwnerFqn { get; private set; }
        public long OwnerId { get; private set; }

        private StringTable(string fqn, DataObjectModel dom)
        {
            this._dom = dom;
            this.Fqn = fqn;
        }

        public StringTableEntry GetEntry(long id)
        {
            StringTableEntry result;
            if (data.TryGetValue(id, out result))
            {
                return result;
            }

            return null;
        }

        public string GetText(long id, string forFqn)
        {
            return GetText(id, forFqn, "enMale");
        }

        public string GetText(long id, string forFqn, string localization)
        {
            StringTableEntry entry;
            if (data.TryGetValue(id, out entry))
            {
                if (entry.localizedText.ContainsKey(localization))
                {
                    return entry.localizedText[localization];
                }
                else return "";
            }

            //Console.WriteLine("Cannot find String {0} in StringTable {1} for {2}", id, this.Fqn, forFqn);
            return String.Empty;
        }

        public Dictionary<string, string> GetLocalizedText(long id, string forFqn)
        {
            StringTableEntry entry;
            if (data.TryGetValue(id, out entry))
            {
                if (entry.localizedText.Count > 0)
                {
                    return entry.localizedText;
                }
                else return null;
            }

            //Console.WriteLine("Cannot find String {0} in StringTable {1} for {2}", id, this.Fqn, forFqn);
            return null;
        }

        public Dictionary<string, string> GetOptionText(long id, string forFqn)
        {
            StringTableEntry entry;
            if (data.TryGetValue(id, out entry))
            {
                if (entry.hasOptionText)
                {
                    return entry.optionText;
                }
                else return null;
            }

            //Console.WriteLine("Cannot find String {0} in StringTable {1} for {2}", id, this.Fqn, forFqn);
            return null;
        }

        public bool StbFileExists()
        {
            List<string> localizations = new List<string> {
                "en-us",
                "fr-fr",
                "de-de"
            };

            foreach (string localization in localizations)
            {
                var path = String.Format("/resources/" + localization + "/{0}.stb", this.Fqn.Replace('.', '/'));
                if (_dom._assets.HasFile(path))
                {
                    return true;
                }
            }

            return false;
        }

        private void Load()
        {
            // Version with String Tables as XML files
            //var path = String.Format("/resources/en-us/{0}.str", this.Fqn.Replace('.','/'));
            //var file = Assets.FindFile(path);
            //if (file == null) { throw new Exception("File not found"); }

            //using (var fs = file.Open())
            //{
            //    var xmlReader = XmlReader.Create(fs);
            //    var xdoc = XDocument.Load(xmlReader);
            //    var xroot = xdoc.Root;

            //    this.Version = xroot.Attribute("version").AsInt();
            //    this.OwnerFqn = (string)xroot.Attribute("owner");
            //    this.OwnerId = xroot.Attribute("ownerID").AsLong();
            //    this.Guid = xroot.Attribute("GUID").AsLong();
            //    this.Fqn = (string)xroot.Attribute("fqn");
            //    var results = from row in xdoc.Descendants("string") select LoadString(row);
            //    data = results.ToDictionary(k => k.Id, v => v);
            //}

            // Version with String Tables as nodes
            //var enUsPath = "en-us." + this.Fqn;
            //var file = DataObjectModel.GetObject(enUsPath);
            //if (file == null) { throw new Exception("StringTable not found"); }

            //var strings = file.Data.strTableVariantStrings as IDictionary<object, object>; // Map<enum, Map<int, string>>
            //var entries = (IDictionary<object,object>)strings.First(kvp => ((ScriptEnum)kvp.Key).ToString() == "MaleMale").Value;
            //data = new Dictionary<long, StringTableEntry>();
            //foreach (var kvp in entries)
            //{
            //    var entry = new StringTableEntry()
            //    {
            //        Id = (long)kvp.Key,
            //        Text = (string)kvp.Value
            //    };
            //    data[entry.Id] = entry;
            //}
            List<string> localizations = new List<string> {
                "en-us",
                "fr-fr",
                "de-de"
            };
            bool foundAtLeastOneTable = false;

            data = new Dictionary<long, StringTableEntry>();

            foreach (var localization in localizations)
            {
                // Version with String Tables as unique file format contained in swtor_en-us_global_1.tor
                var path = String.Format("/resources/" + localization +"/{0}.stb", this.Fqn.Replace('.', '/'));
                var file = _dom._assets.FindFile(path);
                if (file == null) { continue; } //throw new Exception("File not found"); }
                foundAtLeastOneTable = true;

                using (var fs = file.OpenCopyInMemory())
                {
                    var br = new GomBinaryReader(fs, _dom);
                    br.ReadBytes(3);
                    int numStrings = br.ReadInt32();

                    long streamPos = 0;

                    for (var i = 0; i < numStrings; i++)
                    {
                        var entryId = br.ReadInt64();
                        var entry_t1 = br.ReadByte();
                        var entry_t2 = br.ReadByte();
                        var entry_A = br.ReadSingle();
                        var entryLength = br.ReadInt32();
                        var entryOffset = br.ReadInt32();
                        var entryLength2 = br.ReadInt32();

                        var entry = new StringTableEntry()
                        {
                            Id = entryId,
                            localizedText = new Dictionary<string, string> {
                                { "enMale", "" },
                                //{ "enFemale", "" },
                                { "frMale", "" },
                                { "frFemale", "" },
                                { "deMale", "" },
                                { "deFemale", "" },
                            },
                            optionText = new Dictionary<string, string> {
                                { "enMale", "" },
                                //{ "enFemale", "" },
                                { "frMale", "" },
                                { "frFemale", "" },
                                { "deMale", "" },
                                { "deFemale", "" },
                            }

                            //enMaleText = String.Empty
                        };

                        string text = "";
                        if (entryLength > 0)
                        {
                            streamPos = fs.Position;
                            fs.Position = entryOffset;
                            text = br.ReadFixedLengthString(entryLength);
                            fs.Position = streamPos;
                        }

                        if (!data.ContainsKey(entryId))
                        {
                            data[entryId] = entry;
                        }

                        if (text.Length > 0)
                        {
                            string textTag = "unknown";
                            switch (localization)
                            {
                                case "en-us":
                                    if(entry_t1 == 65 || entry_t1 == 80)
                                        textTag = "enMale";
                                    break;
                                case "de-de":
                                    if (entry_t1 == 65 || entry_t1 == 80)
                                        textTag = "deMale";
                                    else
                                        textTag = "deFemale";
                                    break;
                                case "fr-fr":
                                    if (entry_t1 == 65 || entry_t1 == 80)
                                        textTag = "frMale";
                                    else
                                        textTag = "frFemale";
                                    break;
                            }

                            if (entry_t1 == 65 || entry_t1 == 70)
                            {
                                data[entryId].localizedText[textTag] = text;
                            }
                            else if(entry_t1 == 80 || entry_t1 == 81)
                            {
                                data[entryId].hasOptionText = true;
                                data[entryId].optionText[textTag] = text;
                            }
                        }
                    }
                }
            }
            if (!foundAtLeastOneTable) { throw new Exception("File not found"); }
        }

        //private StringTableEntry LoadString(XElement row)
        //{
        //    StringTableEntry result = new StringTableEntry();
        //    result.Id = row.Attribute("id").AsLong();
        //    result.Text = (string)row.Element("text");
        //    result.TextFemale = (string)row.Element("textFemale");
        //    result.InAlien = row.Element("inAlien").AsBool();
        //    result.DisableVoRecording = row.Element("disableVoRecording").AsBool();
        //    return result;
        //}

        public string TryGetString(string fqn, GomObjectData textRetriever)
        {
            return TryGetString(fqn, textRetriever, "enMale");
        }

        public string TryGetString(string fqn, GomObjectData textRetriever, string localization)
        {
            string locBucket = textRetriever.ValueOrDefault<string>("strLocalizedTextRetrieverBucket", null);
            long strId = textRetriever.ValueOrDefault<long>("strLocalizedTextRetrieverStringID", -1);
            string defaultStr = textRetriever.ValueOrDefault<string>("strLocalizedTextRetrieverDesignModeText", String.Empty);

            if ((locBucket == null) || (strId == -1))
            {
                return defaultStr;
            }

            StringTable strTable = null;
            try
            {
                strTable = this.Find(locBucket);
            }
            catch
            {
                strTable = null;
            }

            if (strTable == null)
            {
                return defaultStr;
            }

            string result = strTable.GetText(strId, fqn, localization);
            return result ?? defaultStr;
        }

        public string TryGetString(string bucket, long textRetriever)
        {
            return TryGetString(bucket, textRetriever, "enMale");
        }

        public string TryGetString(string bucket, long textRetriever, string localization)
        {
            string locBucket = bucket;
            long strId = textRetriever;
            string defaultStr = null;

            if ((locBucket == null) || (strId == -1))
            {
                return defaultStr;
            }

            StringTable strTable = null;
            try
            {
                strTable = this.Find(locBucket);
            }
            catch
            {
                strTable = null;
            }

            if (strTable == null)
            {
                return defaultStr;
            }

            string result = strTable.GetText(strId, "", localization);
            return result ?? defaultStr;
        }

        public Dictionary<string, string> TryGetLocalizedStrings(string fqn, GomObjectData textRetriever)
        {
            string locBucket = textRetriever.ValueOrDefault<string>("strLocalizedTextRetrieverBucket", null);
            long strId = textRetriever.ValueOrDefault<long>("strLocalizedTextRetrieverStringID", -1);
            Dictionary<string, string> defaultStr = new Dictionary<string, string> {
            { "enMale", textRetriever.ValueOrDefault<string>("strLocalizedTextRetrieverDesignModeText", String.Empty) },
            //{ "enFemale", "" },
            { "frMale", "" },
            { "frFemale", "" },
            { "deMale", "" },
            { "deFemale", "" },
            };

            if ((locBucket == null) || (strId == -1))
            {
                return defaultStr;
            }

            StringTable strTable = null;
            try
            {
                strTable = this.Find(locBucket);
            }
            catch
            {
                strTable = null;
            }

            if (strTable == null)
            {
                return defaultStr;
            }

            Dictionary<string, string> result = strTable.GetLocalizedText(strId, fqn);
            return result ?? defaultStr;
        }

        public Dictionary<string, string> TryGetLocalizedStrings(string bucket, long textRetriever)
        {
            string locBucket = bucket;
            long strId = textRetriever;

            if ((locBucket == null) || (strId == -1))
            {
                return null;
            }

            StringTable strTable = null;
            try
            {
                strTable = this.Find(locBucket);
            }
            catch
            {
                strTable = null;
            }

            if (strTable == null)
            {
                return null;
            }

            return strTable.GetLocalizedText(strId, "");
        }

        public Dictionary<string, string> TryGetLocalizedOptionStrings(string fqn, GomObjectData textRetriever)
        {
            string locBucket = textRetriever.ValueOrDefault<string>("strLocalizedTextRetrieverBucket", null);
            long strId = textRetriever.ValueOrDefault<long>("strLocalizedTextRetrieverStringID", -1);

            if ((locBucket == null) || (strId == -1))
            {
                return null;
            }

            StringTable strTable = null;
            try
            {
                strTable = this.Find(locBucket);
            }
            catch
            {
                strTable = null;
            }

            if (strTable == null)
            {
                return null;
            }

            Dictionary<string, string> result = strTable.GetOptionText(strId, fqn);
            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            StringTable stb = obj as StringTable;
            if (stb == null) return false;

            return Equals(stb);
        }

        public bool Equals(StringTable stb)
        {
            if (stb == null) return false;

            if (ReferenceEquals(this, stb)) return true;

            if (this.Fqn != stb.Fqn)
                return false;

            var ssComp = new Models.DictionaryComparer<long, StringTableEntry>();
            if (!ssComp.Equals(this.data, stb.data))
                return false;

                //foreach (var entry in data)
            bool isEqual = true;
            System.Threading.Tasks.Parallel.ForEach(data, (entry, loopState) =>
            {
                if (isEqual)
                {
                    StringTableEntry prevEntry;
                    if (stb.data.TryGetValue(entry.Key, out prevEntry))
                    {
                        if (prevEntry == null)
                        {
                            isEqual = false;
                            loopState.Stop();
                        }

                        if (!entry.Value.Equals(prevEntry))
                        {
                            isEqual = false;
                            loopState.Stop();
                        }
                    }
                    else
                    {
                        isEqual = false;
                        loopState.Stop();
                    }
                }
            });

            return isEqual;
        }

        public override int GetHashCode()
        {
            int hash = Fqn.GetHashCode();
            hash ^= Version.GetHashCode();
            if (fqnMap != null) { hash ^= fqnMap.GetHashCode(); }
            if (data != null) { hash ^= data.GetHashCode(); }
            if (failedFqns != null) { hash ^= failedFqns.GetHashCode(); }
            return hash;
        }
    }
}
