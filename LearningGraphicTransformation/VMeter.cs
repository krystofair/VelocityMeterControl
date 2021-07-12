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
        private string _location_files_dir_path = "";
        private static readonly string[] _img_names = { "base.png", "middle.png", "pointer.png" };
        private List<Bitmap> _img_files = new List<Bitmap>();
        public event PropertyChangedEventHandler LocationDirChange;
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

        public string LocationDir
        {
            set {
                if (value != "")
                {
                    _location_files_dir_path = value;
                    LocationDirChange.Invoke(this, new PropertyChangedEventArgs("LocationDir"));
                }
            }
            get { return _location_files_dir_path; }
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
            LocationDirChange = new PropertyChangedEventHandler(OnLocationDirChanged);
        }
        private void OnExtraChanged(object sender, PropertyChangedEventArgs e)
        {
            OnLocationDirChanged(sender, e);
        }

        private void OnLocationDirChanged(object sender, PropertyChangedEventArgs e)
        {
            _img_files.Clear();
            try
            {
                string path = _location_files_dir_path;
                if (!(_location_files_dir_path.EndsWith('\\')
                    || _location_files_dir_path.EndsWith('/')))
                    path += "\\";
                Bitmap base_bp = new Bitmap(Image.FromFile(path + _img_names[0]));
                float sx = (this.Size.Width / (float)base_bp.Width) * base_bp.Width;
                float sy = (this.Size.Width / (float)base_bp.Height) * base_bp.Height;
                _img_files.Add(new Bitmap(base_bp, new Size((int)sx, (int)sy)));
                base_bp.Dispose();
                if (_load_extra)
                {
                    _img_files.Add(
                        new Bitmap(Image.FromFile(path + _img_names[1]), new Size((int)sx, (int)sy))
                        );
                }
                _img_files.Add(
                        new Bitmap(Image.FromFile(path + _img_names[2]), new Size((int)sx, (int)sy))
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
            OnLocationDirChanged(this, new PropertyChangedEventArgs("LocationDir"));
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            if(_location_files_dir_path != "")
            {
                try
                {
                    if (_img_files.Count >= 1)
                    {
                        pe.Graphics.DrawImage(_img_files[0], 0, 0);//Size.Width/2, Size.Height/2);
                        if(_load_extra) pe.Graphics.DrawImage(_img_files[1], 0, 0);
                        pe.Graphics.TranslateTransform(this.ClientRectangle.Width/2, this.ClientRectangle.Height/2);
                        pe.Graphics.RotateTransform(where);
                        pe.Graphics.DrawImage(_img_files[^1], -1*this.ClientRectangle.Width / 2, -1*this.ClientRectangle.Height / 2);
                    }
                }
                catch
                {   
                }
            }
        }
    }
}
