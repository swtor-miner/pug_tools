using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.Models
{
    public class Vendor
    {
        public static Dictionary<string, Vendor> fqnMap;
        public static Dictionary<string, string> failedFqns;

        static Vendor()
        {
            fqnMap = new Dictionary<string, Vendor>();
            failedFqns = new Dictionary<string, string>();
        }

        public string Spec { get; set; }

        //public static Vendor FromFqn(string fqn)
        //{
        //    Vendor vendor;
        //    if (failedFqns.ContainsKey(fqn)) { return null; }
        //    if (!fqnMap.TryGetValue(fqn, out vendor))
        //    {
        //        var rows = TorLib.Tables.VendorTable.FindRowsBySpec(fqn);
        //        if ((rows == null) || (rows.Count == 0))
        //        {
        //            failedFqns.Add(fqn, "Row not found");
        //            return null;
        //        }
        //        try
        //        {
        //            vendor = FromRows(rows);
        //        }
        //        catch (Exception ex)
        //        {
        //            failedFqns.Add(fqn, ex.Message);
        //            return null;
        //        }
        //    }

        //    return vendor;
        //}

        //public static Vendor FromRows(List<TorLib.Tables.VendorTable.Row> rows)
        //{
        //    if (rows.Count == 0) { return null; }
        //    if (fqnMap.ContainsKey(rows[0].spec)) { return fqnMap[rows[0].spec]; }

        //    Vendor result = new Vendor();
        //    result.Spec = rows[0].spec;

        //    fqnMap.Add(rows[0].spec, result);
        //    return result;
        //}
    }
}
