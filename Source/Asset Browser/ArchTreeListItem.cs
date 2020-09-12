using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorLib;

namespace PugTools
{
    public class ArchTreeListItem
    {
        public string Id;
        public string parentId;
        public string displayName;
        public Archive arch;

        public ArchTreeListItem(string id, string parent, string display, Archive arch)
        {
            Id = id;
            parentId = parent;
            displayName = display;
            this.arch = arch;
        }
    }
}
