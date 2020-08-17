using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class TalentLoader
    {
        const long NameLookupKey = -2761358831308646330;
        const long DescriptionLookupKey = 2806211896052149513;

        Dictionary<ulong, Talent> idMap;
        Dictionary<string, Talent> nameMap;
        readonly DataObjectModel _dom;

        public TalentLoader(DataObjectModel dom)
        {
            _dom = dom;
            if (idMap == null || nameMap == null)
            {
                Flush();
            }
        }

        public void Flush()
        {
            idMap = new Dictionary<ulong, Talent>();
            nameMap = new Dictionary<string, Talent>();
        }

        public string ClassName
        {
            get { return "talType"; }
        }

        public Models.Talent Load(ulong nodeId)
        {
            if (idMap.TryGetValue(nodeId, out Talent result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(nodeId);
            Talent tal = new Talent();
            return Load(tal, obj);
        }

        public Models.Talent Load(string fqn)
        {
            if (nameMap.TryGetValue(fqn, out Talent result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(fqn);
            Talent tal = new Talent();
            return Load(tal, obj);
        }

        public Models.Talent Load(GomObject obj)
        {
            Talent tal = new Talent();
            return Load(tal, obj);
        }

        public Models.GameObject CreateObject()
        {
            return new Models.Talent();
        }

        public Models.Talent Load(Models.GameObject obj, GomObject gom)
        {
            if (gom == null) { return (Talent)obj; }

            return Load(obj as Talent, gom);
        }

        public Models.Talent Load(Models.Talent tal, GomObject obj)
        {
            if (obj == null) { return null; }
            if (tal == null) { return null; }

            tal.Fqn = obj.Name;
            tal.NodeId = obj.Id;
            tal.Dom_ = _dom;
            tal.References = obj.References;

            var textLookup = obj.Data.Get<Dictionary<object, object>>("locTextRetrieverMap");
            // Load Talent Name
            var nameLookupData = (GomObjectData)textLookup[NameLookupKey];
            tal.NameId = nameLookupData.Get<long>("strLocalizedTextRetrieverStringID");
            tal.LocalizedName = _dom.stringTable.TryGetLocalizedStrings(tal.Fqn, nameLookupData);
            Normalize.Dictionary(tal.LocalizedName, tal.Fqn);
            tal.Name = tal.LocalizedName["enMale"];
            tal.Id = obj.Id;//(ulong)(tal.NameId >> 32);
            // Load Talent Description
            var descLookupData = (GomObjectData)textLookup[DescriptionLookupKey];
            tal.DescriptionId = descLookupData.Get<long>("strLocalizedTextRetrieverStringID");
            tal.LocalizedDescription = _dom.stringTable.TryGetLocalizedStrings(tal.Fqn, descLookupData);
            tal.Description = tal.LocalizedDescription["enMale"];

            var statLookupList = obj.Data.ValueOrDefault<List<object>>("talTalentData", null);
            tal.RankStats = new List<Talent.RankStatData>();
            foreach (GomObjectData statLookup in statLookupList)
            {
                Talent.RankStatData rankstd = new Talent.RankStatData();
                var defensiveStatList = statLookup.ValueOrDefault<List<object>>("talTalentDefensiveStats", null);
                rankstd.DefensiveStats = new List<Talent.StatData>();
                foreach (GomObjectData defensiveStat in defensiveStatList)
                {//create a stat type that will hold the stat, the value, a bool, and the affected ability
                    Talent.StatData statData = new Talent.StatData(_dom, defensiveStat);
                    rankstd.DefensiveStats.Add(statData);
                }
                var offensiveStatList = statLookup.ValueOrDefault<List<object>>("talTalentOffensiveStats", null);
                rankstd.OffensiveStats = new List<Talent.StatData>();
                foreach (GomObjectData offensiveStat in offensiveStatList)
                {
                    Talent.StatData statData = new Talent.StatData(_dom, offensiveStat);
                    rankstd.OffensiveStats.Add(statData);
                }
                tal.RankStats.Add(rankstd);
            }
            tal.Ranks = tal.RankStats.Count;

            tal.TalentVisibility = obj.Data.ValueOrDefault<ScriptEnum>("talTalentVisibility", new ScriptEnum()); //.Value;
            tal.Icon = obj.Data.ValueOrDefault<string>("talTalentIcon", String.Empty);
            _dom._assets.icons.Add(tal.Icon);

            //Read tokens
            tal.TokenList = new List<float>();
            if (obj.Data.ContainsKey("talTalentTokens"))
            {
                foreach (GomObjectData tokDesc in obj.Data.Get<List<object>>("talTalentTokens"))
                {
                    if (tokDesc.ContainsKey("")) throw new Exception("ERROR: Unexpected ablDescriptionTokenType in talent " + tal.Fqn + " (" + tal.NodeId + ")");
                    tal.TokenList.Add(tokDesc.ValueOrDefault<float>("ablDescriptionTokenMultiplier"));
                }
            }

            //Parse tokens in description for XML/Text output
            if (tal.Ranks > 1) tal.DescriptionRank2 = tal.Description;
            if (tal.Ranks > 2) tal.DescriptionRank3 = tal.Description;
            for (int i = 0; i < tal.TokenList.Count; i++)
            {
                tal.Description = ReplaceTokens(tal.Description, i + 1, tal.TokenList[i].ToString());
                if (tal.Ranks > 1) tal.DescriptionRank2 = ReplaceTokens(tal.DescriptionRank2, i + 1, (tal.TokenList[i] * 2).ToString());
                if (tal.Ranks > 2) tal.DescriptionRank3 = ReplaceTokens(tal.DescriptionRank3, i + 1, (tal.TokenList[i] * 3).ToString());
            }

            obj.Unload();
            return tal;
        }

        private string ReplaceTokens(string desc, int _, string replacement)
        {
            if (string.IsNullOrEmpty(desc) || string.IsNullOrEmpty(replacement))
            {
            }

            /*string searchSimple = "<<" + tokenIndex.ToString() + ">>";
            if (System.Text.RegularExpressions.Regex.IsMatch(desc, searchSimple))
            {
                return System.Text.RegularExpressions.Regex.Replace(desc, searchSimple, replacement);
            }
            else
            {
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("<<" + tokenIndex.ToString() + "\\[([^/]+)/([^/]+)/([^/]+)\\]>>");
                System.Text.RegularExpressions.Match match = regex.Match(desc);
                if (match.Success)
                {
                    if (replacement == "0")
                    {
                        return regex.Replace(desc, match.Groups[1].Value.Replace("%d", replacement));
                    }
                    else if (replacement == "1")
                    {
                        return regex.Replace(desc, match.Groups[2].Value.Replace("%d", replacement));
                    }
                    else
                    {
                        return regex.Replace(desc, match.Groups[3].Value.Replace("%d", replacement));
                    }
                }
            }*/
            return desc;
        }

        public void LoadObject(Models.GameObject loadMe, GomObject obj)
        {
            GomLib.Models.Talent tal = (Models.Talent)loadMe;
            Load(tal, obj);
        }

        public void LoadReferences(Models.GameObject obj, GomObject gom)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (gom is null)
            {
                throw new ArgumentNullException(nameof(gom));
            }
            // No references to load
        }
    }
}
