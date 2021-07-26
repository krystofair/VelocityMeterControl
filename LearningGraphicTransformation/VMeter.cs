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
        /* Function which optimize size of needed bitmap 
         * Here is process of deleting alpha channel from image. */
        protected void ChangeBMPToNoAlpha(ref Bitmap b)
        {
            // Pixel to obtain color from.
            Color pix_col;
            // Rectangle to specify area of pointer;
            Rectangle rec;
            // Checking if bitmap has a properly format.
            // It has to be 32bppArgb, cause we're searching transparency value.
            if (b.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                // Algorithm depends on finding left top, and right bottom corner of "arrow".
                bool new_scanline = false;
                Point p_top = new Point(b.Width, -1);
                Point p_bot = new Point(-1, -1);
                for (int r = 0; r < b.Height; ++r)
                {
                    new_scanline = true;
                    for (int c = 0; c < b.Width; ++c)
                    {
                        pix_col = b.GetPixel(c, r);
                        if (pix_col.A < 128)
                        {
                            if (new_scanline)
                            {
                                if (p_top.Y == -1) p_top.Y = r;
                                if (c < p_top.X) p_top.X = c;
                                new_scanline = false;
                            }
                        }
                        else
                        {
                            if (!new_scanline)
                            {
                                if (p_bot.Y < r) p_bot.Y = r;
                                if (p_bot.X < c) p_bot.X = c;
                            }
                        }
                    }
                }
                //pointer_position = p_top;
                rec = new Rectangle(p_top, new Size(p_bot.X - p_top.X, p_bot.Y - p_top.Y));
                if (rec.Width == 0 || rec.Height == 0)
                    throw new Exception("It doesn't find pointer, change value of boundary searching alpha or " +
                        "modify original bitmap of pointer file.");
                b = b.Clone(rec, b.PixelFormat);
            }
            else
            {
                throw new Exception("File of pointer should be 32bpp format.");
            }
        }
    }
}
