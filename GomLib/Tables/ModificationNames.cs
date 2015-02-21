using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.Tables
{
    public class ModificationNames
    {
        private DataObjectModel _dom;

        public ModificationNames(DataObjectModel dom)
        {
            _dom = dom;
        }

        const long nameOffset = 0x647ac00000000;
        public string GetName(long id)
        {
            StringTable st = _dom.stringTable.Find("str.itm.modifiers");
            return st.GetText(nameOffset + id, "whatever");
        }
    }
}
