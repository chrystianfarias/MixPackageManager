using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MixMods.MixPackageManager.Commands.Script
{
    public class Button
    {
        private System.Windows.Forms.Button bt;
        public Button(System.Windows.Forms.Button bt)
        {
            this.bt = bt;
        }
        public void setText(string text)
        {
            bt.Text = text;
            bt.Refresh();
        }
        public void setEnabled(bool value)
        {
            bt.Enabled = value;
        }
    }
}
