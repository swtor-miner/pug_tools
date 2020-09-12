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
            if (chkCdxCat.Checked)
                fileTypes.Add("CDXCAT");
            if (chkSetBonus.Checked)
                fileTypes.Add("SBN");
            if (chkGOM.Checked)
                fileTypes.Add("GOM");
            if (chkEXP.Checked)
                fileTypes.Add("EXP");
            if (chkAREA.Checked)
                fileTypes.Add("AREA");
            if (chkSTB.Checked)
                fileTypes.Add("STB");

            if (chkABL.Checked)
                fileTypes.Add("ABL");
            if (chkABLEFF.Checked)
                fileTypes.Add("ABLEFF");
            if (chkACH.Checked)
                fileTypes.Add("ACH");
            if (chkAPT.Checked)
                fileTypes.Add("APT");
            if (chkCDX.Checked)
                fileTypes.Add("CDX");
            if (chkCNV.Checked)
                fileTypes.Add("CNV");
            if (chkCMP.Checked)
                fileTypes.Add("CMP");
            if (chkDEC.Checked)
                fileTypes.Add("DEC");
            if (chkIPP.Checked)
                fileTypes.Add("IPP");
            if (chkITM.Checked)
                fileTypes.Add("ITM");
            if (chkNPC.Checked)
                fileTypes.Add("NPC");
            if (chkNPP.Checked)
                fileTypes.Add("NPP");
            if (chkQST.Checked)
                fileTypes.Add("QST");
            if (chkSCHEM.Checked)
                fileTypes.Add("SCHEM");
            if (chkTAL.Checked)
                fileTypes.Add("TAL");

            if (chkCOL.Checked)
                fileTypes.Add("COL");
            if (chkMTX.Checked)
                fileTypes.Add("MTX");
            if (chkGSF.Checked)
                fileTypes.Add("GSF");
            if (chkCNQ.Checked)
                fileTypes.Add("CNQ");
            if (chkAC.Checked)
                fileTypes.Add("AC");

            if (chkMISC.Checked)
                fileTypes.Add("MISC");

            if (chkICONS.Checked)
                fileTypes.Add("ICONS");
            if (chkTORC.Checked)
                fileTypes.Add("TORC");
            if (chkSchemVaris.Checked)
                fileTypes.Add("SCHVARI");
            if (chkClass.Checked)
                fileTypes.Add("CLASS");
            if (chkAPN.Checked)
                fileTypes.Add("APN");
            if (chkSPN.Checked)
                fileTypes.Add("SPN");
            return fileTypes;
        }

        private void ChkALL_CheckedChanged(object sender, EventArgs e)
        {
            bool value = chkALL.Checked;
            chkABL.Checked = value;
            chkABLEFF.Checked = value;
            chkAC.Checked = value;
            chkACH.Checked = value;
            chkAPT.Checked = value;
            chkAREA.Checked = value;
            chkCDX.Checked = value;
            chkCMP.Checked = value;
            chkCNQ.Checked = value;
            chkCNV.Checked = value;
            chkCOL.Checked = value;
            chkDEC.Checked = value;
            chkEXP.Checked = value;
            chkGOM.Checked = value;
            chkGSF.Checked = value;
            chkIPP.Checked = value;
            chkITM.Checked = value;
            chkMISC.Checked = value;
            chkMTX.Checked = value;
            chkNPC.Checked = value;
            chkNPP.Checked = value;
            chkQST.Checked = value;
            chkSCHEM.Checked = value;
            chkSTB.Checked = value;
            chkTAL.Checked = value;
            chkICONS.Checked = value;
            chkSetBonus.Checked = value;
            chkCdxCat.Checked = value;
            chkSchemVaris.Checked = value;
        }

        private void ChkDB_CheckedChanged(object sender, EventArgs e)
        {
            chkABLEFF.Checked = false;
            chkEXP.Checked = false;
            chkGOM.Checked = false;
            chkIPP.Checked = false;
            chkMISC.Checked = false;
            chkNPP.Checked = false;
            chkSTB.Checked = false;
            chkICONS.Checked = false;
            chkCdxCat.Checked = false;
            chkSchemVaris.Checked = false;

            chkACH.Checked = true;
            chkABL.Checked = true;
            chkAC.Checked = true;
            chkAPT.Checked = true;
            chkAREA.Checked = true;
            chkCDX.Checked = true;
            chkCMP.Checked = true;
            chkCNQ.Checked = true;
            chkCNV.Checked = true;
            chkCOL.Checked = true;
            chkDEC.Checked = true;
            chkGSF.Checked = true;
            chkITM.Checked = true;
            chkMTX.Checked = true;
            chkNPC.Checked = true;
            chkSCHEM.Checked = true;
            chkQST.Checked = true;
            chkClass.Checked = true;
            chkTAL.Checked = true;
            chkSetBonus.Checked = true;
            chkAPN.Checked = true;
        }
    }
}
