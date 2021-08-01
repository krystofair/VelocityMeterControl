using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace LearningGraphicTransformation
{
    public partial class Form1 : Form
    {
        float i = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            i += 1f;
            vMeter1.SetPosition(i);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            i -= 1f;
            vMeter1.SetPosition(i);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            vMeter1.Invalidate();
        }
    }
}
