using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.Tables
{
    /// <summary>
    /// GomLib.Tables.ItemRating.TableData[ItemLevel][Models.ItemQuality]
    /// </summary>
    public class ItemRating
    {
        private readonly DataObjectModel _dom;

        public ItemRating(DataObjectModel dom)
        {
            _dom = dom;
            LoadData();
        }

        private Dictionary<int, Dictionary<Models.ItemQuality, int>> item_rating_data;
        readonly string itmRatingsTablePath = "itmRatingTablePrototype";

        public Dictionary<int, Dictionary<Models.ItemQuality, int>> TableData
        {
            get
            {
                if (item_rating_data == null) { LoadData(); }
                return item_rating_data;
            }
        }

        public int GetRating(Item i) { return GetRating(i.ItemLevel, i.Quality); }
        public int GetRating(int level, Models.ItemQuality quality)
        {
            if (level <= 0) { return 0; }
            else if (level > item_rating_data.Count - 1) { return GetRating(item_rating_data.Count - 1, quality); }
            if (item_rating_data == null) { LoadData(); }

            return item_rating_data[level][quality];
        }

        private void LoadData()
        {
            GomObject table = _dom.GetObject(itmRatingsTablePath);
            Dictionary<object, object> tableData = table.Data.Get<Dictionary<object, object>>("itmRatings");
            item_rating_data = new Dictionary<int, Dictionary<Models.ItemQuality, int>>();
            foreach (var kvp in tableData)
            {
                int lvl = (int)(long)kvp.Key;
                var ratingData = (Dictionary<object, object>)kvp.Value;
                var qMap = new Dictionary<Models.ItemQuality, int>();
                foreach (var qr in ratingData)
                {
                    var quality = Models.ItemQualityExtensions.ToItemQuality((ScriptEnum)qr.Key);
                    var rating = (int)(long)qr.Value;
                    qMap.Add(quality, rating);
                }
                item_rating_data.Add(lvl, qMap);
            }

            //var rows = Utilities.ReadDataTable(itmRatingsTablePath, (row) =>
            //{
            //    return new
            //    {
            //        Level = row.Element("level").AsInt(),
            //        Cheap = row.Element("itmQualityCheap").AsInt(),
            //        Standard = row.Element("itmQualityStandard").AsInt(),
            //        Premium = row.Element("itmQualityPremium").AsInt(),
            //        Prototype = row.Element("itmQualityPrototype").AsInt(),
            //        Artifact = row.Element("itmQualityArtifact").AsInt(),
            //        Legendary = row.Element("itmQualityLegendary").AsInt(),
            //        Legacy = row.Element("itmQualityLegacy").AsInt()
            //    };
            //});
            //item_rating_data = new Dictionary<int, Dictionary<Models.ItemQuality, int>>();
            //foreach (var row in rows)
            //{
            //    var qMap = new Dictionary<ItemQuality, int>();
            //    qMap.Add(Models.ItemQuality.Cheap, row.Cheap);
            //    qMap.Add(Models.ItemQuality.Standard, row.Standard);
            //    qMap.Add(Models.ItemQuality.Premium, row.Premium);
            //    qMap.Add(Models.ItemQuality.Prototype, row.Prototype);
            //    qMap.Add(Models.ItemQuality.Artifact, row.Artifact);
            //    qMap.Add(Models.ItemQuality.Legendary, row.Legendary);
            //    qMap.Add(Models.ItemQuality.Legacy, row.Legacy);
            //    qMap.Add(Models.ItemQuality.Quest, row.Quest);
            //    item_rating_data.Add(row.Level, qMap);
            //}
        }
    }
}
