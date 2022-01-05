using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MixMods.MixPackageManager.Commands.Script
{
    public partial class OptionDialog : Form
    {
        public OptionDialog()
        {
            InitializeComponent();
        }

        private void OptionDialog_Load(object sender, EventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        private int componentsCount = 1;
        public void AddComponent(Control control)
        {
            if (componentsCount > 1)
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.Controls.Add(control, 0, componentsCount-1);
            tableLayoutPanel1.RowCount = componentsCount;
            componentsCount++;
            control.Dock = DockStyle.Top;
        }
    }
}
