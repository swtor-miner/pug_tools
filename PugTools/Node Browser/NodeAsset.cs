using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tor_tools
{
    public class NodeAsset
    {
        public string Id;
        public string parentId;
        public string displayName;
        public object dynObject;
        private GomLib.DataObjectModel _dom;
        private GomLib.GomObject _obj;
        public GomLib.GomObjectData objData;
        public GomLib.GomObject obj
        {
            get
            {
                if (_obj == null && _dom != null)
                    _obj = _dom.GetObject(Id);
                return _obj;
            }
        }

        public NodeAsset(string id, string parent, string display, object obj)
        {
            this.Id = id;
            this.parentId = parent;
            this.displayName = display;
            if (obj is GomLib.DataObjectModel)
                this._dom = (GomLib.DataObjectModel)obj;
            else if (obj is GomLib.GomObject)
                this._obj = (GomLib.GomObject)obj;
            else if (obj is GomLib.GomObjectData)
                this.objData = (GomLib.GomObjectData)obj;
            else if (obj is FileFormats.GR2)
                this.dynObject = (FileFormats.GR2)obj;
            else if (obj is FileFormats.GR2_Material)
                this.dynObject = (FileFormats.GR2_Material)obj;
            else if (obj is FileFormats.GR2_Mesh)
                this.dynObject = (FileFormats.GR2_Mesh)obj;
            else if (obj is TorLib.HashFileInfo)
                this.dynObject = (TorLib.HashFileInfo)obj;
            else if (obj is System.Drawing.Bitmap)
                this.dynObject = (System.Drawing.Bitmap)obj;
            else if (obj is FileFormats.GR2_Bone_Skeleton)
                this.dynObject = (FileFormats.GR2_Bone_Skeleton)obj;
        }
    }     
}
