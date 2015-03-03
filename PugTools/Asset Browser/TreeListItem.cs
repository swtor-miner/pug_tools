using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorLib;

namespace tor_tools
{
    public class TreeListItem
    {
        public string Id;
        public string parentId;
        public string displayName;
        public HashFileInfo hashInfo;        

        public TreeListItem(string id, string parent, string display, HashFileInfo hashInfo)
        {
            this.Id = id;
            this.parentId = parent;
            this.displayName = display;
            this.hashInfo = hashInfo;         
        }
    }     
}
