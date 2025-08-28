using System;
using System.Drawing;
using System.Windows.Forms;

namespace PriceTag.App.Forms
{
    public class SplashScreenForm : Form
    {
        public SplashScreenForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.Black;
            TransparencyKey = Color.Black;
            Size = new Size(500, 400);
            Icon = new Icon("Properties\\Resources\\icon.ico");

            PictureBox logo = new PictureBox();
            logo.Image = Image.FromFile("Properties\\Resources\\logo.png");
            logo.SizeMode = PictureBoxSizeMode.Zoom;
            logo.Size = new Size(350, 250);
            logo.BackColor = Color.Transparent;
            logo.Location = new Point((Width - logo.Width) / 2, (Height - logo.Height) / 2);
            Controls.Add(logo);
        }
    }
}