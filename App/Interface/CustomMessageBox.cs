using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTag.App.Interface
{
    public static class CustomMessageBox
    {
        public static DialogResult Show(string message)
        {
            return Show(message, "Atentie!", MessageBoxButtons.OK);
        }
        public static DialogResult Show(string message, string title, MessageBoxButtons buttons)
        {
            Form msgForm = new();
            msgForm.StartPosition = FormStartPosition.CenterScreen;
            msgForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            msgForm.BackColor = AppColors.BgColor;
            msgForm.ClientSize = new Size(400, 150);
            msgForm.Text = "";
            msgForm.CreateControl();
            msgForm.SetTitleBarColor(AppColors.BgColor);
            Label headerLabel = new();
            headerLabel.Text = "Atentie!";
            headerLabel.Font = new Font("Century Gothic", 14, FontStyle.Bold);
            headerLabel.ForeColor = AppColors.AccentColor;
            headerLabel.BackColor = AppColors.BgColor;
            headerLabel.AutoSize = false;
            headerLabel.Size = new Size(msgForm.ClientSize.Width, 40);
            headerLabel.TextAlign = ContentAlignment.MiddleCenter;
            headerLabel.Dock = DockStyle.Top;
            headerLabel.Padding = new Padding(0, 20, 0, 0);
            msgForm.Controls.Add(headerLabel);
            Label lbl = new();
            lbl.Text = message;
            lbl.Font = new Font("Century Gothic", 10, FontStyle.Bold);
            lbl.ForeColor = AppColors.AccentColor;
            lbl.BackColor = AppColors.BgColor;
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            lbl.Dock = DockStyle.Fill;
            lbl.Padding = new Padding(10, headerLabel.Height + 15, 10, 10);
            msgForm.Controls.Add(lbl);
            FlowLayoutPanel panel = new();
            panel.FlowDirection = FlowDirection.RightToLeft;
            panel.Dock = DockStyle.Bottom;
            panel.Height = 40;
            panel.Padding = new Padding(0, 0, 10, 10);
            Button btnYes = CustomButton.CreateCustomButton("Da", DialogResult.Yes);
            Button btnNo = CustomButton.CreateCustomButton("Nu", DialogResult.No);
            Button btnOK = CustomButton.CreateCustomButton("OK", DialogResult.OK);
            if (buttons == MessageBoxButtons.YesNo)
            {
                panel.Controls.Add(btnNo);
                panel.Controls.Add(btnYes);
            }
            else
                panel.Controls.Add(btnOK);
            msgForm.Controls.Add(panel);
            msgForm.AcceptButton = (buttons == MessageBoxButtons.YesNo) ? btnYes : btnOK;
            msgForm.FormClosing += (s, e) => { if (msgForm.DialogResult == DialogResult.None) msgForm.DialogResult = DialogResult.No; };
            DialogResult result = msgForm.ShowDialog();
            return result;
        }
    }
}
