using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;

namespace LearningGraphicTransformation
{
    public partial class Form1 : Form
    {
        float i = 0;
        public Form1()
        {
            InitializeComponent();
            vMeter1.AddStartPoint(0);
            vMeter1.AddStartPoint(10);
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
            this.Run();
        }
        public void Run()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler((object o, DoWorkEventArgs dwea) => {
                for (float p = 0; ;)
                {
                    while (p < 270)
                    {
                        vMeter1.SetPosition(p);
                        Thread.Sleep(10);
                        p += 1f;
                    }
                    while (p > 0)
                    {
                        vMeter1.SetPosition(p);
                        Thread.Sleep(10);
                        p -= 1f;
                    }
                    Thread.Sleep(2000);
                }
            });
            bw.RunWorkerAsync();
        }
    }
}
