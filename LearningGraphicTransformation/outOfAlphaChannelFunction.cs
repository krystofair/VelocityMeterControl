/* Function which optimize size of needed bitmap
         * this receive reference and return reference, because
         * there was an exception "Out of memory".
         */
        protected ref Bitmap ChangeBMPToNoAlpha(ref Bitmap b)
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
                pointer_position = p_top;
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
            return ref b;
        }