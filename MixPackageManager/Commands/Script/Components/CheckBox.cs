using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MixMods.MixPackageManager.Commands.Script
{
    public class CheckBox
    {
        private System.Windows.Forms.CheckBox cb;
        public CheckBox(System.Windows.Forms.CheckBox cb)
        {
            this.cb = cb;
        }

        public void setText(string text)
        {
            cb.Text = text;
        }
        public void setValue(bool value)
        {
            cb.Checked = value;
        }
        public bool getValue()
        {
            return cb.Checked;
        }
        public void setEnabled(bool value)
        {
            cb.Enabled = value;
        }
    }
}
