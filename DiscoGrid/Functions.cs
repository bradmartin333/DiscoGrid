using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace DiscoGrid
{
    static class Functions
    {
        private static List<DiscoTile> DiscoTileList = new List<DiscoTile>();
        private static RectangleF InfoRectangle;

        #region Tile Iteration

        /// <summary>
        /// Populate a list of tiles
        /// </summary>
        /// <param name="form"></param>
        public static void MakeGrid(DiscoForm form)
        {
            Bitmap bitmap = (Bitmap)form.pictureBox.Image.Clone();
            Size cellSize = new Size(bitmap.Width / form.GridSize.Width, bitmap.Height / form.GridSize.Height);
            Point center = new Point(bitmap.Width / 2, bitmap.Height / 2);
            double radius = Math.Min(bitmap.Width, bitmap.Height) / 2;
            for (int i = 0; i < form.GridSize.Width; i++)
            {
                for (int j = 0; j < form.GridSize.Height; j++)
                {
                    Rectangle rectangle = new Rectangle(i * cellSize.Width, j * cellSize.Height, cellSize.Width, cellSize.Height);
                    if (CheckRectInCircle(rectangle, center, radius))
                        DiscoTileList.Add(new DiscoTile(rectangle, i, form.GridSize.Height - j - 1));
                }
            }
            DrawGrid(form);
            DrawTiles(form);
        }

        private static bool CheckRectInCircle(Rectangle rectangle, Point center, double radius)
        {
            Point[] corners = new Point[] {
                        new Point(rectangle.Left, rectangle.Top),
                        new Point(rectangle.Right, rectangle.Top),
                        new Point(rectangle.Left, rectangle.Bottom),
                        new Point(rectangle.Right, rectangle.Bottom)
                    };

            bool inCircle = true;
            foreach (Point point in corners)
            {
                double dist = Math.Sqrt(Math.Pow(point.X - center.X, 2) + Math.Pow(point.Y - center.Y, 2));
                if (dist > radius) inCircle = false;
            }
            return inCircle;
        }

        /// <summary>
        /// Toggle the highlight param for the tile under the cursor
        /// </summary>
        /// <param name="position"></param>
        /// <param name="form"></param>
        public static void HighlightDiscoTile(Point position, DiscoForm form)
        {
            Point zoomPos = ZoomMousePos(position, form);
            DiscoTileList.ForEach(x => x.Highlight = x.Rectangle.Contains(zoomPos));     
            DrawTiles(form);
        }

        /// <summary>
        /// Change the color of the tile under the cursor
        /// </summary>
        /// <param name="click"></param>
        /// <param name="form"></param>
        /// <param name="waitForExit"></param>
        public static void ClickDiscoTile(Point click, DiscoForm form, bool waitForExit = false)
        {
            Point zoomClick = ZoomMousePos(click, form);
            foreach (DiscoTile tile in DiscoTileList.Where(x => x.Rectangle.Contains(zoomClick)))
            {
                if (tile.Rectangle.Contains(zoomClick))
                {
                    if (waitForExit && tile.Highlight)
                        return;
                    tile.ChangeColor();
                }
            }
            DrawTiles(form);
        }

        #endregion

        #region Drawing

        private static void DrawGrid(DiscoForm form)
        {
            Bitmap bitmap = (Bitmap)form.pictureBox.Image.Clone();
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                foreach (DiscoTile tile in DiscoTileList)
                {
                    g.DrawRectangle(Pens.Black, tile.Rectangle);
                }
            }
            form.pictureBox.Image = bitmap;
        }

        private static void DrawTiles(DiscoForm form)
        {
            // For timing how long a process takes
            Stopwatch sw = new Stopwatch();
            sw.Restart();

            Bitmap bitmap = (Bitmap)form.pictureBox.BackgroundImage.Clone();
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // Clear last text
                g.FillRectangle(new SolidBrush(SystemColors.Control), InfoRectangle);

                DiscoTile? tileUnderCursor = null;
                foreach (DiscoTile tile in DiscoTileList.Where(x => x.NeedsUpdate || x.Highlight))
                {
                    if (tile.Highlight)
                    {
                        // Take advantage of the fact that a cell is highlighted when it changes color
                        g.FillRectangle(new SolidBrush(tile.Color), tile.Rectangle);
                        g.FillRectangle(new SolidBrush(Color.FromArgb(90, Color.Black)), tile.Rectangle);
                        tileUnderCursor = tile;
                    }
                    else
                    {
                        // Look for previously highlighted cells
                        double A = bitmap.GetPixel(tile.Rectangle.X, tile.Rectangle.Y).A;
                        if (A != 100)
                            g.FillRectangle(new SolidBrush(tile.Color), tile.Rectangle);
                    }
                }

                if (tileUnderCursor != null)
                {
                    // These options help with text drawing
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    // Draw new text
                    
                    string location = string.Format("{0}, {1}", tileUnderCursor.Location.X, tileUnderCursor.Location.Y);
                    Font font = new Font("Tahoma", 25);
                    SizeF size = g.MeasureString(location, font);
                    InfoRectangle = new RectangleF(bitmap.Width - size.Width * 1.1f, size.Height * 1.1f, size.Width, size.Height);
                    g.DrawString(location, font, Brushes.Black, InfoRectangle);
                }
            }
            form.pictureBox.BackgroundImage = bitmap;

            // Output the time for this method's duration
            sw.Stop();
            Debug.WriteLine(sw.Elapsed);
        }

        #endregion

        /// <summary>
        /// Copy paste method for adjusting mouse pos to pictureBox set to Zoom
        /// </summary>
        /// <param name="click"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        public static Point ZoomMousePos(Point click, DiscoForm form)
        {
            PictureBox pbx = form.pictureBox;
            float BackgroundImageAspect = pbx.BackgroundImage.Width / (float)pbx.BackgroundImage.Height;
            float controlAspect = pbx.Width / (float)pbx.Height;
            PointF pos = new PointF(click.X, click.Y);
            if (BackgroundImageAspect > controlAspect)
            {
                float ratioWidth = pbx.BackgroundImage.Width / (float)pbx.Width;
                pos.X *= ratioWidth;
                float scale = pbx.Width / (float)pbx.BackgroundImage.Width;
                float displayHeight = scale * pbx.BackgroundImage.Height;
                float diffHeight = pbx.Height - displayHeight;
                diffHeight /= 2;
                pos.Y -= diffHeight;
                pos.Y /= scale;
            }
            else
            {
                float ratioHeight = pbx.BackgroundImage.Height / (float)pbx.Height;
                pos.Y *= ratioHeight;
                float scale = pbx.Height / (float)pbx.BackgroundImage.Height;
                float displayWidth = scale * pbx.BackgroundImage.Width;
                float diffWidth = pbx.Width - displayWidth;
                diffWidth /= 2;
                pos.X -= diffWidth;
                pos.X /= scale;
            }
            return new Point((int)pos.X, (int)pos.Y);
        }
    }
}
