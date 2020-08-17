using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class SCFFColorOptionLoader
    {
        readonly DataObjectModel _dom;
        public SCFFColorOptionLoader(DataObjectModel dom)
        {
            _dom = dom;
        }

        public Models.ScFFColorOption Load(Models.ScFFColorOption col, GomObjectData obj)
        {
            if (obj == null) { return col; }
            if (col == null) { return null; }

            col.NameId = obj.ValueOrDefault<ulong>("scFFColorName", 0);
            col.Name = _dom.stringTable.TryGetString("str.gui.colornames", Convert.ToInt64(col.NameId));
            col.ColorCode = obj.ValueOrDefault<string>("scFFColorCodeId", null);
            col.ShipId = obj.ValueOrDefault<long>("scFFColorShipId", 0);
            if (obj.ContainsKey("scFFColorType"))
            {
                ScriptEnum type = obj.Get<ScriptEnum>("scFFColorType");
                col.Type = type.ToString();
            }
            else
            {
                col.Type = "scFFColorOptionBlaster";
            }

            ScriptEnum availability = obj.Get<ScriptEnum>("scFFColorIsAvailable");
            CheckAvailability(col, availability);
            col.ShortId = obj.ValueOrDefault<long>("scFFColorShortId", 0);
            col.Icon = obj.ValueOrDefault<string>("scFFComponentColorIcon", null);
            _dom._assets.icons.Add(col.Icon);

            return col;
        }

        private void CheckAvailability(Models.ScFFColorOption col, ScriptEnum availability)
        {
            bool available = false;
            bool deprecated = false;
            switch (availability.ToString())
            {
                case "scFFDeprecated":
                    deprecated = true;
                    break;
                case "scFFAvailable": //default is scFFUnavailable
                    available = true;
                    break;
            }
            col.IsAvailable = available;
            col.IsDeprecated = deprecated;
        }
    }
}
