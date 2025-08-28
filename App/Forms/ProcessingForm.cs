using PriceTag.App.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PriceTag.App.Forms
{
    public partial class ProcessingForm : Form
    {
        private Label lblMessage;
        public ProcessingForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = AppColors.BgColor;
            this.Size = new Size(300, 100);
            lblMessage = new Label
            {
                Text = "Fisierul este in curs de prelucrare...",
                Font = new Font("Century Gothic", 10, FontStyle.Bold),
                ForeColor = AppColors.AccentColor,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            this.Controls.Add(lblMessage);
        }
    }
}
