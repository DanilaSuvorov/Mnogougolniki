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
    public partial class Form2 : Form
    {
        public int Data { get { return trackBar1.Value; } }

        public Form2()
        {
            InitializeComponent();
            ControlBox = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 main = Owner as Form1;
            main.Invalidate();
            ActiveForm.Close();
        }
    }
}
