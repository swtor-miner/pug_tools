using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class PlayerTitleLoader
    {
        List<object> idLookup;
        readonly DataObjectModel _dom;

        public PlayerTitleLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            idLookup = new List<object>();
        }

        public PlayerTitle Load(PlayerTitle title, GomObjectData obj)
        {
            if (obj == null) { return title; }
            if (title == null) { return null; }

            if (idLookup.Count == 0)
            {
                idLookup = _dom.GetObject("chrPlayerTitlesTablePrototype").Data.Get<List<object>>("chrPlayerTitlesMapping");
            }
            return Load(title, idLookup.FindIndex(x => x == obj), obj);
        }
        public PlayerTitle Load(PlayerTitle title, long id, GomObjectData obj)
        {
            if (obj == null) { return title; }
            if (title == null) { return null; }

            title.TitleId = id;
            title.TitleStringId = obj.ValueOrDefault<long>("titleDetailStringID", 0);
            title.TitleString = _dom.stringTable.TryGetString("str.pc.title", title.TitleStringId);
            title.LocalizedTitleString = _dom.stringTable.TryGetLocalizedStrings("str.pc.title", title.TitleStringId);
            title.TitleCodexNode = obj.ValueOrDefault<ulong>("titleCodex", 0);
            title.TitleLegacyPrefix = obj.ValueOrDefault("titleDetailLegacyPrefix", false);

            return title;
        }
    }
}
