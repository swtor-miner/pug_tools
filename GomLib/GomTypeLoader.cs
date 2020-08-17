using System;
using System.Collections.Generic;

namespace GomLib
{
    public class GomTypeLoader
    {
        private readonly Dictionary<GomTypeId, GomTypeLoaders.IGomTypeLoader> gomTypeLoaderMap;

        private void AddLoader(GomTypeLoaders.IGomTypeLoader loader)
        {
            gomTypeLoaderMap.Add(loader.SupportedType, loader);
        }

        private DataObjectModel _dom;

        public DataObjectModel Dom { get => _dom; set => _dom = value; }

        public GomTypeLoader(DataObjectModel dom)
        {
            Dom = dom;

            gomTypeLoaderMap = new Dictionary<GomTypeId, GomTypeLoaders.IGomTypeLoader>();
            AddLoader(new GomTypeLoaders.UInt64Loader());
            AddLoader(new GomTypeLoaders.IntegerLoader());
            AddLoader(new GomTypeLoaders.BooleanLoader());
            AddLoader(new GomTypeLoaders.FloatLoader());
            AddLoader(new GomTypeLoaders.EnumLoader());
            AddLoader(new GomTypeLoaders.StringLoader());
            AddLoader(new GomTypeLoaders.ListLoader(dom));
            AddLoader(new GomTypeLoaders.MapLoader(dom));
            AddLoader(new GomTypeLoaders.EmbeddedClassLoader());
            // Array
            // Table
            // Cubic
            AddLoader(new GomTypeLoaders.ScriptLoader());
            AddLoader(new GomTypeLoaders.ClassRefLoader());
            AddLoader(new GomTypeLoaders.TimerLoader());
            AddLoader(new GomTypeLoaders.VectorLoader());
            AddLoader(new GomTypeLoaders.TimeSpanLoader());
            AddLoader(new GomTypeLoaders.TimeLoader());
        }

        public void Flush()
        {
            Dom = null;
        }

        public GomType Load(GomBinaryReader reader, DataObjectModel dom, bool fromGom = true)
        {
            GomTypeId typeId = (GomTypeId)reader.ReadByte();
            //if (typeId == (GomTypeId.None)) typeId = GomTypeId.UInt64;
            if (!gomTypeLoaderMap.TryGetValue(typeId, out GomTypeLoaders.IGomTypeLoader gomTypeLoader))
            {
                //throw new InvalidOperationException(String.Format("Unknown GomType with Type ID {0}", (byte)typeId));
                return null;
            }

            return gomTypeLoader.Load(reader, fromGom, dom);
        }
    }
}
