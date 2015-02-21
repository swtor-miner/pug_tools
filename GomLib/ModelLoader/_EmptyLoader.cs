using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GomLib.Models;

namespace GomLib.ModelLoader
{
    public class EmptyLoader
    {
        Dictionary<object, object> idLookup;

        DataObjectModel _dom;

        public EmptyLoader(DataObjectModel dom)
        {
            _dom = dom;
            Flush();
        }

        public void Flush()
        {
            idLookup = new Dictionary<object, object>();
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
