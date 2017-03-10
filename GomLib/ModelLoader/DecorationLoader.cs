using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;
using System.Xml.Linq;

namespace GomLib.ModelLoader
{
    public class DecorationLoader : IModelLoader
    {
        DataObjectModel _dom;

        public DecorationLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            _HookList = null;
        }

        internal Dictionary<long, Hook> _HookList;

        public Dictionary<long, Hook> HookList
        {
            get
            {
                if (_HookList == null)
                    _HookList = Hook.LoadHooks(_dom);
                return _HookList;
            }
        }

        public string ClassName
        {
            get { return "decDecoration"; }
        }

        public Models.GameObject CreateObject()
        {
            return new Models.Decoration();
        }

        private HashSet<string> fields = new HashSet<string>() {
            "decPrevObjRotationX",
            "decPrevObjRotationY",
            "decNameId",
            "decUseItemName",
            "decUnlockingItemId",
            "decDecorationId",
            "decPlcState",
            "decDefaultAnimation",
            "decMaxUnlockLimit",
            "decUnknownUnlockLimit",
            "decUniquePerLegacy",
            "decFactionPlacementRestriction",
            "decCategoryNameId",
            "decSubCategoryNameId",
            "decHookList",
            "decPrevCamHeightOff",
            "decPrevCamDisOff",
            "decRequiredAbilityType",
            "decRequiresAbilityUnlocked",
            "decGuildPurchaseCost"
            };

        public Decoration Load(Models.GameObject obj, GomObject gom)
        {
            if (gom == null) { return (Decoration)obj; }
            if (gom == null) { return null; }

            //if (HookList == null)
                //HookList = Hook.LoadHooks(_dom);

            var dec = (Models.Decoration)obj;

            dec.Id = gom.Id;
            dec.Fqn = gom.Name;
            dec._dom = _dom;
            dec.References = obj.References;
            var keys = gom.Data.Dictionary.Keys;
            foreach (var key in keys.Skip(3)) //Skip the first three keys as they are internal ones
            {
                if (!fields.Contains(key))
                {
                    //Console.WriteLine(String.Join(", ", key, dec.Fqn));
                    //if (key != "Script_Type" && key != "Script_TypeId" && key != "Script_NumFields")
                    //throw new IndexOutOfRangeException();
                }
            }

            var nameTable = _dom.stringTable.Find("str.dec");
            
            dec.UseItemName = gom.Data.ValueOrDefault<bool>("decUseItemName", false);
            if (!dec.UseItemName)
            {
                dec.NameId = gom.Data.ValueOrDefault<long>("decNameId", 0);
                //dec.Name = nameTable.GetText(dec.NameId, "str.dec");
                dec.LocalizedName = nameTable.GetLocalizedText(dec.NameId, "str.dec");
            }

            dec.decPrevObjRotationX = gom.Data.ValueOrDefault<float>("decPrevObjRotationX", 0);
            dec.decPrevObjRotationY = gom.Data.ValueOrDefault<float>("decPrevObjRotationY", 0);

            dec.UnlockingItemId = gom.Data.ValueOrDefault<ulong>("decUnlockingItemId", 0);
            //dec.UnlockingItem = _dom.itemLoader.Load(dec.UnlockingItemId);
            dec.SourceDict = dec.UnlockingItem.StrongholdSourceNameDict;
            dec.LocalizedSourceDict = dec.UnlockingItem.LocalizedStrongholdSourceNameDict;

            if (dec.UseItemName)
            {
                dec.NameId = ((Item)dec.UnlockingItem).NameId;
                //dec.Name = ((Item)dec.UnlockingItem).Name;
                dec.LocalizedName = ((Item)dec.UnlockingItem).LocalizedName;
            }
            Normalize.Dictionary(dec.LocalizedName, dec.Fqn);
            dec.Name = dec.LocalizedName["enMale"];

            //dec.Id = (ulong)(dec.NameId >> 32);

            dec.DecorationId = gom.Data.ValueOrDefault<ulong>("decDecorationId", 0);

            var decoration = _dom.GetObject(dec.DecorationId);
            dec.DecorationFqn = decoration.Name;

            string decorationPrefix = decoration.Name.Substring(0, 3);
            switch (decorationPrefix)
            {
                case "dyn":
                    break;
                case "plc":
                    break;
                case "npc":
                    dec.DecorationObject = new Npc();
                    _dom.npcLoader.Load(dec.DecorationObject, decoration);
                    if (dec.Name == "")
                    {
                        dec.NameId = ((Npc)dec.DecorationObject).NameId;
                        dec.Name = ((Npc)dec.DecorationObject).Name;
                        dec.LocalizedName = ((Npc)dec.DecorationObject).LocalizedName;
                    }
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }

            dec.State = gom.Data.ValueOrDefault<string>("decPlcState", null);

            dec.DefaultAnimation = gom.Data.ValueOrDefault<string>("decDefaultAnimation", null);

            var data = gom.Data.ValueOrDefault<object>("4611686351126997007", null);

            if (data != null)
            {
                //Console.WriteLine(String.Format("Field {0} set to: {1} type: {2}, {3}", "4611686351126997007", data.ToString(), data.GetType().ToString(), gom.Name));
            }

            dec.MaxUnlockLimit = gom.Data.ValueOrDefault<long>("decMaxUnlockLimit", 0);
            dec.F2PLimit = gom.Data.ValueOrDefault<long>("decUnknownUnlockLimit", 0);

            dec.UniquePerLegacy = gom.Data.ValueOrDefault<bool>("decUniquePerLegacy", false);

            if (dec.UniquePerLegacy)
            {
                //Console.WriteLine(String.Format("Field {0} set to: {1}, {2}", "decUniquePerLegacy", dec.UniquePerLegacy, gom.Name));
            }

            ScriptEnum faction = gom.Data.ValueOrDefault<ScriptEnum>("decFactionPlacementRestriction", null);

            if (faction != null)
            {
                dec.FactionPlacementRestriction = faction.ToString();
            }

            dec.CategoryId = gom.Data.ValueOrDefault<long>("decCategoryNameId", 0);

            var decCats = _dom.GetObject("decCategoriesTable");

            var catNameTable = _dom.stringTable.Find("str.gui.auctionhouse");

            if (decCats != null)
            {
                var tableNameLookup = decCats.Data.Get<Dictionary<object, object>>("decCategoryTable");

                object catTab;
                tableNameLookup.TryGetValue(dec.CategoryId, out catTab);
                dec.CategoryNameId = ((GomObjectData)catTab).Get<long>("decCategoryTableNameId");
                dec.CategoryName = catNameTable.GetText(dec.CategoryNameId, "str.gui.auctionhouse");
                dec.LocalizedCategory = catNameTable.GetLocalizedText(dec.CategoryNameId, "str.gui.auctionhouse");
            }

            dec.SubCategoryId = gom.Data.ValueOrDefault<long>("decSubCategoryNameId", 0);

            var decSubCats = _dom.GetObject("decSubCategoriesTable");

            if (decSubCats != null)
            {
                var tableNameLookup = decSubCats.Data.Get<Dictionary<object, object>>("decSubCategoryTable");

                object catTab;
                tableNameLookup.TryGetValue(dec.SubCategoryId, out catTab);
                dec.SubCategoryNameId = ((GomObjectData)catTab).Get<long>("decSubCategoryTableNameId");
                dec.SubCategoryName = catNameTable.GetText(dec.SubCategoryNameId, "str.gui.auctionhouse");
                dec.LocalizedSubCategory = catNameTable.GetLocalizedText(dec.SubCategoryNameId, "str.gui.auctionhouse");
            }

            var hookList = gom.Data.ValueOrDefault<Dictionary<object, object>>("decHookList", new Dictionary<object, object>()).ToDictionary(x => (long)x.Key, y => (bool)y.Value);

            dec.Hooks = hookList;

            dec.AvailableHooks = new List<string>();

            foreach (var hook in dec.Hooks)
            {
                Hook h;
                HookList.TryGetValue(hook.Key, out h);
                if (h == null)
                    dec.AvailableHooks.Add(String.Format("Unknown Hooktype: {0}", hook.Key));
                else
                    dec.AvailableHooks.Add(h.Name);
            }

            dec.PrevCamHeightOff = gom.Data.ValueOrDefault<float>("decPrevCamHeightOff", 0);
            dec.PrevCamDisOff = gom.Data.ValueOrDefault<float>("decPrevCamDisOff", 0);

            ScriptEnum comp = gom.Data.ValueOrDefault<ScriptEnum>("decRequiredAbilityType", null);
            if (comp != null)
            {
                dec.StubType = comp.ToString();
                if (dec.StubType == "decStubTypeFallback")
                {
                    var decProto = _dom.GetObject("decorationsPrototype");
                    Dictionary<object, object> decCompanionHoloTable = decProto.Data.ValueOrDefault<Dictionary<object, object>>("decCompanionHoloTable", new Dictionary<object, object>());
                    var results = decCompanionHoloTable.Where(x => (ulong)x.Value == dec.Id);
                    if (results.Count() > 1)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    else
                    {
                        if (results.Count() > 0)
                        {
                            ulong baseId = (ulong)results.First().Key;
                            if (baseId != dec.Id) //self-referencing decoration check!
                            {
                                var baseDecoration = _dom.decorationLoader.Load(baseId);
                                if (baseDecoration != null)
                                {
                                    dec.Name = String.Format("{0} - Holo", baseDecoration.Name);
                                    dec.SourceDict.Add(0, "Fallback");
                                }
                            }
                        }
                    }

                }
            }

            dec.RequiresAbilityUnlocked = gom.Data.ValueOrDefault<bool>("decRequiresAbilityUnlocked ", false);

            dec.GuildPurchaseCost = gom.Data.ValueOrDefault<long>("decGuildPurchaseCost", 0);

            /*if (dec.Name == "")//obsolete debugging code
            {
                string pausehere = "";
            }*/
            gom.Unload();
            return dec;
        }

        public void LoadReferences(Models.GameObject obj, GomObject gom)
        {
            var itm = (Models.Decoration)obj;
        }

        public void LoadObject(Models.GameObject obj, GomObject gom)
        {
            Load(obj, gom);
        }

        public Models.Decoration Load(ulong nodeId)
        {
            GomObject obj = _dom.GetObject(nodeId);
            if (obj == null) { return null; }
            Models.Decoration itm = new Decoration();
            return Load(itm, obj);
        }

        public Models.Decoration Load(string fqn)
        {
            GomObject obj = _dom.GetObject(fqn);
            if (obj == null) { return null; }
            Models.Decoration itm = new Decoration();
            return Load(itm, obj);
        }

        public Models.Decoration Load(GomObject obj)
        {
            Models.Decoration itm = new Decoration();
            return Load(itm, obj);
        }
    }
}
