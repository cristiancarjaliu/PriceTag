using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTag.App.Services
{
    public static class TextHelper
    {
        public static string TruncateText(Graphics g, string text, Font font, float maxWidth)
        {
            string txt = text ?? "";
            SizeF size = g.MeasureString(txt, font);
            if (size.Width <= maxWidth)
                return txt;
            string ellipsis = "...";
            while (g.MeasureString(txt + ellipsis, font).Width > maxWidth && txt.Length > 0)
                txt = txt.Substring(0, txt.Length - 1);
            return txt + ellipsis;
        }
        public static string TruncateText(XGraphics gfx, string text, XFont font, double maxWidth)
        {
            string txt = text ?? "";
            XSize size = gfx.MeasureString(txt, font);
            if (size.Width <= maxWidth)
                return txt;
            string ellipsis = "...";
            while (gfx.MeasureString(txt + ellipsis, font).Width > maxWidth && txt.Length > 0)
                txt = txt.Substring(0, txt.Length - 1);
            return txt + ellipsis;
        }
    }
}
