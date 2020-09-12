using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PugTools
{
    public partial class AssetBrowserTestFile : Form
    {
        public List<string> fileTypes = new List<string>();

        public AssetBrowserTestFile()
        {
            InitializeComponent();
        }

        public List<string> GetTypes()
        {
            fileTypes.Clear();
            if (chkAMX.Checked)
                fileTypes.Add("AMX");
            if (chkBNK.Checked)
                fileTypes.Add("BNK");
            if (chkCNV.Checked)
                fileTypes.Add("CNV");
            if (chkDAT.Checked)
                fileTypes.Add("DAT");
            if (chkDYN.Checked)
                fileTypes.Add("DYN");
            if (chkEPP.Checked)
                fileTypes.Add("EPP");
            if (chkFXSPEC.Checked)
                fileTypes.Add("FXSPEC");
            if (chkGR2.Checked)
                fileTypes.Add("GR2");
            if (chkHYD.Checked)
                fileTypes.Add("HYD");
            if (chkICONS.Checked)
                fileTypes.Add("ICONS");
            if (chkMAT.Checked)
                fileTypes.Add("MAT");
            if (chkMISC.Checked)
                fileTypes.Add("MISC");
            if (chkMISC_WORLD.Checked)
                fileTypes.Add("MISC_WORLD");
            if (chkPLC.Checked)
                fileTypes.Add("PLC");
            if (chkPRT.Checked)
                fileTypes.Add("PRT");
            if (chkSDEF.Checked)
                fileTypes.Add("SDEF");
            if (chkSTB.Checked)
                fileTypes.Add("STB");
            if (chkXML.Checked)
                fileTypes.Add("XML");
            return fileTypes;
        }

        private void ChkALL_CheckedChanged(object sender, EventArgs e)
        {
            bool value = chkALL.Checked;
            chkAMX.Checked = value;
            chkBNK.Checked = value;
            chkCNV.Checked = value;
            chkDAT.Checked = value;
            chkDYN.Checked = value;
            chkEPP.Checked = value;
            chkFXSPEC.Checked = value;
            chkGR2.Checked = value;
            chkHYD.Checked = value;
            chkICONS.Checked = value;
            chkMAT.Checked = value;
            chkMISC.Checked = value;
            chkMISC_WORLD.Checked = value;
            chkPLC.Checked = value;
            chkPRT.Checked = value;
            chkSDEF.Checked = value;
            chkSTB.Checked = value;
            chkXML.Checked = value;
        }
    }
}
