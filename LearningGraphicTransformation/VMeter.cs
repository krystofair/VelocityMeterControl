using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace LearningGraphicTransformation
{
    public partial class VMeter : Control
    {
        private List<Bitmap> _img_files = new List<Bitmap>();
        public event PropertyChangedEventHandler ExtraChange;
        private bool _load_extra = false;
        private Point ptr_pos;
        private Point middle_pos;
        private float where = 0;

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
                    base_bp = new Bitmap(Properties.Resources.middle, new Size(sx, sy));
                    middle_pos = CropBMPToNoAlpha(ref base_bp);
                    _img_files.Add(base_bp);
                }
                base_bp = new Bitmap(Properties.Resources.pointer, new Size(sx, sy));
                ptr_pos = CropBMPToNoAlpha(ref base_bp);
                _img_files.Add(base_bp);
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG] " + e.Message);
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
                    pe.Graphics.DrawImage(_img_files[0], 0, 0);//Size.Width/2, Size.Height/2);
                    if (_load_extra) pe.Graphics.DrawImage(_img_files[1], middle_pos.X, middle_pos.Y);
                    pe.Graphics.TranslateTransform(this.ClientRectangle.Width / 2, this.ClientRectangle.Height / 2);
                    pe.Graphics.RotateTransform(where);
                    pe.Graphics.DrawImage(_img_files[^1], - this.ClientRectangle.Width / 2 + ptr_pos.X, -this.ClientRectangle.Height / 2 + ptr_pos.Y);
                }
            }
            catch
            {
            }
        }
        /* Function which optimize size of needed bitmap 
         * Here is process of deleting alpha channel from image.
         * The pointer cannot be too thin */
        public Point CropBMPToNoAlpha(ref Bitmap bmp, int alpha_boundary = 64)
        {
            if (alpha_boundary > 254 || alpha_boundary < 16) return new Point(-1, -1);
            // Rectangle to specify area of pointer;
            Rectangle rec;
            // Checking if bitmap has a properly format.
            // It has to be 32bppArgb, cause we're searching transparency value
            if (bmp.PixelFormat == PixelFormat.Format16bppArgb1555 || bmp.PixelFormat == PixelFormat.Format32bppArgb
                || bmp.PixelFormat == PixelFormat.Format32bppPArgb|| bmp.PixelFormat == PixelFormat.Format64bppArgb
                || bmp.PixelFormat == PixelFormat.Format64bppPArgb ) {
                    try {
                    int top_x = bmp.Width, top_y = bmp.Height, bot_x = 0, bot_y = 0;
                    for (int x = 0; x < bmp.Width; ++x)
                    {
                        // creating list of pixels of one column
                        for (int y = 0; y < bmp.Height; ++y)
                        {
                            // check boundary condition
                            if (bmp.GetPixel(x, y).A > alpha_boundary)
                            {
                                if (top_y > y)
                                    top_y = y;
                                if (bot_y < y)
                                    bot_y = y;
                            }
                        }
                    }
                    // scanning now for rows but with boundary, which are counted above.
                    for (int y = top_y; y < bot_y; ++y)
                    {
                        // creating list of pixels of one column
                        for (int x = 0; x < bmp.Height; ++x)
                        {
                            // check boundary condition
                            if (bmp.GetPixel(x, y).A > alpha_boundary) // noalpha_pixels.Add(new Point(x, y));
                            {
                                if (top_x > x)
                                    top_x = x;
                                if (bot_x < x)
                                    bot_x = x;
                            }
                        }
                    }
                    var p_top = new Point(top_x, top_y);
                    var p_bot = new Point(bot_x, bot_y);
                    rec = new Rectangle(p_top, new Size(p_bot.X - p_top.X, p_bot.Y - p_top.Y));
                    if (rec.Width <= 0 || rec.Height <= 0)
                    {
                        Debug.WriteLine("[ERROR] It doesn't find pointer, change value of boundary searching alpha or " +
                            "modify original bitmap of pointer file.");
                    }
                    else
                    {
                        bmp = bmp.Clone(rec, bmp.PixelFormat);
                    }
                    return p_top;
                }
                catch (Exception e) { Debug.WriteLine(e.Message); }
            }
            else
            {
                throw new Exception("File of pointer must be in format with Alpha");
            }
            return new Point(-1, -1);
        }
    }
}
