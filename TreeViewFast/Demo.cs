using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using TreeViewFast.DomainServices;
using TreeViewFast.Entities;
using TreeViewFast.Extenders;


namespace TreeViewFast
{
    public partial class Demo : Form
    {
        public Demo()
        {
            InitializeComponent();
        }


        #region Methods

        private void LoadItems()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                lblFeedbackStandard.Text = "Please wait for Standard TreeView to load ...";
                lblFeedbackFast.Text = "Please wait for TreeViewFast to load ...";

                // Parse count
                var itemCount = int.Parse(txtItemCount.Text);

                // Retrieve employees
                var employees = EmployeeGenerator.GetEmployees(itemCount);

                // Load items into both TreeViews
                LoadFast(employees);
                LoadStandard(employees);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "LoadItems", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void LoadFast(IEnumerable<Employee> employees)
        {
            // Define functions needed by the load method
            var start = DateTime.Now;
            Func<Employee, int> getId = (x => x.EmployeeId);
            Func<Employee, int?> getParentId = (x => x.ManagerId);
            Func<Employee, string> getDisplayName = (x => x.Name);

            // Load items into TreeViewFast
            myTreeViewFast.LoadItems(employees, getId, getParentId, getDisplayName);
            var elapsed = DateTime.Now.Subtract(start);
            lblFeedbackFast.Text = string.Format("TreeViewFast: {0:N0} ms ({1})", elapsed.TotalMilliseconds, elapsed.Display());
            Application.DoEvents();
        }

        private void LoadStandard(IEnumerable<Employee> employees)
        {
            var start = DateTime.Now;

            myTreeView.Nodes.Clear();
            foreach (var employee in employees)
            {
                var node = new TreeNode { Name = employee.EmployeeId.ToString(), Text = employee.Name, Tag = employee };
                if (employee.ManagerId.HasValue)
                {
                    var parentId = employee.ManagerId.Value.ToString();
                    var parentNode = myTreeView.Nodes.Find(parentId, true)[0];
                    parentNode.Nodes.Add(node);
                }
                else
                {
                    myTreeView.Nodes.Add(node);
                }
            }

            var elapsed = DateTime.Now.Subtract(start);
            lblFeedbackStandard.Text = string.Format("Standard TreeView: {0:N0} ms ({1})", elapsed.TotalMilliseconds, elapsed.Display());
        }

        #endregion



        #region Event handlers

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadItems();
        }

        #endregion

    }
}
