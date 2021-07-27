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
        int i = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            i += 2;
            vMeter1.ChangePos(i);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            i -= 2;
            vMeter1.ChangePos(i);
        }
    }
}
