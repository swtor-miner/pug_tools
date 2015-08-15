using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using nsHashDictionary;
using TorLib;

namespace tor_tools
{
    public partial class AssetBrowserHashStatus : Form
    {
        private HashDictionaryInstance hashData;
        private TorLib.Assets currentAssets;

        private int intAllTotal = 0;
        private int intAllNamed = 0;
        private int intAllMissing = 0;
        private double dblAllCompletion = 0;

        private DataTable dt = new DataTable();

        public AssetBrowserHashStatus()
        {
            InitializeComponent();
        }

        private void AssetBrowserHashStatus_Load(object sender, EventArgs e)
        {
            hashData = HashDictionaryInstance.Instance;
            currentAssets = AssetHandler.Instance.getCurrentAssets();
            pictureBox1.Visible = true;
            dgvHashStatus.Enabled = false;
            toolStripProgressBar1.Visible = true;
            toolStripStatusLabel1.Text = "Calculating Hash Status...";
            this.Refresh();
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            dt = new DataTable();
            dt.Columns.Add("Archive", typeof(string));
            dt.Columns.Add("Total Files", typeof(int));
            dt.Columns.Add("Named Files", typeof(int));
            dt.Columns.Add("Missing Files", typeof(int));
            dt.Columns.Add("Completion", typeof(double));            

            foreach (Library lib in currentAssets.libraries)
            {
                foreach (KeyValuePair<int, Archive> arch in lib.archives)
                {
                    string archName = arch.Value.FileName.Split('\\').Last();
                    int intTotal = 0;
                    int intNamed = 0;

                    foreach (TorLib.File file in arch.Value.files)
                    {
                        intTotal++;
                        intAllTotal++;
                        HashFileInfo hashInfo = new HashFileInfo(file.FileInfo.ph, file.FileInfo.sh, file);

                        if (hashInfo.IsNamed)
                        {
                            intNamed++;
                            intAllNamed++;
                        }
                    }
                    int intMissing = intTotal - intNamed;
                    double dblCompletion = (double)intNamed / (double)intTotal;
                    DataRow row = dt.NewRow();
                    row[0] = archName;
                    row[1] = intTotal;
                    row[2] = intNamed;
                    row[3] = intMissing;
                    row[4] = dblCompletion;
                    dt.Rows.Add(row);
                    //dt.Rows.Add(new string[] { archName, String.Format("{0:n0}", intTotal), String.Format("{0:n0}", intNamed), String.Format("{0:n0}", intMissing), String.Format("{0:0.0%}", dblCompletion) });
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dgvHashStatus.DataSource = dt;
            dgvHashStatus.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvHashStatus.Enabled = true;
            dgvHashStatus.Columns["Total Files"].DefaultCellStyle.Format = "0";
            dgvHashStatus.Columns["Named Files"].DefaultCellStyle.Format = "0";
            dgvHashStatus.Columns["Missing Files"].DefaultCellStyle.Format = "0";            
            dgvHashStatus.Columns["Completion"].DefaultCellStyle.Format = "0.0%";

            intAllMissing = intAllTotal - intAllNamed;
            dblAllCompletion = (double)intAllNamed / (double)intAllTotal;

            lblTotalFilesVal.Text = String.Format("{0:n0}", intAllTotal);
            lblTotalNamedVal.Text = String.Format("{0:n0}", intAllNamed);
            lblTotalMissingVal.Text = String.Format("{0:n0}", intAllMissing);
            lblCompletionVal.Text = String.Format("{0:0.0%}", dblAllCompletion);
            
            lblTotalFilesVal.Visible = true;
            lblTotalNamedVal.Visible = true;
            lblTotalMissingVal.Visible = true;
            lblCompletionVal.Visible = true;

            pictureBox1.Visible = false;
            toolStripProgressBar1.Visible = false;
            toolStripStatusLabel1.Text = "Complete";
        }
    }
}
