using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PriceTag.App.Interface
{
    public static class TitleBarColor
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(nint hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

        public const int DWMWA_CAPTION_COLOR = 35;

        public static void SetTitleBarColor(Form form, Color color)
        {
            int colorValue = color.R | color.G << 8 | color.B << 16;
            DwmSetWindowAttribute(form.Handle, DWMWA_CAPTION_COLOR, ref colorValue, sizeof(int));
        }
    }

    public static class TitleBarHelperExtensions
    {
        public static void SetTitleBarColor(this Form form, Color color)
            => TitleBarColor.SetTitleBarColor(form, color);
    }
}
