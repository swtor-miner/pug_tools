using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class SCFFPatternLoader
    {
        Dictionary<object, object> idLookup;
        readonly DataObjectModel _dom;

        public SCFFPatternLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            idLookup = new Dictionary<object, object>();
        }

        public ScFFPattern Load(ScFFPattern pat, GomObjectData obj)
        {
            if (obj == null) { return pat; }
            if (pat == null) { return null; }

            if (idLookup.Count == 0)
            {
                idLookup = _dom.GetObject("scFFPatternsDefinitionProtoype").Data.Get<Dictionary<object, object>>("scFFPatternsDefinitionData");
            }
            pat.NameId = obj.ValueOrDefault<long>("scFFPatternName", 0);
            pat.Name = _dom.stringTable.TryGetString("str.spvp.misc", pat.NameId);
            pat.VeryShortId = obj.ValueOrDefault<long>("scFFPatternVeryShortId", 0);
            pat.ShipId = obj.ValueOrDefault<long>("scFFPatternShipId", 0);
            pat.Id = (long)idLookup[pat.VeryShortId];
            pat.TexturesByShipId = new Dictionary<long, string>();

            if (pat.ShipId != 0)
            {
                var patTextureLookup = _dom.GetObject("scFFPatternsTextureDataProtoype").Data.Get<Dictionary<object, object>>("scFFPatternTexture");
                if (patTextureLookup.ContainsKey(pat.Id))
                {
                    Dictionary<object, object> patList = (Dictionary<object, object>)patTextureLookup[pat.Id];
                    foreach (var pattern in patList)
                    {
                        pat.TexturesByShipId.Add((long)pattern.Key, pattern.Value.ToString());
                    }
                }
            }


            ScriptEnum availability = obj.Get<ScriptEnum>("scFFPatternIsAvailable");
            CheckAvailability(pat, availability);
            pat.ShortId = obj.ValueOrDefault<long>("scFFPattternShortId", 0);
            pat.Icon = obj.ValueOrDefault<string>("scFFPatternIcon", null);
            _dom._assets.icons.Add(pat.Icon);

            return pat;
        }

        private void CheckAvailability(ScFFPattern pat, ScriptEnum availability)
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
            pat.IsAvailable = available;
            pat.IsDeprecated = deprecated;
        }
    }
}
