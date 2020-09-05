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
            if (this.chkAMX.Checked)
                fileTypes.Add("AMX");
            if (this.chkBNK.Checked)
                fileTypes.Add("BNK");
            if (this.chkCNV.Checked)
                fileTypes.Add("CNV");
            if (this.chkDAT.Checked)
                fileTypes.Add("DAT");
            if (this.chkDYN.Checked)
                fileTypes.Add("DYN");
            if (this.chkEPP.Checked)
                fileTypes.Add("EPP");
            if (this.chkFXSPEC.Checked)
                fileTypes.Add("FXSPEC");
            if (this.chkGR2.Checked)
                fileTypes.Add("GR2");
            if (this.chkHYD.Checked)
                fileTypes.Add("HYD");
            if (this.chkICONS.Checked)
                fileTypes.Add("ICONS");
            if (this.chkMAT.Checked)
                fileTypes.Add("MAT");
            if (this.chkMISC.Checked)
                fileTypes.Add("MISC");
            if (this.chkMISC_WORLD.Checked)
                fileTypes.Add("MISC_WORLD");
            if (this.chkPLC.Checked)
                fileTypes.Add("PLC");
            if (this.chkPRT.Checked)
                fileTypes.Add("PRT");
            if (this.chkSDEF.Checked)
                fileTypes.Add("SDEF");
            if (this.chkSTB.Checked)
                fileTypes.Add("STB");
            if (this.chkXML.Checked)
                fileTypes.Add("XML");
            return fileTypes;
        }

        private void ChkALL_CheckedChanged(object sender, EventArgs e)
        {
            bool value = this.chkALL.Checked;
            this.chkAMX.Checked = value;
            this.chkBNK.Checked = value;
            this.chkCNV.Checked = value;
            this.chkDAT.Checked = value;
            this.chkDYN.Checked = value;
            this.chkEPP.Checked = value;
            this.chkFXSPEC.Checked = value;
            this.chkGR2.Checked = value;
            this.chkHYD.Checked = value;
            this.chkICONS.Checked = value;
            this.chkMAT.Checked = value;
            this.chkMISC.Checked = value;
            this.chkMISC_WORLD.Checked = value;
            this.chkPLC.Checked = value;
            this.chkPRT.Checked = value;
            this.chkSDEF.Checked = value;
            this.chkSTB.Checked = value;
            this.chkXML.Checked = value;
        }
    }
}
