using System;
using System.Drawing;
using System.Windows.Forms;

namespace DiscoGrid
{
    static class Functions
    {

        private static DiscoTile[,] DiscoTiles = new DiscoTile[0,0];

        #region Tile Iteration

        public static void MakeGrid(DiscoForm form)
        {
            DiscoTiles = new DiscoTile[form.GridSize.Width, form.GridSize.Height];
            Bitmap bitmap = (Bitmap)form.pictureBox.Image.Clone();
            Size cellSize = new Size(bitmap.Width / form.GridSize.Width, bitmap.Height / form.GridSize.Height);
            for (int i = 0; i < form.GridSize.Width; i++)
            {
                for (int j = 0; j < form.GridSize.Height; j++)
                {
                    DiscoTiles[i, j] = new DiscoTile(i, j, cellSize.Width, cellSize.Height);
                }
            }
            DrawGrid(form);
        }

        public static void HighlightDiscoTile(Point position, DiscoForm form)
        {
            Point zoomPos = ZoomMousePos(position, form);
            foreach (DiscoTile tile in DiscoTiles)
            {
                if (tile.Rectangle.Contains(zoomPos))
                {
                    tile.Highlight = true;
                }
                else
                {
                    tile.Highlight = false;
                }
            }
            DrawTiles(form);
        }

        public static void ClickDiscoTile(Point click, DiscoForm form)
        {
            Point zoomClick = ZoomMousePos(click, form);
            foreach (DiscoTile tile in DiscoTiles)
            {
                if (tile.Rectangle.Contains(zoomClick))
                {
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
                foreach (DiscoTile tile in DiscoTiles)
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
            Bitmap bitmap = (Bitmap)form.pictureBox.BackgroundImage.Clone();
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                foreach (DiscoTile tile in DiscoTiles)
                {
                    Color color = bitmap.GetPixel(tile.Rectangle.X, tile.Rectangle.Y);

                    if (tile.Highlight)
                    {
                        g.FillRectangle(new SolidBrush(tile.Color), tile.Rectangle);
                        g.FillRectangle(new SolidBrush(Color.FromArgb(90, Color.Black)), tile.Rectangle);
                    }
                    else if (color != tile.Color)
                    {
                        g.FillRectangle(new SolidBrush(tile.Color), tile.Rectangle);
                    }
                }
            }
            form.pictureBox.BackgroundImage = bitmap;
        }

        #endregion

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
