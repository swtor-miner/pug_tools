using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class EmptyLoader
    {
        private Dictionary<object, object> idLookup;

        public Dictionary<object, object> IdLookup { get => idLookup; set => idLookup = value; }

        public EmptyLoader(DataObjectModel _)
        {
            Flush();
        }

        public void Flush()
        {
            IdLookup = new Dictionary<object, object>();
        }

        public Models.Empty Load(Models.Empty model, GomObject gom)
        {
            if (gom == null) { return model; }
            if (model == null) { return null; }

            //model.NodeId = obj.ValueOrDefault<long>("repGroupInfoId", 0);
            model.References = gom.References;

            return model;
        }
    }
}
