using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GomLib;

namespace PugTools
{
    class WemListItem

    {
        public string name;
        public WEM_File obj;
        public int size = 0;
        public string value = "";

        public List<WemListItem> children = new List<WemListItem>();

        public WemListItem(string name, WEM_File obj)
        {
            this.name = name;
            this.obj = obj;
            size = (obj.Data.Count() / 1024);
            value = "";
        }

        public WemListItem(string name, FileFormat_BNK_DIDX didx)
        {
            this.name = name;
            size = 0;
            value = "";
            BnkIdDict dict = BnkIdDict.Instance;
            if (didx.wems.Count > 0)
            {
                foreach (WEM_File wem in didx.wems)
                {
                    uint.TryParse(wem.Name.Replace(".wem", ""), out uint id);
                    if (dict.Data.Keys.Contains(id))
                    {
                        wem.Name = dict.Data[id] + ".wem";
                    }
                    children.Add(new WemListItem(wem.Name.ToString(), wem));
                }
            }
        }

        public WemListItem(string name, FileFormat_BNK_STID stid)
        {
            this.name = name;
            _ = BnkIdDict.Instance;
            if (stid.numSoundBanks > 0)
            {
                foreach (FileFormat_BNK_STID_SoundBank bnk in stid.soundBanks)
                {
                    children.Add(new WemListItem(bnk));
                }
            }
        }

        public WemListItem(string name, FileFormat_BNK_HIRC hirc)
        {
            this.name = name;
            if (hirc.numObject > 0)
            {
                Dictionary<uint, List<FileFormat_BNK_HIRC_Object>> hircDict = new Dictionary<uint, List<FileFormat_BNK_HIRC_Object>>();
                foreach (FileFormat_BNK_HIRC_Object obj in hirc.objects)
                {
                    if (!hircDict.Keys.Contains(obj.type))
                    {
                        List<FileFormat_BNK_HIRC_Object> objList = new List<FileFormat_BNK_HIRC_Object>
                        {
                            obj
                        };
                        hircDict.Add(obj.type, objList);
                    }
                    else
                    {
                        hircDict[obj.type].Add(obj);
                    }
                }
                foreach (var kvp in hircDict)
                {
                    string displayName;
                    switch (kvp.Key)
                    {
                        case 1:
                            displayName = "1 - Settings";
                            break;
                        case 2:
                            displayName = "2 - Sound SFX/Sound Voice";
                            break;
                        case 3:
                            displayName = "3 - Event Action";
                            break;
                        case 4:
                            displayName = "4 - Event";
                            break;
                        case 5:
                            displayName = "5 - Random Container or Sequence Container";
                            break;
                        case 6:
                            displayName = "6 - Switch Container";
                            break;
                        case 7:
                            displayName = "7 - Actor-Mixer";
                            break;
                        case 8:
                            displayName = "8 - Audio Bus";
                            break;
                        case 9:
                            displayName = "9 - Blend Container";
                            break;
                        case 10:
                            displayName = "10 - Music Segment";
                            break;
                        case 11:
                            displayName = "11 - Music Track";
                            break;
                        case 12:
                            displayName = "12 - Music Switch Container";
                            break;
                        case 13:
                            displayName = "13 - Music Playlist Container";
                            break;
                        case 14:
                            displayName = "14 - Attenuation";
                            break;
                        case 15:
                            displayName = "15 - Dialogue Event";
                            break;
                        case 16:
                            displayName = "16 - Motion Bus";
                            break;
                        case 17:
                            displayName = "17 - Motion FX";
                            break;
                        case 18:
                            displayName = "18 - Effect";
                            break;
                        case 20:
                            displayName = "20 - Auxiliary Bus";
                            break;
                        default:
                            displayName = "**UNKNOWN**";
                            break;

                    }
                    children.Add(new WemListItem(displayName, kvp.Value));
                }
                //Console.WriteLine("pause here");
            }

        }

        public WemListItem(string name, List<FileFormat_BNK_HIRC_Object> objList)
        {
            this.name = name;
            foreach (var obj in objList)
            {
                FileFormat_BNK_HIRC_Object hircObj = obj;
                children.Add(new WemListItem(obj.id.ToString(), hircObj));
            }
        }

        public WemListItem(string name, FileFormat_BNK_HIRC_Object objList)
        {
            if (objList is null)
            {
                throw new ArgumentNullException(nameof(objList));
            }

            this.name = name;
            BnkIdDict dict = BnkIdDict.Instance;
            uint.TryParse(this.name, out uint id);
            if (dict.Data.Keys.Contains(id))
            {
                value = dict.Data[id];
            }
        }

        public WemListItem(FileFormat_BNK_STID_SoundBank bnk)
        {
            name = bnk.name;
        }

        public static void ResetTreeListViewColumns(BrightIdeasSoftware.TreeListView tlv)
        {
            BrightIdeasSoftware.OLVColumn olvColumn1 = new BrightIdeasSoftware.OLVColumn();
            BrightIdeasSoftware.OLVColumn olvColumn2 = new BrightIdeasSoftware.OLVColumn();
            BrightIdeasSoftware.OLVColumn olvColumn3 = new BrightIdeasSoftware.OLVColumn();

            olvColumn1.AspectName = "name";
            olvColumn1.CellPadding = null;
            olvColumn1.Text = "Name";
            olvColumn1.MinimumWidth = 167;
            olvColumn1.AutoResize(System.Windows.Forms.ColumnHeaderAutoResizeStyle.ColumnContent);

            olvColumn2.AspectName = "value";
            olvColumn2.CellPadding = null;
            olvColumn2.Text = "Event / Action";
            olvColumn2.Width = 230;

            olvColumn3.AspectName = "size";
            olvColumn3.CellPadding = null;
            olvColumn3.Text = "Size (KB)";
            olvColumn3.Width = 230;


            tlv.Columns.Clear();
            tlv.Columns.Add(olvColumn1);
            tlv.Columns[0].AutoResize(System.Windows.Forms.ColumnHeaderAutoResizeStyle.ColumnContent);
            tlv.Columns.Add(olvColumn2);
            tlv.Columns.Add(olvColumn3);
        }
    }
}
