using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DiscoGrid
{
    static class Functions
    {
        private static List<DiscoTile> DiscoTileList = new List<DiscoTile>();

        #region Tile Iteration

        /// <summary>
        /// Populate a list of tiles
        /// </summary>
        /// <param name="form"></param>
        public static void MakeGrid(DiscoForm form)
        {
            Bitmap bitmap = (Bitmap)form.pictureBox.Image.Clone();
            Size cellSize = new Size(bitmap.Width / form.GridSize.Width, bitmap.Height / form.GridSize.Height);
            for (int i = 0; i < form.GridSize.Width; i++)
            {
                for (int j = 0; j < form.GridSize.Height; j++)
                {
                    DiscoTileList.Add(new DiscoTile(i, j, cellSize.Width, cellSize.Height));
                }
            }
            DrawGrid(form);
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
                foreach (DiscoTile tile in DiscoTileList.Where(x => x.NeedsUpdate))
                {
                    g.DrawRectangle(
                        new Pen(new SolidBrush(Color.Black), 1), 
                        tile.Rectangle
                    );
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
                foreach (DiscoTile tile in DiscoTileList.Where(x => x.NeedsUpdate))
                {
                    if (tile.Highlight) 
                    {
                        // Take advantage of the fact that a cell is highlighted when it changes color
                        g.FillRectangle(new SolidBrush(tile.Color), tile.Rectangle);
                        g.FillRectangle(new SolidBrush(Color.FromArgb(90, Color.Black)), tile.Rectangle);
                    }
                    else
                    {
                        // Look for previously highlighted cells
                        double A = bitmap.GetPixel(tile.Rectangle.X, tile.Rectangle.Y).A;
                        if (A != 0 && A != 100)
                            g.FillRectangle(new SolidBrush(tile.Color), tile.Rectangle);
                    }
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
