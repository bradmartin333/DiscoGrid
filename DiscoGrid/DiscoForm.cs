using System;
using System.Drawing;
using System.Windows.Forms;

namespace DiscoGrid
{
    public partial class DiscoForm : Form
    {
        private Size _GridSize = new Size(10, 10);
        public Size GridSize { 
            get
            {
                return _GridSize;
            }
            set
            {
                _GridSize = value;
            }
        }

        public DiscoForm()
        {
            InitializeComponent();
            pictureBox.MouseUp += PictureBox_MouseUp;
            pictureBox.MouseMove += PictureBox_MouseMove;
        }

        private void DiscoForm_Load(object sender, EventArgs e)
        {
            pictureBox.BackgroundImage = new Bitmap(450, 450);
            pictureBox.Image = new Bitmap(450, 450);
            Functions.MakeGrid(this);
        }

        private void PictureBox_MouseMove(object? sender, MouseEventArgs e)
        {
            Functions.HighlightDiscoTile(e.Location, this);
        }

        private void PictureBox_MouseUp(object? sender, MouseEventArgs e)
        {
            Functions.ClickDiscoTile(e.Location, this);
        }       
    }
}
