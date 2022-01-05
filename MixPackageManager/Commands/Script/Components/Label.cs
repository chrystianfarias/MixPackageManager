using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MixMods.MixPackageManager.Commands.Script
{
    public class Label
    {
        private System.Windows.Forms.Label lbl;

        public Label(System.Windows.Forms.Label lbl)
        {
            this.lbl = lbl;
        }

        public void setText(string text)
        {
            lbl.Text = text;
        }
    }
}
