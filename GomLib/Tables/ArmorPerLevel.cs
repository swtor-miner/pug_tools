using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.Tables
{
    /// <summary>
    /// GomLib.Tables.ArmorPerLevel.TableData[ArmorSpec][Models.ItemQuality][ItemLevel][ArmorSlot]
    /// </summary>
    public class ArmorPerLevel
    {
        private readonly DataObjectModel _dom;

        public ArmorPerLevel(DataObjectModel dom)
        {
            _dom = dom;
            LoadData();
        }

        class ArmorRow
        {
            public ArmorSpec Spec { get; set; }
            public ItemQuality Quality { get; set; }
            public int Level { get; set; }
            public Dictionary<SlotType, int> SlotToRating { get; set; }
        }

        private Dictionary<long, Dictionary<int, Dictionary<int, Dictionary<int, int>>>> table_data;

        // string tablePath = "/resources/server/tbl/cbtarmorperleveltable.tbl";
        readonly string tablePath = "cbtArmorPerLevel";

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                foreach (var dict1 in table_data)
                {
                    foreach (var dict2 in dict1.Value)
                    {
                        foreach (var dict3 in dict2.Value)
                        {
                            dict3.Value.Clear();
                        }
                        dict2.Value.Clear();
                    }
                    dict1.Value.Clear();
                }
                table_data.Clear();
                table_data = null;
            }
            disposed = true;
        }

        ~ArmorPerLevel()
        {
            Dispose(false);
        }


        public Dictionary<long, Dictionary<int, Dictionary<int, Dictionary<int, int>>>> TableData
        {
            get
            {
                if (table_data == null) { LoadData(); }
                return table_data;
            }
        }

        public int GetArmor(Item i) { return GetArmor(i.ArmorSpec, i.ItemLevel, i.Quality, i.Slots.Count > 0 ? i.Slots[0] : SlotType.Invalid); }
        public int GetArmor(ArmorSpec spec, int level, ItemQuality quality, SlotType slot)
        {
            if (level <= 0) { return 0; }
            if (spec == null) { return -1; }
            if (table_data == null) { LoadData(); }

            if (table_data.ContainsKey(spec.Id))
                return table_data[spec.Id][(int)quality][level][(int)slot];
            else
                return -1;
        }

        private void LoadData()
        {
            GomObject table = _dom.GetObject(tablePath);
            Dictionary<object, object> tableData = table.Data.ValueOrDefault<Dictionary<object, object>>("cbtArmorValues", null);

            // var rows = Utilities.ReadDataTable(tablePath, ReadArmorRow);
            table_data = new Dictionary<long, Dictionary<int, Dictionary<int, Dictionary<int, int>>>>();
            foreach (var kvp in tableData)
            {
                //ArmorSpec armorSpec = ArmorSpec.Load(_dom, (long)kvp.Key);
                Dictionary<object, object> qualityToLevelMap = (Dictionary<object, object>)kvp.Value;

                Dictionary<int, Dictionary<int, Dictionary<int, int>>> container0 = new Dictionary<int, Dictionary<int, Dictionary<int, int>>>();
                table_data[(long)kvp.Key] = container0;

                foreach (var quality_level in qualityToLevelMap)
                {
                    ItemQuality quality = ItemQualityExtensions.ToItemQuality((ScriptEnum)quality_level.Key);
                    var levelToSlotMap = (Dictionary<object, object>)quality_level.Value;

                    Dictionary<int, Dictionary<int, int>> container1 = new Dictionary<int, Dictionary<int, int>>();
                    container0[(int)quality] = container1;

                    foreach (var level_slot in levelToSlotMap)
                    {
                        int level = (int)(long)level_slot.Key;
                        var slotToArmorMap = (Dictionary<object, object>)level_slot.Value;

                        Dictionary<int, int> container2 = new Dictionary<int, int>();
                        container1[level] = container2;

                        foreach (var slot_armor in slotToArmorMap)
                        {
                            SlotType slot = SlotTypeExtensions.ToSlotType((ScriptEnum)slot_armor.Key);
                            int armor = (int)(long)slot_armor.Value;
                            container2[(int)slot] = armor;
                        }
                    }
                }
            }
        }
    }
}
