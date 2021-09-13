using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;

namespace LearningGraphicTransformation
{
    /*
      * TODO:
      * 1. Interface to deleting elements from ranges_for_extra list.
      * 2. Adding element from Design view.
      * 3. Thread safety.
      * 4. Add timer to count how long extra should be visibled.
      */
    public partial class VMeter : Control
    {
        private List<Bitmap> _img_files = new List<Bitmap>();
        public event PropertyChangedEventHandler ExtraChange;
        private bool is_extra_drawn = false;
        private bool load_extra = false;
        private Point ptr_pos;
        private Point middle_pos;
        private Rectangle OldRectangle;
        private float where = 0;
        private List<float> start_extra = null;
        private int steps_counter = 0;
        readonly private int STEPS = 10;
        readonly private float eps = 0.01f;


        public void SetPosition(float value)
        {
            Invalidate(OldRectangle);
            where = value % 360;
            OldRectangle = CalcDrawingAreaForPointer();
        }
        
        public float GetPosition()
        {
            return where;
        }

        /**
         * Add where value to list in order to specify when the extra (middle) image will be shown.
         */
        public void AddStartPoint(float point)
        {
            if (start_extra == null)
                throw new Exception("The range object doesn't exist.");
            else
                start_extra.Add(point);
        }

        /**
         * Field for being visible in Design in 'Misc' section.
         */
        public bool LoadExtra
        {
            set { load_extra = value; ExtraChange.Invoke(this, new PropertyChangedEventArgs("LoadExtra")); }
            get { return load_extra; }
        }

        /**
         * Default size when you drag and drop this control at form.
         */
        protected override Size DefaultSize
        {
            get { return new Size(200, 200); }
        }

        public VMeter()
        {
            InitializeComponent();
            BackColor = Color.Black;
            ExtraChange = new PropertyChangedEventHandler(OnExtraChanged);
            AddStartPoint(0);
            AddStartPoint(10);
        }

        [DesignOnly(true)]
        private void OnExtraChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!LoadExtra)
            {
                if (_img_files.Count > 2)
                    _img_files.RemoveAt(1);
                start_extra = null;
            }
            else
            {
                if (_img_files.Count >= 1 && _img_files.Count <= 2)
                {
                    int sx = (int)((this.Size.Width / (float)_img_files[0].Width) * _img_files[0].Width);
                    int sy = (int)((this.Size.Width / (float)_img_files[0].Height) * _img_files[0].Height);
                    Bitmap extra_bmp = new Bitmap(Properties.Resources.middle, new Size(sx, sy));
                    middle_pos = CropBMPToNoAlpha(ref extra_bmp);
                    _img_files.Insert(1, extra_bmp);
                    start_extra = new List<float>();
                }
            }
        }

        [DesignOnly(true)]
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
                base_bp = new Bitmap(Properties.Resources.pointer, new Size(sx, sy));
                ptr_pos = CropBMPToNoAlpha(ref base_bp);
                _img_files.Add(base_bp);
            }
            catch (Exception e)
            {
                Debug.WriteLine("[DEBUG] " + e.Message);
            }
        }

        protected Rectangle CalcDrawingAreaForPointer()
        {
            if (_img_files.Count < 2) return ClientRectangle;
            Rectangle re = new Rectangle(); // clip rectangle to return.
            re.Width = _img_files[^1].Width;
            re.Height = _img_files[^1].Height;
            Point[] points = {
                new Point(ptr_pos.X, ptr_pos.Y),
                new Point(ptr_pos.X, ptr_pos.Y+re.Height),
                new Point(ptr_pos.X+re.Width, ptr_pos.Y),
                new Point(ptr_pos.X+re.Width, ptr_pos.Y+re.Height)
            };
            Matrix m = new Matrix();
            m.RotateAt(this.where, new Point(ClientRectangle.Width / 2, ClientRectangle.Height / 2));
            m.TransformPoints(points);
            m.Dispose();
            List<Point> l = new List<Point>(points); // list of points to easy sort.
            
            l.Sort((b,a) => { return a.X - b.X; }); // sort by Xs.
            re.Width = l[^1].X - l[0].X; // max width of new rectangle.
            re.X = l[0].X;
            l.Sort((b, a) => { return a.Y - b.Y; }); // sort by Ys.
            re.Y = l[0].Y;
            re.Height = l[^1].Y - l[0].Y; // max height of new rectangle.
            return re;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.Size = new Size(this.Size.Width, this.Size.Width); // potem poprawić.
            LoadImagesWithSpecifiedSize();
            OnExtraChanged(this, null);
            OldRectangle = new Rectangle(ptr_pos, new Size(_img_files[^1].Width, _img_files[^1].Height));
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            try
            {
                if (_img_files.Count >= 1)
                {
                    // Here fragment of the shield is being drawn, which was passed to invalidate method.
                    pe.Graphics.DrawImage(
                        _img_files[0].Clone(pe.ClipRectangle, _img_files[0].PixelFormat),
                        pe.ClipRectangle.Left, pe.ClipRectangle.Top
                    );
                    if (LoadExtra)
                    {
                        foreach (int i in start_extra)
                            if (Math.Abs(where - i) < 0.001)
                                steps_counter = STEPS;
                        if (steps_counter == 0 && is_extra_drawn)
                        {
                            // redrawing area where extra image is.
                            Rectangle r = new Rectangle(middle_pos.X, middle_pos.Y,
                                _img_files[1].Width, _img_files[1].Height
                            );
                            pe.Graphics.DrawImage(
                                   _img_files[0].Clone(r, _img_files[0].PixelFormat),
                                   r.Left, r.Right
                               );
                            is_extra_drawn = false;
                        }
                        else
                        {
                            if (!is_extra_drawn)
                            {
                                pe.Graphics.DrawImage(_img_files[1], middle_pos.X, middle_pos.Y);
                                is_extra_drawn = true;
                            }
                            steps_counter -= 1;
                        }
                    }
                    // Move image for new origin
                    pe.Graphics.TranslateTransform(ClientRectangle.Width / 2, ClientRectangle.Height / 2);
                    // Rotate image in it
                    pe.Graphics.RotateTransform(where);
                    // Draw image on old position, that's why there are set new coords.
                    pe.Graphics.DrawImage(_img_files[^1], ptr_pos.X - ClientRectangle.Width / 2, ptr_pos.Y - ClientRectangle.Height / 2);
                }
            }
            catch { }
            finally
            {
                pe.Graphics.Dispose();
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
