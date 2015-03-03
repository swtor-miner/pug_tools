using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GomLib;

namespace tor_tools
{
    class WemListItem

    {   
        public string name;
        public WEM_File obj;
        public int size;
        public string displaySize;

        public List<NodeListItem> children = new List<NodeListItem>();

        public WemListItem(string name, WEM_File obj)
        {
            this.name = name;
            this.obj = (WEM_File)obj;
            this.size = (obj.data.Count() / 1024);
            this.displaySize = this.size.ToString("#,##0");
         }

        public static void resetTreeListViewColumns(BrightIdeasSoftware.TreeListView tlv)
        {
            BrightIdeasSoftware.OLVColumn olvColumn1 = new BrightIdeasSoftware.OLVColumn();
            BrightIdeasSoftware.OLVColumn olvColumn2 = new BrightIdeasSoftware.OLVColumn();

            olvColumn1.AspectName = "name";
            olvColumn1.CellPadding = null;
            olvColumn1.Text = "Name";
            olvColumn1.Width = 167;

            olvColumn2.AspectName = "displaySize";
            olvColumn2.CellPadding = null;
            olvColumn2.Text = "Size (KB)";
            olvColumn2.Width = 230;

            tlv.Columns.Clear();
            tlv.Columns.Add(olvColumn1);
            tlv.Columns.Add(olvColumn2);
        }
    }
}
