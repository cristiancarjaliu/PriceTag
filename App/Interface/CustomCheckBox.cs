using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTag.App.Interface
{
    public class CustomCheckBox : CheckBox
    {
        public CustomCheckBox()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.FlatStyle = FlatStyle.Flat;
            this.BackColor = AppColors.BgColor;
            this.ForeColor = AppColors.AccentColor;
        }
        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.Clear(this.BackColor);
            int boxSize = 16;
            Rectangle boxRect = new(0, (this.Height - boxSize) / 2, boxSize, boxSize);
            using (Pen pen = new(this.ForeColor, 1))
                g.DrawRectangle(pen, boxRect);
            if (this.Checked)
            {
                using (Pen pen = new(this.ForeColor, 2))
                {
                    g.DrawLine(pen, boxRect.Left + 3, boxRect.Top + boxSize / 2, boxRect.Left + boxSize / 2, boxRect.Bottom - 3);
                    g.DrawLine(pen, boxRect.Left + boxSize / 2, boxRect.Bottom - 3, boxRect.Right - 3, boxRect.Top + 3);
                }
            }
            TextRenderer.DrawText(g, this.Text, this.Font, new Point(boxRect.Right + 5, (this.Height - this.Font.Height) / 2), this.ForeColor);
        }
    }
}
