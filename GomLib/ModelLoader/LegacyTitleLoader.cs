using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class LegacyTitleLoader
    {
        Dictionary<long, LegacyTitle> idLookup;
        readonly DataObjectModel _dom;

        public LegacyTitleLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            idLookup = new Dictionary<long, LegacyTitle>();
        }

        public LegacyTitle Load(long id)
        {
            if (idLookup.Count == 0)
            {
                idLookup = _dom.GetObject("lgcLegacyTitlesTablePrototype").Data.Get<Dictionary<object, object>>("lgcLegacyTitlesData").ToDictionary(x => Convert.ToInt64(x.Key), x => Load(new LegacyTitle(), (GomObjectData)x.Value));
            }
            idLookup.TryGetValue(id, out LegacyTitle data);

            return data;
        }

        public LegacyTitle Load(LegacyTitle title, GomObjectData obj)
        {
            if (obj == null) { return title; }
            if (title == null) { return null; }

            title.LegacyTitleId = obj.ValueOrDefault<long>("lgcLegacyTitleId", 0);
            title.LegacyTitleStringId = obj.ValueOrDefault<long>("lgcLegacyTitleString", 0);
            title.LegacyTitleString = _dom.stringTable.TryGetString("str.pc.legacytitle", title.LegacyTitleStringId);

            return title;
        }
    }
}
