using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GomLib.GomTypes
{
    public class Vector : GomType
    {
        public Vector() : base(GomTypeId.Vec3) { }

        public override string ToString()
        {
            return "vector3";
        }

        public override object ReadData(DataObjectModel dom, GomBinaryReader reader)
        {
            List<float> vec = new List<float>(3);
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            vec.Add(x);
            vec.Add(y);
            vec.Add(z);

            return vec;
        }
    }
}
