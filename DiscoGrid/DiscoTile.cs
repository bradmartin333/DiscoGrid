using System;
using System.Drawing;

namespace DiscoGrid
{
    public class DiscoTile
    {
        private static Color[] Colors = new Color[] { Color.White, Color.Red, Color.Green, Color.Blue, Color.Magenta, Color.Yellow };

        private bool _NeedsUpdate = true;
        /// <summary>
        /// Parameter that allows for reduced number of drawing iterations
        /// </summary>
        public bool NeedsUpdate
        {
            get 
            {
                if (_NeedsUpdate)
                {
                    _NeedsUpdate = false;
                    return true;
                }
                return false; 
            }
        }

        private Rectangle _Rectangle;
        public Rectangle Rectangle 
        {
            get { return _Rectangle; }
        }

        private Color _Color = Colors[0];
        public Color Color 
        {
            get { return _Color; }
        }

        private bool _LastHighlight;
        private bool _Highlight;
        public bool Highlight 
        { 
            get { return _Highlight; }
            set 
            { 
                _Highlight = value;

                if (_LastHighlight != _Highlight)
                {
                    _NeedsUpdate = true;
                    _LastHighlight = _Highlight;
                }
            }
        }

        public DiscoTile(int x, int y, int wid, int hgt)
        {
            _Rectangle = new Rectangle(new Point(x * wid, y * hgt), new Size(wid, hgt));
        }

        /// <summary>
        /// Cycle the tile's color through the DiscoTile Colors array
        /// </summary>
        public void ChangeColor()
        {
            int colorIdx = Array.IndexOf(Colors, _Color);
            _Color = colorIdx < Colors.Length - 1 ? Colors[colorIdx + 1] : Colors[0];
            _NeedsUpdate = true;
        }
    }
}