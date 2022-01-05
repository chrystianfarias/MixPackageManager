using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MixMods.MixPackageManager.Commands.Script
{
    public class Dialog
    {
        private OptionDialog form;
        public Dialog() : this("Options") { }
        public Dialog(string title)
        {
            OptionDialog dialog = new OptionDialog();
            dialog.Text = title;
            this.form = dialog;
        }
        public string show()
        {
            var res = form.ShowDialog();
            if (res == DialogResult.OK)
                return "OK";
            if (res == DialogResult.Cancel)
                return "Cancel";
            return "Close";
        }
        public Label createLabel(string text)
        {
            var control = new System.Windows.Forms.Label();
            control.Text = text;
            form.AddComponent(control);
            return new Label(control);
        }
        public CheckBox createCheckBox(string text, bool value)
        {
            var control = new System.Windows.Forms.CheckBox();
            control.Text = text;
            control.Checked = value;
            form.AddComponent(control);
            return new CheckBox(control);
        }
        public Button createButton(string text, Action action)
        {
            var control = new System.Windows.Forms.Button();
            control.Text = text;
            control.Click += new EventHandler((Object sender, EventArgs e) => action.Invoke());

            form.AddComponent(control);
            return new Button(control);
        }
        public List createList(string[] items)
        {
            var control = new System.Windows.Forms.ListBox();
            control.Items.AddRange(items);
            form.AddComponent(control);
            return new List(control);
        }
        public void setSize(int x, int y)
        {
            form.Size = new System.Drawing.Size(x,y);
        }
    }
}
