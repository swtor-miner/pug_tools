using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class CodexLoader
    {
        const long NameLookupKey = -8863348193830878519;
        const long CategoryLookupKey = -367464477071745071;
        const long DescriptionLookupKey = 1078249248256508798;

        Dictionary<ulong, Codex> idMap;
        Dictionary<string, Codex> nameMap;

        private DataObjectModel _dom;

        public CodexLoader(DataObjectModel dom)
        {
            _dom = dom;
            if (nameMap == null)
            {
                Flush();
            }
        }

        public void Flush()
        {
            idMap = new Dictionary<ulong, Codex>();
            nameMap = new Dictionary<string, Codex>();
        }

        public string ClassName
        {
            get { return "cdxType"; }
        }

        public Models.Codex Load(ulong nodeId)
        {
            Codex result;
            if (idMap.TryGetValue(nodeId, out result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(nodeId);
            Codex cdx = new Codex();
            return Load(cdx, obj);
        }

        public Models.Codex Load(string fqn)
        {
            Codex result;
            if (nameMap.TryGetValue(fqn, out result))
            {
                return result;
            }

            GomObject obj = _dom.GetObject(fqn);
            Codex cdx = new Codex();
            return Load(cdx, obj);
        }

        public Models.Codex Load(GomObject obj)
        {
            Codex cdx = new Codex();
            return Load(cdx, obj);
        }

        public Models.GameObject CreateObject()
        {
            return new Models.Codex();
        }

        public Models.Codex Load(Models.GameObject obj, GomObject gom)
        {
            if (gom == null) { return (Codex)obj; }

            return Load(obj as Codex, gom);
        }

        public Models.Codex Load(Models.Codex cdx, GomObject obj)
        {
            if (obj == null) { return null; }
            if (cdx == null) { return null; }

            cdx.Fqn = obj.Name;
            cdx.NodeId = obj.Id;
            cdx._dom = _dom;
            cdx.References = obj.References;
            cdx.Image = obj.Data.ValueOrDefault<string>("cdxImage", null);

            _dom._assets.icons.AddCodex(cdx.Image);

            var textLookup = obj.Data.ValueOrDefault<Dictionary<object,object>>("locTextRetrieverMap", null);
            var descLookup = (GomObjectData)textLookup[DescriptionLookupKey];
            var titleLookup = (GomObjectData)textLookup[NameLookupKey];
            object categoryLookup;
            if (textLookup.TryGetValue(CategoryLookupKey, out categoryLookup))
            {
                cdx.CategoryId = ((GomObjectData)categoryLookup).ValueOrDefault<long>("strLocalizedTextRetrieverStringID", 0);
                cdx.CategoryName = _dom.stringTable.Find("str.sys.codexcategories").GetText(cdx.CategoryId, cdx.Fqn);
            }

            var titleId = titleLookup.ValueOrDefault<long>("strLocalizedTextRetrieverStringID", 0);
            cdx.Id = (ulong)(titleId >> 32);

            cdx.LocalizedName = _dom.stringTable.TryGetLocalizedStrings(cdx.Fqn, titleLookup);
            cdx.Name = _dom.stringTable.TryGetString(cdx.Fqn, titleLookup);

            cdx.LocalizedDescription = _dom.stringTable.TryGetLocalizedStrings(cdx.Fqn, descLookup);
            cdx.Description = _dom.stringTable.TryGetString(cdx.Fqn, descLookup);

            cdx.Level = (int)obj.Data.ValueOrDefault<long>("cdxLevel", 0);
            if (cdx.LocalizedName.Values.ToString() == "") { cdx.IsHidden = true; }

            cdx.Faction = FactionExtensions.ToFaction((long)obj.Data.ValueOrDefault<long>("cdxFaction", 0));
            cdx.IsPlanet = obj.Data.ValueOrDefault<bool>("cdxIsPlanetCodex", false);

            Dictionary<object,object> classLookup = obj.Data.ValueOrDefault<Dictionary<object, object>>("cdxClassesLookupList", null);
            if (classLookup == null)
            {
                cdx.ClassRestricted = false;
            }
            else
            {
                cdx.ClassRestricted = true;
                cdx.Classes = new List<ClassSpec>();
                foreach (KeyValuePair<object, object> kvp in classLookup)
                {
                    if ((bool)kvp.Value)
                    {
                        cdx.Classes.Add(_dom.classSpecLoader.Load((ulong)kvp.Key));
                    }
                }
            }

            List<object> cdxPlanets = obj.Data.ValueOrDefault<List<object>>("cdxPlanets", null);
            if (cdxPlanets != null)
            {
                cdx.HasPlanets = true;
                cdx.PlanetsIds = cdxPlanets.ConvertAll(x => (ulong)x);
                cdx.Planets = new List<Codex>();
                foreach (var planetId in cdxPlanets)
                {
                    Codex planet = null;
                    if ((ulong)planetId != cdx.NodeId) planet = _dom.codexLoader.Load((ulong)planetId);
                    if (planet != null) cdx.Planets.Add(planet);
                }
            }

            obj.Unload();
            return cdx;
        }

        public void LoadObject(Models.GameObject loadMe, GomObject obj)
        {
            GomLib.Models.Codex cdx = (Models.Codex)loadMe;
            Load(cdx, obj);
        }

        public void LoadReferences(Models.GameObject obj, GomObject gom)
        {
            // No references to load
        }
    }
}
