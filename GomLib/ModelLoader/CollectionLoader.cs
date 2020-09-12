using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class CollectionLoader
    {
        public Dictionary<object, object> CollectionItemsData;

        private readonly DataObjectModel _dom;

        public CollectionLoader(DataObjectModel dom)
        {
            _dom = dom;
            _dom.mtxStorefrontEntryLoader = new MtxStorefrontEntryLoader(_dom);
            _dom.mtxStorefrontEntryLoader.Flush();
        }

        public void Flush()
        {
            CollectionItemsData = new Dictionary<object, object>();
        }

        public Collection Load(long id)
        {
            if (CollectionItemsData.Count == 0)
            {
                CollectionItemsData = _dom.GetObject("colCollectionItemsPrototype").Data.Get<Dictionary<object, object>>("colCollectionItemsData");
            }

            _ = new object();
            CollectionItemsData.TryGetValue(id, out object mtxData);

            Collection mtx = new Collection();
            return Load(mtx, id, (GomObjectData)mtxData);
        }

        public Collection Load(Collection col, long Id, GomObjectData obj)
        {
            if (obj == null) { return col; }
            if (col == null) { return null; }

            if (_dom.mtxStorefrontEntryLoader.MtxStoreFrontData.Count == 0)
            {
                GomObject MtxStoreFrontDataObject = _dom.GetObject("mtxStorefrontInfoPrototype");
                _dom.mtxStorefrontEntryLoader.MtxStoreFrontData = MtxStoreFrontDataObject.Data.Get<Dictionary<object, object>>("mtxStorefrontData");
                MtxStoreFrontDataObject.Unload();
            }
            object mtxData = new object();
            _dom.mtxStorefrontEntryLoader.MtxStoreFrontData.TryGetValue(Id, out mtxData);

            col.Dom_ = _dom;
            col.Prototype = "colCollectionItemsPrototype";
            col.ProtoDataTable = "colCollectionItemsData";

            var unknownId = ((GomObjectData)mtxData).ValueOrDefault<long>("4611686297592334024", 0); //Always 3042172580397056 for collection items
            col.UnknowntextId = unknownId;
            col.Unknowntext = _dom.stringTable.TryGetString("str.gui.mtxstorefrontitems", unknownId); // need to find the right stringtable for this.
            col.Localizedunknowntext = _dom.stringTable.TryGetLocalizedStrings("str.gui.mtxstorefrontitems", unknownId);

            var rarityId = ((GomObjectData)mtxData).ValueOrDefault<long>("mtxRarityDescriptionId", 0);
            col.RarityDescId = rarityId;
            col.RarityDesc = _dom.stringTable.TryGetString("str.gui.mtxstorefrontitems", rarityId);
            col.LocalizedRarityDesc = _dom.stringTable.TryGetLocalizedStrings("str.gui.mtxstorefrontitems", rarityId);

            var bulletPointIds = ((GomObjectData)mtxData).ValueOrDefault("mtxBulletPointDescriptionIds", new List<object>()).ConvertAll(x => (long)x);
            col.BulletPoints = new List<string>();
            foreach (var bullet in bulletPointIds) { col.BulletPoints.Add(_dom.stringTable.TryGetString("str.gui.mtxstorefrontitems", bullet)); }
            col.LocalizedBulletPoints = new List<Dictionary<string, string>>();
            foreach (var bullet in bulletPointIds) { col.LocalizedBulletPoints.Add(_dom.stringTable.TryGetLocalizedStrings("str.gui.mtxstorefrontitems", bullet)); }

            var nameId = ((GomObjectData)mtxData).ValueOrDefault<long>("mtxName", 0);
            col.Name = _dom.stringTable.TryGetString("str.gui.mtxstorefrontitems", nameId);
            col.LocalizedName = _dom.stringTable.TryGetLocalizedStrings("str.gui.mtxstorefrontitems", nameId);

            col.Id = Id;
            col.CreationIndex = (long)obj.ValueOrDefault("colCreationIndex", new object()); // 3460
            col.Icon = obj.ValueOrDefault("colCollectionIcon", ""); // "Mtx.Season3.Bikini_V02"
            _dom._assets.icons.AddMtx(col.Icon);

            col.IsFoundInPacks = obj.ValueOrDefault("colItemIsFoundInPacks", false); // True

            col.LinkedId = obj.ValueOrDefault<long>("colLinkedId", 0); // 1597

            List<object> unknownList = obj.ValueOrDefault<List<object>>("4611686297968184000", null); // seems to be always empty.

            /*if (unknownList.Count != 0) {                                         //This is some code to isolate cases where this list might have values
                var u = DataObjectModel.GetObject((ulong)unknownList[0]);
                if (u != null) {
                    string stopHere = ""; } }*/

            List<object> unknownList2 = obj.Get<List<object>>("4611686297983034002"); // { 15607973448745700563 } - need to find out what this is.
            /*if (unknownList.Count != 0) {                                         //This is some code to isolate cases where this list might have values
                var u = DataObjectModel.GetObject((ulong)unknownList2[0]);
                if (u != null) {
                    string stopHere = ""; } }*/

            //col.CategoryId = (long)obj.ValueOrDefault<object>("mtxStorefrontMainCategory", new object()); // 610 looked up in colCollectionItemsPrototype("colCollectionItemsCategoryData")

            col.ItemIdsList = obj.ValueOrDefault("colItemList", new List<object>()).ConvertAll(x => (ulong)x);
            /*{ 16141048636041134811, 16140999226559259282, 16141134542521957469, 
            * 16140928499777528367, 16141006708961340344, 16141053294373613055, 
            * 16140959691716914276,  } - items*/
            //Add some code here to load each item.
            /*col.ItemList = new List<Item>();
            foreach (var item in col.ItemIdsList)
            {
                col.ItemList.Add(_dom.itemLoader.Load(item));
            }*/

            col.AbilityIdsList = obj.Get<List<object>>("colAbilityList").ConvertAll(x => (ulong)x);  // { 16140962263260863698 } - ability
            /*col.AbilityList = new List<Ability>();
            foreach (var ability in col.AbilityIdsList)
            {
                col.AbilityList.Add(_dom.abilityLoader.Load(ability));
            }*/

            var titleShortIdLookupList = obj.ValueOrDefault("colCollectionsTitleId", new List<object>()).ConvertAll(x => (long)x); /* { 150 } - always 1 value that you lookup in
                                                                                                                                 * colCollectionItemsPrototype
                                                                                                                                 * colCollectionsTitleData                   */

            var emoteShortIdLookupList = obj.ValueOrDefault("colCollectionsEmoteId", new List<object>()).ConvertAll(x => (long)x); /* { 202 } - always 1 value that you lookup in
                                                                                                                                 * colCollectionItemsPrototype
                                                                                                                                 * colCollectionsEmoteData                   */

            var longBoolDic = obj.Get<Dictionary<object, object>>("4611686347575727004"); /* [ 93001972900066292: True, 4095493194534377413: True, 4104425345487988702: True,  ]
                                                                                              * need to figure out what the heck these are */

            List<object> linkedListO = obj.ValueOrDefault<List<object>>("4611686347582697000", null);
            if (linkedListO != null)
            {
                List<long> linkedList = linkedListO.ConvertAll(x => (long)x);
            }

            var unknownlong = obj.ValueOrDefault<long>("4611686348190277001", 0); // -7824174851411027002 - not sure what this is

            col.RequiredLevel = obj.ValueOrDefault<long>("colCollectionsRequiredLevel", 1); // 1

            var alternateUnlocks = obj.ValueOrDefault("4611686348190657005", new Dictionary<object, object>());
            col.HasAlternateUnlocks = (alternateUnlocks.Count > 0);
            col.AlternateUnlocksMap = new Dictionary<ulong, List<ulong>>();

            if (alternateUnlocks.Count > 0)
            {
                col.AlternateUnlocksMap = alternateUnlocks.ToDictionary(p => (ulong)p.Key, p => ((List<object>)p.Value).ConvertAll(x => (ulong)x));
            }


            List<ulong> collectionItemsList2 = new List<ulong>(); // Might be items granted.
            collectionItemsList2 = obj.ValueOrDefault("4611686348671327000", new List<object>()).ConvertAll(x => (ulong)x);
            /*{ 16141048636041134811, 16140999226559259282, 16141134542521957469, 
            * 16140928499777528367, 16141006708961340344, 16141053294373613055, 
            * 16140959691716914276,  } - items*/

            return col;
        }
    }
}
