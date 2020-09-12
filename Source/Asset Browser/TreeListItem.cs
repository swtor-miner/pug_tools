using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorLib;

namespace PugTools
{
    public class TreeListItem
    {
        public string Id;
        public string parentId;
        public string displayName;
        public HashFileInfo hashInfo;

        public TreeListItem(string id, string parent, string display, HashFileInfo hashInfo)
        {
            Id = id;
            parentId = parent;
            displayName = display;
            this.hashInfo = hashInfo;
        }
    }
}
