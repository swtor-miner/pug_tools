using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tor_tools
{
    public partial class AssetBrowserTestFile : Form
    {
        public List<string> fileTypes = new List<string>();

        public AssetBrowserTestFile()
        {
            InitializeComponent();
        }

        public List<string> getTypes()
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
            if (this.chkEPP.Checked)
                fileTypes.Add("EPP");
            if (this.chkFXSPEC.Checked)
                fileTypes.Add("FXSPEC");
            if (this.chkGR2.Checked)
                fileTypes.Add("GR2");
            if (this.chkMAT.Checked)
                fileTypes.Add("MAT");
            if (this.chkMISC.Checked)
                fileTypes.Add("MISC");
            if (this.chkMISC_WORLD.Checked)
                fileTypes.Add("MISC_WORLD");
            if (this.chkPRT.Checked)
                fileTypes.Add("PRT");
            if (this.chkXML.Checked)
                fileTypes.Add("XML");
            return fileTypes;
        }

        private void chkALL_CheckedChanged(object sender, EventArgs e)
        {
            bool value = this.chkALL.Checked;
            this.chkAMX.Checked = value;
            this.chkBNK.Checked = value;
            this.chkCNV.Checked = value;                
            this.chkDAT.Checked = value;                
            this.chkEPP.Checked = value;
            this.chkFXSPEC.Checked = value;                
            this.chkGR2.Checked = value;                
            this.chkMAT.Checked = value;
            this.chkMISC.Checked = value;
            this.chkMISC_WORLD.Checked = value;               
            this.chkPRT.Checked = value;                
            this.chkXML.Checked = value;
        }
    }
}
