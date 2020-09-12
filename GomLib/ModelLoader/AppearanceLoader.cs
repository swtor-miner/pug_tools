using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;
using System.Drawing;

namespace GomLib.ModelLoader
{
    public class AppearanceLoader
    {
        private readonly DataObjectModel _dom;
        public AppearanceLoader(DataObjectModel dom)
        {
            _dom = dom;
        }

        public GameObject Load(ulong nodeId)
        {
            var obj = _dom.GetObject(nodeId);
            return Load(obj);
        }

        public GameObject Load(string fqn)
        {
            var obj = _dom.GetObject(fqn);
            return Load(obj);
        }

        public GameObject Load(GameObject obj, GomObject gom)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (gom == null) { return null; }

            obj = Load(gom);

            return obj;
        }

        public GameObject Load(GomObject obj)
        {
            if (obj == null) { return null; }

            switch (obj.Name.Substring(0, 3))
            {
                case "ipp":
                    return LoadIpp(obj);
                case "npp":
                    return LoadNpp(obj);
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public NpcAppearance LoadNpp(GomObject obj)
        {
            NpcAppearance pkg = new NpcAppearance(_dom)
            {
                Fqn = obj.Name,
                //Console.WriteLine(obj.Name);
                Id = obj.Id,
                Dom_ = _dom,
                References = obj.References,
                BodyType = obj.Data.ValueOrDefault<string>("nppBodyType")
            };

            var slotMap = obj.Data.ValueOrDefault<Dictionary<object, object>>("nppAppearanceSlotMap_ForPrototype", null);

            pkg.AppearanceSlotMap = new Dictionary<string, List<AppSlot>>();

            if (slotMap != null)
            {
                foreach (var kvp in slotMap)
                {
                    var key = ((ScriptEnum)kvp.Key).ToString();
                    List<AppSlot> appList = new List<AppSlot>();
                    for (int i = 0; i < ((List<object>)kvp.Value).Count; i++)
                    {
                        //if (((List<object>)kvp.Value).Count > 1)
                        //throw new IndexOutOfRangeException();
                        var value = LoadAppSlot((GomObjectData)((List<object>)kvp.Value)[0], pkg.BodyType);
                        appList.Add(value);
                    }

                    pkg.AppearanceSlotMap.Add(key, appList);
                }
            }

            pkg.NppType = ((ScriptEnum)obj.Data.ValueOrDefault<object>("nppNppType") ?? new ScriptEnum()).ToString();
            pkg.SoundPackage = obj.Data.ValueOrDefault("nppSoundPackage", "");
            pkg.ArmorSoundsetOverride = obj.Data.ValueOrDefault("nppArmorSoundsetOverride", "");

            var vocalOverrides = obj.Data.ValueOrDefault("nppVocalSoundsetOverride", new Dictionary<object, object>());
            pkg.VocalSoundsetOverride = new Dictionary<long, string>();
            foreach (var kvp in vocalOverrides)
            {
                pkg.VocalSoundsetOverride.Add((long)kvp.Key, (string)kvp.Value);
            }

            return pkg;
        }

        public ItemAppearance LoadIpp(GomObject obj)
        {

            ItemAppearance pkg = new ItemAppearance(_dom)
            {
                Fqn = obj.Name,
                Id = obj.Id,
                Dom_ = _dom,
                References = obj.References,
                ColorScheme = obj.Data.ValueOrDefault<long>("ippColorScheme", 0),
                VOSoundTypeOverride = obj.Data.ValueOrDefault("ippVOSoundTypeOverride", ""),
                IPP = LoadAppSlot(obj.Data, "")
            };

            return pkg;
        }

        public AppSlot LoadAppSlot(GomObjectData obj, string btOverride)
        {
            if (obj == null)
                return new AppSlot(_dom);

            AppSlot app = new AppSlot(_dom)
            {
                Dom_ = _dom,

                BodyType = btOverride
            };
            var typ = ((ScriptEnum)obj.ValueOrDefault<object>("appAppearanceSlotType", null));
            if (typ == null)
                app.Type = "appSlotAge";
            else
                app.Type = typ.ToString();
            app.ModelID = obj.ValueOrDefault<long>("appAppearanceSlotModelID", 0);
            app.MaterialIndex = obj.ValueOrDefault<long>("appAppearanceSlotMaterialIndex", 0);
            app.Attachments = obj.ValueOrDefault("appAppearanceSlotAttachments", new List<object>()).ConvertAll(x => (long)x);
            app.RandomWeight = obj.Get<long>("appAppearanceSlotRandomWeight");

            app.PrimaryHueId = obj.ValueOrDefault<long>("appAppearanceSlotHuePrimary", 0);
            app.SecondaryHueId = obj.ValueOrDefault<long>("appAppearanceSlotHueSecondary", 0);

            return app;
        }

        internal Dictionary<string, WeaponAppearance> itmAppearanceDatatable;

        public WeaponAppearance LoadWeaponAppearance(string name)
        {
            if (itmAppearanceDatatable == null)
            {
                var dataTable = _dom.GetObject("itmAppearanceDatatable");
                var tempDict = dataTable.Data.Get<Dictionary<object, object>>("itmAppearances");
                dataTable.Unload();
                itmAppearanceDatatable = new Dictionary<string, WeaponAppearance>();
                foreach (var kvp in tempDict)
                {
                    itmAppearanceDatatable.Add((string)kvp.Key, LoadWeaponAppearance((string)kvp.Key, (GomObjectData)kvp.Value));
                }
            }

            itmAppearanceDatatable.TryGetValue(name, out WeaponAppearance output);
            return output;
        }

        public WeaponAppearance LoadWeaponAppearance(string name, GomObjectData obj)
        {
            WeaponAppearance pkg = new WeaponAppearance(_dom)
            {
                Prototype = "itmAppearanceDatatable",
                ProtoDataTable = "itmAppearances",
                Name = name,

                BoneName = obj.ValueOrDefault<string>("itmBoneName", null),
                CombatStance = obj.ValueOrDefault<string>("itmCombatStance", null),
                DrawnOffset = obj.ValueOrDefault<List<float>>("itmDrawnOffset", null),
                DrawnRotation = obj.ValueOrDefault<List<float>>("itmDrawnRotation", null),
                DrawnScale = obj.ValueOrDefault<List<float>>("itmDrawnScale", null),
                DynamicData = obj.ValueOrDefault<string>("itmDynamicData", null),
                FxSpec = obj.ValueOrDefault<string>("itmFxSpec", null),
                Model = obj.ValueOrDefault<string>("itmModel", null),
                StowedOffset = obj.ValueOrDefault<List<float>>("itmStowedOffset", null),
                StowedRotation = obj.ValueOrDefault<List<float>>("itmStowedRotation", null),
                StowedScale = obj.ValueOrDefault<List<float>>("itmStowedScale", null),
                WeaponType = obj.ValueOrDefault("itmWeaponType", new ScriptEnum()).ToString()
            };

            return pkg;
        }

        public void Flush()
        {
            itmAppearanceDatatable = null;
        }
    }
    public class DetailedAppearanceColorLoader
    {
        public DataObjectModel _dom;
        private Dictionary<long, DetailedAppearanceColor> idMap;

        public DetailedAppearanceColorLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }
        public void Flush()
        {
            idMap = new Dictionary<long, DetailedAppearanceColor>();
        }

        public DetailedAppearanceColor Load(long id)
        {
            if (idMap.Count == 0)
            {
                Initialize();
            }
            idMap.TryGetValue(id, out DetailedAppearanceColor ret);
            return ret;
        }

        private void Initialize()
        {
            var itmAppearanceColorsPrototype = _dom.GetObject("itmAppearanceColorsPrototype");
            var itmAppColorTable = itmAppearanceColorsPrototype.Data.ValueOrDefault("itmAppColorTable", new List<object>());
            var itmAppColorIdLookup = itmAppearanceColorsPrototype.Data.ValueOrDefault("itmAppColorIdLookup", new Dictionary<object, object>());
            itmAppearanceColorsPrototype.Unload();
            StringTable stringTable = _dom.stringTable.Find("str.gui.colornames");

            foreach (GomObjectData gom in itmAppColorTable.ConvertAll(x => (GomObjectData)x))
            {
                DetailedAppearanceColor det = new DetailedAppearanceColor
                {
                    ColorId = gom.ValueOrDefault<long>("itmAppColorId", 0)
                };
                if (itmAppColorIdLookup.ContainsKey(det.ColorId))
                {
                    det.ShortId = (long)itmAppColorIdLookup[det.ColorId];
                }
                det.ColorNameId = gom.ValueOrDefault<long>("itmAppColorName", 0);
                det.ColorName = stringTable.GetText(det.ColorNameId, "str.gui.colornames");
                det.LocalizedColorName = stringTable.GetLocalizedText(det.ColorNameId, "str.gui.colornames");
                det.ColorSchemeId = gom.ValueOrDefault<long>("itmAppColorSchemeId", 0);
                det.HueName = gom.ValueOrDefault("itmAppColorHueName", "");
                det.UnknownBool1 = gom.ValueOrDefault("4611686298195974006", false);
                det.UnknownBool2 = gom.ValueOrDefault("4611686298195974007", false);

                GomObjectData pal1 = (GomObjectData)gom.ValueOrDefault<object>("itmAppColorPalette1Rep", null);
                if (pal1 != null)
                {
                    byte a = Convert.ToByte((255f * pal1.ValueOrDefault("a", 0f)));
                    byte r = Convert.ToByte((255f * pal1.ValueOrDefault("r", 0f)));
                    byte g = Convert.ToByte((255f * pal1.ValueOrDefault("g", 0f)));
                    byte b = Convert.ToByte((255f * pal1.ValueOrDefault("b", 0f)));
                    det.Palette1Rep = Color.FromArgb(a, r, g, b);
                }
                GomObjectData pal2 = (GomObjectData)gom.ValueOrDefault<object>("itmAppColorPalette2Rep", null);
                if (pal2 != null)
                {
                    byte a = Convert.ToByte((255f * pal2.ValueOrDefault("a", 0f)));
                    byte r = Convert.ToByte((255f * pal2.ValueOrDefault("r", 0f)));
                    byte g = Convert.ToByte((255f * pal2.ValueOrDefault("g", 0f)));
                    byte b = Convert.ToByte((255f * pal2.ValueOrDefault("b", 0f)));
                    det.Palette2Rep = Color.FromArgb(a, r, g, b);
                }
                idMap.Add(det.ShortId, det);
            }
        }
    }
}
