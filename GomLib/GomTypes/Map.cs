using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypes
{
    public class Map : GomType
    {
        public Map() : base(GomTypeId.Map) { }

        public GomType KeyType { get; internal set; }
        public GomType ValueType { get; internal set; }

        internal override void Link(DataObjectModel dom)
        {
            _dom = dom;
            KeyType.Link(dom);
            ValueType.Link(dom);
        }

        public override string ToString()
        {
            return string.Format("Map<{0}, {1}>", KeyType, ValueType);
        }

        public override bool ConfirmType(GomBinaryReader reader)
        {
            //return base.ConfirmType(reader) && KeyType.ConfirmType(reader) && ValueType.ConfirmType(reader);
            return base.ConfirmType(reader);
        }

        public override object ReadData(DataObjectModel dom, GomBinaryReader reader)
        {
            GomType keyType = dom.gomTypeLoader.Load(reader, dom, false);
            if (KeyType != null)
            {
                if (keyType == null)
                {
                    //if (keyType.TypeId == KeyType.TypeId)
                    //{
                    keyType = KeyType;
                    //}
                }
                else
                {
                    if (keyType.TypeId == KeyType.TypeId)
                    {
                        keyType = KeyType;
                    }
                }
            }

            GomType valType = dom.gomTypeLoader.Load(reader, dom, false);
            if (ValueType != null)
            {
                if (valType == null)
                {
                    valType = ValueType;
                }
                else
                {
                    if (valType.TypeId == ValueType.TypeId)
                    {
                        valType = ValueType;
                    }
                }
            }

            int len = (int)reader.ReadNumber();
            int len2 = (int)reader.ReadNumber();
            if (len != len2)
            {
                throw new InvalidOperationException("Map length values aren't the same?!");
            }

            Dictionary<object, object> result = new Dictionary<object, object>(len);

            for (var i = 0; i < len; i++)
            {
                var key = keyType.ReadItem(dom, reader);
                var val = valType.ReadItem(dom, reader);
                result.Add(key, val);
            }

            return result;
        }
    }
}
