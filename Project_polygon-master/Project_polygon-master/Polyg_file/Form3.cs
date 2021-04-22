using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Polygons
{
    public delegate void RchEventhandler(object sender, RadiusEventArgs e);

    public partial class Form3 : Form
    {
        public event RchEventhandler Rch;

        public Form3()
        {
            InitializeComponent();
            ControlBox = false;
            numericUpDown1.Value = My_Shape.r;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            int v = Convert.ToInt32(numericUpDown1.Value);
            if (Rch != null)
            {
                Rch(this, new RadiusEventArgs(v));
            }
        }
    }
    public class RadiusEventArgs : EventArgs
    {
        public int Rad { get; set; }
        public RadiusEventArgs(int r)
        {
            Rad = r;
        }

    }
}