using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class MtxStorefrontEntryLoader
    {
        public Dictionary<object, object> MtxStoreFrontData;
        readonly DataObjectModel _dom;

        public MtxStorefrontEntryLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            MtxStoreFrontData = new Dictionary<object, object>();
        }

        public MtxStorefrontEntry Load(long id)
        {
            if (MtxStoreFrontData.Count == 0)
            {
                MtxStoreFrontData = _dom.GetObject("mtxStorefrontInfoPrototype").Data.Get<Dictionary<object, object>>("mtxStorefrontData");
            }

            _ = new object();
            MtxStoreFrontData.TryGetValue(id, out object mtxData);

            MtxStorefrontEntry mtx = new MtxStorefrontEntry();
            return Load(mtx, id, (GomObjectData)mtxData);
        }

        public MtxStorefrontEntry Load(MtxStorefrontEntry mtx, long Id, GomObjectData obj)
        {
            if (obj == null) { return mtx; }
            if (mtx == null) { return null; }

            mtx.Dom_ = _dom;
            mtx.Prototype = "mtxStorefrontInfoPrototype";
            mtx.ProtoDataTable = "mtxStorefrontData";
            var unknownId = obj.ValueOrDefault<long>("4611686297592334024", 0); //Always 3042172580397056 for collection items
            mtx.UnknowntextId = unknownId;
            mtx.Unknowntext = _dom.stringTable.TryGetString("str.gui.mtxstorefrontitems", unknownId); // need to find the right stringtable for this.
            mtx.Localizedunknowntext = _dom.stringTable.TryGetLocalizedStrings("str.gui.mtxstorefrontitems", unknownId);

            var rarityId = obj.ValueOrDefault<long>("mtxRarityDescriptionId", 0);
            mtx.RarityDescId = rarityId;
            mtx.RarityDesc = _dom.stringTable.TryGetString("str.gui.mtxstorefrontitems", rarityId);
            mtx.LocalizedRarityDesc = _dom.stringTable.TryGetLocalizedStrings("str.gui.mtxstorefrontitems", rarityId);

            var bulletPointIds = obj.ValueOrDefault("mtxBulletPointDescriptionIds", new List<object>()).ConvertAll(x => (long)x);
            mtx.BulletPoints = new List<string>();
            mtx.LocalizedBulletPoints = new List<Dictionary<string, string>>();
            foreach (var bullet in bulletPointIds)
            {
                mtx.BulletPoints.Add(_dom.stringTable.TryGetString("str.gui.mtxstorefrontitems", bullet));
                mtx.LocalizedBulletPoints.Add(_dom.stringTable.TryGetLocalizedStrings("str.gui.mtxstorefrontitems", bullet));
            }

            var nameId = obj.ValueOrDefault<long>("mtxName", 0);
            mtx.Name = _dom.stringTable.TryGetString("str.gui.mtxstorefrontitems", nameId);
            mtx.LocalizedName = _dom.stringTable.TryGetLocalizedStrings("str.gui.mtxstorefrontitems", nameId);

            mtx.Id = Id;
            mtx.Icon = obj.ValueOrDefault("mtxStorefrontIcon", ""); // "Mtx.Season3.Bikini_V02"
            _dom._assets.icons.AddMtx(mtx.Icon);

            mtx.UnknownNumber = obj.ValueOrDefault<long>("4611686296598030002", 0);
            mtx.Categories = obj.ValueOrDefault("mtxCategories", new Dictionary<object, object>());

            mtx.DiscountCost = obj.ValueOrDefault<long>("mtxDiscountPrice", 0);
            mtx.FullPriceCost = obj.ValueOrDefault<long>("mtxFullPrice", 0);

            //mtx.CategoryId = (long)obj.ValueOrDefault("mtxMainCategory", new object()); // 610 looked up in colCollectionItemsPrototype("colCollectionItemsCategoryData")

            mtx.ItemIdsList = obj.ValueOrDefault("mtxCollection", new List<object>()).ConvertAll(x => (ulong)x);
            /*{ 16141048636041134811, 16140999226559259282, 16141134542521957469, 
            * 16140928499777528367, 16141006708961340344, 16141053294373613055, 
            * 16140959691716914276,  } - items*/
            /*mtx.ItemList = new List<Item>();
            foreach (var item in mtx.ItemIdsList)
            {
                mtx.ItemList.Add(_dom.itemLoader.Load(item));
            }*/

            var retiredItemsLookupDictionary = obj.ValueOrDefault("4611686348190657002", new Dictionary<object, object>());

            mtx.IsAccountUnlock = obj.ValueOrDefault("mtxIsAccountUnlock", false); // if they have it, it's true
            mtx.UnknownBool2 = obj.ValueOrDefault("4611686297975974006", false); // if they have it, it's true

            mtx.LinkedMTXEntryId = obj.ValueOrDefault<long>("mtxLinkedId", 0);

            return mtx;
        }
    }
}
