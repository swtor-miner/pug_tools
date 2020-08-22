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
    public partial class ExtractCheckForm : Form
    {
        public List<string> fileTypes = new List<string>();

        public ExtractCheckForm()
        {
            InitializeComponent();
        }

        public List<string> GetTypes()
        {
            fileTypes.Clear();
            if (this.chkCdxCat.Checked)
                fileTypes.Add("CDXCAT");
            if (this.chkSetBonus.Checked)
                fileTypes.Add("SBN");
            if (this.chkGOM.Checked)
                fileTypes.Add("GOM");
            if (this.chkEXP.Checked)
                fileTypes.Add("EXP");
            if (this.chkAREA.Checked)
                fileTypes.Add("AREA");
            if (this.chkSTB.Checked)
                fileTypes.Add("STB");

            if (this.chkABL.Checked)
                fileTypes.Add("ABL");
            if (this.chkABLEFF.Checked)
                fileTypes.Add("ABLEFF");
            if (this.chkACH.Checked)
                fileTypes.Add("ACH");
            if (this.chkAPT.Checked)
                fileTypes.Add("APT");
            if (this.chkCDX.Checked)
                fileTypes.Add("CDX");
            if (this.chkCNV.Checked)
                fileTypes.Add("CNV");
            if (this.chkCMP.Checked)
                fileTypes.Add("CMP");
            if (this.chkDEC.Checked)
                fileTypes.Add("DEC");
            if (this.chkIPP.Checked)
                fileTypes.Add("IPP");
            if (this.chkITM.Checked)
                fileTypes.Add("ITM");
            if (this.chkNPC.Checked)
                fileTypes.Add("NPC");
            if (this.chkNPP.Checked)
                fileTypes.Add("NPP");
            if (this.chkQST.Checked)
                fileTypes.Add("QST");
            if (this.chkSCHEM.Checked)
                fileTypes.Add("SCHEM");
            if (this.chkTAL.Checked)
                fileTypes.Add("TAL");

            if (this.chkCOL.Checked)
                fileTypes.Add("COL");
            if (this.chkMTX.Checked)
                fileTypes.Add("MTX");
            if (this.chkGSF.Checked)
                fileTypes.Add("GSF");
            if (this.chkCNQ.Checked)
                fileTypes.Add("CNQ");
            if (this.chkAC.Checked)
                fileTypes.Add("AC");

            if (this.chkMISC.Checked)
                fileTypes.Add("MISC");

            if (this.chkICONS.Checked)
                fileTypes.Add("ICONS");
            if (this.chkTORC.Checked)
                fileTypes.Add("TORC");
            if (this.chkSchemVaris.Checked)
                fileTypes.Add("SCHVARI");
            if (this.chkClass.Checked)
                fileTypes.Add("CLASS");
            if (this.chkAPN.Checked)
                fileTypes.Add("APN");
            if (this.chkSPN.Checked)
                fileTypes.Add("SPN");
            return fileTypes;
        }

        private void ChkALL_CheckedChanged(object sender, EventArgs e)
        {
            bool value = this.chkALL.Checked;
            this.chkABL.Checked = value;
            this.chkABLEFF.Checked = value;
            this.chkAC.Checked = value;
            this.chkACH.Checked = value;
            this.chkAPT.Checked = value;
            this.chkAREA.Checked = value;
            this.chkCDX.Checked = value;
            this.chkCMP.Checked = value;
            this.chkCNQ.Checked = value;
            this.chkCNV.Checked = value;
            this.chkCOL.Checked = value;
            this.chkDEC.Checked = value;
            this.chkEXP.Checked = value;
            this.chkGOM.Checked = value;
            this.chkGSF.Checked = value;
            this.chkIPP.Checked = value;
            this.chkITM.Checked = value;
            this.chkMISC.Checked = value;
            this.chkMTX.Checked = value;
            this.chkNPC.Checked = value;
            this.chkNPP.Checked = value;
            this.chkQST.Checked = value;
            this.chkSCHEM.Checked = value;
            this.chkSTB.Checked = value;
            this.chkTAL.Checked = value;
            this.chkICONS.Checked = value;
            this.chkSetBonus.Checked = value;
            this.chkCdxCat.Checked = value;
            this.chkSchemVaris.Checked = value;
        }

        private void ChkDB_CheckedChanged(object sender, EventArgs e)
        {
            this.chkABLEFF.Checked = false;
            this.chkEXP.Checked = false;
            this.chkGOM.Checked = false;
            this.chkIPP.Checked = false;
            this.chkMISC.Checked = false;
            this.chkNPP.Checked = false;
            this.chkSTB.Checked = false;
            this.chkICONS.Checked = false;
            this.chkCdxCat.Checked = false;
            this.chkSchemVaris.Checked = false;

            this.chkACH.Checked = true;
            this.chkABL.Checked = true;
            this.chkAC.Checked = true;
            this.chkAPT.Checked = true;
            this.chkAREA.Checked = true;
            this.chkCDX.Checked = true;
            this.chkCMP.Checked = true;
            this.chkCNQ.Checked = true;
            this.chkCNV.Checked = true;
            this.chkCOL.Checked = true;
            this.chkDEC.Checked = true;
            this.chkGSF.Checked = true;
            this.chkITM.Checked = true;
            this.chkMTX.Checked = true;
            this.chkNPC.Checked = true;
            this.chkSCHEM.Checked = true;
            this.chkQST.Checked = true;
            this.chkClass.Checked = true;
            this.chkTAL.Checked = true;
            this.chkSetBonus.Checked = true;
            this.chkAPN.Checked = true;
        }
    }
}
