using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LearningGraphicTransformation
{
    public partial class VMeter : Control
    {
        private List<Bitmap> _img_files = new List<Bitmap>();
        public event PropertyChangedEventHandler ExtraChange;
        private bool _load_extra=false;
        private float where=0;

        public bool LoadExtra
        {
            set { _load_extra = value; ExtraChange.Invoke(this, new PropertyChangedEventArgs("LoadExtra")); }
            get { return _load_extra; }
        }
        public Bitmap GetBase()
        {
            if (_img_files.Count >= 1) return _img_files[0];
            else return null;
        }
        public Bitmap GetPointer()
        {
            if (_img_files.Count >= 2) return _img_files[^1];
            else return null;
        }
        public Bitmap GetExtra()
        {
            if (_img_files.Count == 3) return _img_files[1];
            else return null;
        }
        protected override Size DefaultSize
        {
            get { return new Size(200, 200); }
        }
        public VMeter()
        {
            InitializeComponent();
            BackColor = Color.Black;
            ExtraChange = new PropertyChangedEventHandler(OnExtraChanged);
        }
        private void OnExtraChanged(object sender, PropertyChangedEventArgs e)
        {
            LoadImagesWithSpecifiedSize();
        }

        private void LoadImagesWithSpecifiedSize()
        {
            _img_files.Clear();
            try
            {
                Bitmap base_bp = new Bitmap(Properties.Resources._base);
                int sx = (int)((this.Size.Width / (float)base_bp.Width) * base_bp.Width);
                int sy = (int)((this.Size.Width / (float)base_bp.Height) * base_bp.Height);
                _img_files.Add(new Bitmap(base_bp, new Size((int)sx, (int)sy)));
                base_bp.Dispose();
                if (_load_extra)
                {
                    _img_files.Add(
                        new Bitmap(Properties.Resources.middle, new Size(sx, sy))
                        );
                }
                _img_files.Add(
                        new Bitmap(Properties.Resources.pointer, new Size(sx, sy))
                        );
            }
            catch
            {
                Console.WriteLine("Error with found images.");
            }
        }
        public void ChangePos(int v)
        {
            where = v;
            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.Size = new Size(this.Size.Width, this.Size.Width); // potem poprawić.
            LoadImagesWithSpecifiedSize();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            Rectangle clip_rect = pe.ClipRectangle;
            try
            {
                if (_img_files.Count >= 1)
                {
                    pe.Graphics.DrawImage(_img_files[0], 0,0);//Size.Width/2, Size.Height/2);
                    if (_load_extra) pe.Graphics.DrawImage(_img_files[1], 0, 0);
                    pe.Graphics.TranslateTransform(this.ClientRectangle.Width / 2, this.ClientRectangle.Height / 2);
                    pe.Graphics.RotateTransform(where);
                    pe.Graphics.DrawImage(_img_files[^1], -1 * this.ClientRectangle.Width / 2, -1 * this.ClientRectangle.Height / 2);
                }
            }
            catch
            {
            }
        }
    }
}
