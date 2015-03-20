using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.Tables
{
    /// <summary>
    /// GomLib.Tables.ArmorPerLevel.TableData[WeaponSpec][Models.ItemQuality][ItemLevel][Stat]<br/>
    /// Possible Stats: MaxWeaponDamage, MinWeaponDamage, ForcePowerRating, TechPowerRating
    /// </summary>
    
    public class WeaponPerLevel : IDisposable
    {
        private Dictionary<ulong, Dictionary<int, Dictionary<int, Dictionary<int, float>>>> table_data;
        string tablePath = "cbtWeaponPerLevelPrototype";
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
                Console.WriteLine("WeaponPerLevel Disposing");
                foreach (var dict1 in TableData)
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

        ~WeaponPerLevel()
        {
            Dispose(false);
        }

        /*
         Dictionary<int, Dictionary
         *  <int, Dictionary
         *      <int, Dictionary
         *          <int, float>>>>
         * * 
         * 
         */

        public Dictionary<ulong, Dictionary<int, Dictionary<int, Dictionary<int, float>>>> TableData
        {
            get
            {
                if (table_data == null) { LoadData(); }
                return table_data;
            }
        }

        public float GetStat(Item i, Stat stat) { return GetStat(i.WeaponSpec.Id, i.ItemLevel, i.Quality, stat); }
        public float GetStat(ulong spec, int level, ItemQuality quality, Stat stat)
        {
            if (level <= 0) { return 0; }

            if (table_data == null) { LoadData(); }

            return table_data[spec][(int)quality][level][(int)stat];
        }

        private DataObjectModel _dom;

        public WeaponPerLevel(DataObjectModel dom)
        {
            _dom = dom;
            LoadData();
        }

        private void LoadData()
        {
            GomObject table = _dom.GetObject(tablePath);
            Dictionary<object, object> tableData = table.Data.Get<Dictionary<object,object>>("cbtWeaponPerLevelData");

            table_data = new Dictionary<ulong, Dictionary<int, Dictionary<int, Dictionary<int, float>>>>();
            foreach (var kvp in tableData)
            {
                //WeaponSpec wpnSpec = WeaponSpec.Load(_dom, (ulong)kvp.Key);
                Dictionary<object, object> qualityToLevelMap = (Dictionary<object, object>)kvp.Value;

                var container0 = new Dictionary<int, Dictionary<int, Dictionary<int, float>>>();
                table_data[(ulong)kvp.Key] = container0;

                foreach (var quality_level in qualityToLevelMap)
                {
                    ItemQuality quality = ItemQualityExtensions.ToItemQuality((ScriptEnum)quality_level.Key);
                    var levelToStatMap = (Dictionary<object, object>)quality_level.Value;

                    var container1 = new Dictionary<int, Dictionary<int, float>>();
                    container0[(int)quality] = container1;

                    foreach (var level_stat in levelToStatMap)
                    {
                        int level = (int)(long)level_stat.Key;
                        var statToValueMap = (Dictionary<object, object>)level_stat.Value;

                        Dictionary<int, float> container2 = new Dictionary<int, float>();
                        container1[level] = container2;

                        foreach (var stat_val in statToValueMap)
                        {
                            Stat stat = StatExtensions.ToStat((ScriptEnum)stat_val.Key);
                            float val = (float)stat_val.Value;
                            container2[(int)stat] = val;
                        }
                    }
                }
            }
        }
    }
}
