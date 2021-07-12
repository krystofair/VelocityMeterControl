
namespace LearningGraphicTransformation
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.vMeter1 = new LearningGraphicTransformation.VMeter();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // vMeter1
            // 
            this.vMeter1.BackColor = System.Drawing.Color.Black;
            this.vMeter1.LoadExtra = true;
            this.vMeter1.Location = new System.Drawing.Point(44, 34);
            this.vMeter1.LocationDir = "D:\\LearningGraphicTransformation\\LearningGraphicTransformation\\vmet-mat";
            this.vMeter1.Name = "vMeter1";
            this.vMeter1.Size = new System.Drawing.Size(556, 556);
            this.vMeter1.TabIndex = 0;
            this.vMeter1.Text = "vMeter1";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(680, 95);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(82, 54);
            this.button1.TabIndex = 1;
            this.button1.Text = "w góre";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(680, 179);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(82, 54);
            this.button2.TabIndex = 2;
            this.button2.Text = "w dół";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 661);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.vMeter1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private VMeter vMeter1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}

