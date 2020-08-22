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
    public partial class AssetBrowserExtractExt : Form
    {
        private readonly HashSet<string> extensions = new HashSet<string>();

        public AssetBrowserExtractExt()
        {
            InitializeComponent();
        }

        public HashSet<string> GetExtensions()
        {
            return extensions;
        }

        private void AssetBrowserExtractExt_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (extensions.Count == 0)
            {
                MessageBox.Show("Please enter a list of extensions sperated by a space.", "ERROR: Empty List", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Cancel = true;
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            string[] temp = txtExts.Text.Split(' ');
            foreach (string item in temp)
            {
                if (item != "")
                    extensions.Add(item.ToUpper());
            }
        }

        private void TxtExts_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                btnOK.PerformClick();
            }
        }
    }
}
