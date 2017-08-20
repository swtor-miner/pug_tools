using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class HuntingRadius
    {
        private DataObjectModel _dom;

        public HuntingRadius(DataObjectModel dom)
        {
            _dom = dom;
            LoadData();
        }

        private Dictionary<long, long> table_data;
        string tablePath = "qstHuntingRadiusProto";
        private bool disposed = false;

        private void LoadData()
        {
            GomObject table = _dom.GetObject(tablePath);
            if (table == null) return;
            Dictionary<object, object> tableData = table.Data.ValueOrDefault<Dictionary<object, object>>("qstHuntingRadiusNameMap", new Dictionary<object, object>());

            table_data = tableData.ToDictionary(x => (long)x.Key, x => (long)x.Value);
        }

        public long GetRadius(long i)
        {
            long ret;
            table_data.TryGetValue(i, out ret);
            return ret;
        }

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
                if (table_data != null) table_data.Clear();
                table_data = null;
            }
            disposed = true;
        }


        ~HuntingRadius()
        {
            Dispose(false);
        }
    }
    //public enum HuntingRadius
    //{
    //    None,
    //    Small,
    //    Medium,
    //    Large,
    //    ExtraLarge,
    //    SuperSize,
    //    Husky,
    //    BigBoned
    //}

    //public static class HuntingRadiusExtensions
    //{
    //    public static HuntingRadius ToHuntingRadius(this string str)
    //    {
    //        if (String.IsNullOrEmpty(str)) { return HuntingRadius.None; }

    //        switch (str)
    //        {
    //            case "None": return HuntingRadius.None;
    //            case "Small": return HuntingRadius.Small;
    //            case "Medium": return HuntingRadius.Medium;
    //            case "Large": return HuntingRadius.Large;
    //            case "Extra Large": return HuntingRadius.ExtraLarge;
    //            case "Super Size": return HuntingRadius.SuperSize;
    //            case "Husky": return HuntingRadius.Husky;
    //            case "Big Boned": return HuntingRadius.BigBoned;
    //            default: throw new InvalidOperationException("Unknown HuntingRadius: " + str);
    //        }
    //    }

    //    public static HuntingRadius ToHuntingRadius(long val)
    //    {
    //        switch (val)
    //        {
    //            case 5836340512931272827: return HuntingRadius.None;        // 0
    //            case 2959999436154067372: return HuntingRadius.Small;       // 25
    //            case 589686270506543030: return HuntingRadius.Medium;       // 40
    //            case -2493745135610518796: return HuntingRadius.Large;      // 60
    //            case -9157026497624578618: return HuntingRadius.ExtraLarge; // 100
    //            case 6196483884609695543: return HuntingRadius.SuperSize;   // 150
    //            case 24054240851265359: return HuntingRadius.Husky;         // 200
    //            case -3288063830629591179: return HuntingRadius.BigBoned;   // 250
    //            default: throw new InvalidOperationException("Unknown HuntingRadius: " + val.ToString());
    //        }
    //    }
    //}
}
