using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTag.App.Interface
{
    public static class CustomButton
    {
        public static Button CreateCustomButton(string text, DialogResult result)
        {
            Button btn = new();
            btn.Text = text;
            btn.Font = new Font("Century Gothic", 10, FontStyle.Bold);
            btn.BackColor = AppColors.BgColor;
            btn.ForeColor = AppColors.AccentColor;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = AppColors.AccentColor;
            btn.FlatAppearance.BorderSize = 1;
            btn.Width = 80;
            btn.Height = 30;
            btn.DialogResult = result;
            btn.TabStop = false;
            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = AppColors.AccentColor;
                btn.ForeColor = AppColors.BgColor;
            };
            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = AppColors.BgColor;
                btn.ForeColor = AppColors.AccentColor;
            };
            return btn;
        }
    }
}
