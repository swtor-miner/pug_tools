namespace PugTools
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
            Id = id;
            parentId = parent;
            displayName = display;
            if (obj is GomLib.DataObjectModel model)
                _dom = model;
            else if (obj is GomLib.GomObject @object)
                _obj = @object;
            else if (obj is GomLib.GomObjectData data)
                objData = data;
            else if (obj is FileFormats.GR2 gR)
                dynObject = gR;
            else if (obj is FileFormats.GR2_Material material)
                dynObject = material;
            else if (obj is FileFormats.GR2_Mesh mesh)
                dynObject = mesh;
            else if (obj is TorLib.HashFileInfo info)
                dynObject = info;
            else if (obj is System.Drawing.Bitmap bitmap)
                dynObject = bitmap;
            else if (obj is FileFormats.GR2_Bone_Skeleton skeleton)
                dynObject = skeleton;
        }
    }
}
