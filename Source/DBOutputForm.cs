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
    public partial class DBOutputForm : Form
    {
        public List<string> fileTypes = new List<string>();

        public DBOutputForm()
        {
            InitializeComponent();
        }

        private void VersionTexBox_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(versionTexBox.Text))
                btnOK.Enabled = true;
            else
                btnOK.Enabled = false;
        }
    }
}
