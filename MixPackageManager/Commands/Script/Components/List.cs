using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MixMods.MixPackageManager.Commands.Script
{
    public class List
    {
        private System.Windows.Forms.ListBox lb;
        public List(System.Windows.Forms.ListBox lb)
        {
            this.lb = lb;
        }
        public void setItems(string[] items)
        {
            lb.Items.Clear();
            lb.Items.AddRange(items);
        }
        public int getSelectedIndex()
        {
            return lb.SelectedIndex;
        }
        public void setOnSelect(Action<int> onSelect)
        {
            lb.SelectedIndexChanged += new EventHandler((sender, e) => onSelect(lb.SelectedIndex));
        }
    }
}
