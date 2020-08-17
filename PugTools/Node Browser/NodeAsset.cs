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
        private readonly GomLib.DataObjectModel _dom;
        private GomLib.GomObject _obj;
        public GomLib.GomObjectData objData;
        public GomLib.GomObject Obj
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
            if (obj is GomLib.DataObjectModel model)
                this._dom = model;
            else if (obj is GomLib.GomObject @object)
                this._obj = @object;
            else if (obj is GomLib.GomObjectData data)
                this.objData = data;
            else if (obj is FileFormats.GR2 gR)
                this.dynObject = gR;
            else if (obj is FileFormats.GR2_Material material)
                this.dynObject = material;
            else if (obj is FileFormats.GR2_Mesh mesh)
                this.dynObject = mesh;
            else if (obj is TorLib.HashFileInfo info)
                this.dynObject = info;
            else if (obj is System.Drawing.Bitmap bitmap)
                this.dynObject = bitmap;
            else if (obj is FileFormats.GR2_Bone_Skeleton skeleton)
                this.dynObject = skeleton;
        }
    }
}
