using System;
using System.Drawing;

namespace DiscoGrid
{
    public class DiscoTile
    {
        private static Color[] Colors = new Color[] { Color.White, Color.Red, Color.Green, Color.Blue, Color.Magenta, Color.Yellow };

        private Rectangle _Rectangle;
        public Rectangle Rectangle 
        {
            get
            {
                return _Rectangle;
            }
        }

        private Color _Color = Colors[0];
        public Color Color 
        {
            get
            {
                return _Color;
            }
        }

        private bool _Highlight;
        public bool Highlight 
        { 
            get
            {
                return _Highlight;
            }
            set
            {
                _Highlight = value;
            }
        }

        public DiscoTile(int x, int y, int wid, int hgt)
        {
            _Rectangle = new Rectangle(new Point(x * wid, y * hgt), new Size(wid, hgt));
        }

        public void ChangeColor()
        {
            int colorIdx = Array.IndexOf(Colors, _Color);
            _Color = colorIdx < Colors.Length - 1 ? Colors[colorIdx + 1] : Colors[0];
        }
    }
}