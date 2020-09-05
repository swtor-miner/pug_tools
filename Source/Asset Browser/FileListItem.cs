using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GomLib;
using TorLib;
using BrightIdeasSoftware;

namespace PugTools
{
    class FileListItem

    {
        public string name;
        public string extension;
        public string directory;
        public ulong offset;
        public ulong sizeCompressed;
        public ulong sizeUncompressed;
        public bool isCompressed;
        public ushort compressedMethod;

        public FileListItem(HashFileInfo hashInfo, TorLib.FileInfo info)
        {
            this.name = hashInfo.FileName;
            this.extension = hashInfo.Extension.ToUpper();
            if (hashInfo.IsNamed)
                this.directory = hashInfo.Directory;
            else
                this.directory = "Unknown";
            this.offset = info.Offset;
            this.sizeCompressed = info.CompressedSize;
            this.sizeUncompressed = info.UncompressedSize;
            this.isCompressed = info.IsCompressed;
            this.compressedMethod = info.CompressionMethod;
        }

        public static void ResetTreeListViewColumns(TreeListView tlv)
        {
            OLVColumn olvColumn1 = new OLVColumn();
            OLVColumn olvColumn2 = new OLVColumn();
            OLVColumn olvColumn3 = new OLVColumn();
            OLVColumn olvColumn4 = new OLVColumn();
            OLVColumn olvColumn5 = new OLVColumn();
            OLVColumn olvColumn6 = new OLVColumn();
            OLVColumn olvColumn7 = new OLVColumn();
            OLVColumn olvColumn8 = new OLVColumn();

            olvColumn1.AspectName = "name";
            olvColumn1.CellPadding = null;
            olvColumn1.Text = "Name";
            olvColumn1.Width = 320;

            olvColumn2.AspectName = "extension";
            olvColumn2.CellPadding = null;
            olvColumn2.Text = "File Type";

            olvColumn3.AspectName = "directory";
            olvColumn3.CellPadding = null;
            olvColumn3.Text = "Directory";
            olvColumn3.Width = 160;

            olvColumn4.AspectName = "offset";
            olvColumn4.CellPadding = null;
            olvColumn4.Text = "Offset";

            olvColumn5.AspectName = "sizeUncompressed";
            olvColumn5.CellPadding = null;
            olvColumn5.Text = "Size";

            olvColumn6.AspectName = "sizeCompressed";
            olvColumn6.CellPadding = null;
            olvColumn6.Text = "Compressed Size";

            olvColumn7.AspectName = "isCompressed";
            olvColumn7.CellPadding = null;
            olvColumn7.Text = "Is Compressed";

            olvColumn8.AspectName = "compressedMethod";
            olvColumn8.CellPadding = null;
            olvColumn8.Text = "Compressed Method";

            tlv.Columns.Clear();
            tlv.Columns.Add(olvColumn1);
            tlv.Columns.Add(olvColumn2);
            tlv.Columns.Add(olvColumn3);
            tlv.Columns.Add(olvColumn4);
            tlv.Columns.Add(olvColumn5);
            tlv.Columns.Add(olvColumn6);
            tlv.Columns.Add(olvColumn7);
            tlv.Columns.Add(olvColumn8);
        }
    }
}
