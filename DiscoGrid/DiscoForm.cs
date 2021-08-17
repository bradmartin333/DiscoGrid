using System;
using System.Drawing;
using System.Windows.Forms;

namespace DiscoGrid
{
    public partial class DiscoForm : Form
    {
        private Size _GridSize = new Size(50, 50);
        public Size GridSize { 
            get
            {
                return _GridSize;
            }
        }

        public DiscoForm()
        {
            InitializeComponent();
            pictureBox.MouseUp += PictureBox_MouseUp;
            pictureBox.MouseMove += PictureBox_MouseMove;
            pictureBox.MouseEnter += PictureBox_MouseEnter;
            pictureBox.MouseLeave += PictureBox_MouseLeave;
        }

        private void DiscoForm_Load(object sender, EventArgs e)
        {
            pictureBox.BackgroundImage = new Bitmap(1000, 1000);
            pictureBox.Image = new Bitmap(1000, 1000);
            Functions.MakeGrid(this);
        }

        private void PictureBox_MouseLeave(object? sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void PictureBox_MouseEnter(object? sender, EventArgs e)
        {
            Cursor = Cursors.Cross;
        }

        private void PictureBox_MouseMove(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Functions.ClickDiscoTile(e.Location, this, waitForExit: true);
            Functions.HighlightDiscoTile(e.Location, this);
        }

        private void PictureBox_MouseUp(object? sender, MouseEventArgs e)
        {
            Functions.ClickDiscoTile(e.Location, this);
        }
    }
}
