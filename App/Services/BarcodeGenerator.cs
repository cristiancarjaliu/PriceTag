using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTag.App.Services
{
    public static class BarcodeGenerator
    {
        public static Bitmap GenerateCode39Barcode(string data, int height)
        {
            data = "*" + data.ToUpper() + "*";
            Dictionary<char, string> code39 = new()
            {
                {'0', "nnnwwnwnn"},{'1', "wnnwnnnnw"},{'2', "nnwwnnnnw"},{'3', "wnwwnnnnn"},{'4', "nnnwwnnnw"},
                {'5', "wnnwwnnnn"},{'6', "nnwwwnnnn"},{'7', "nnnwnnwnw"},{'8', "wnnwnnwnn"},{'9', "nnwwnnwnn"},
                {'A', "wnnnnwnnw"},{'B', "nnwnnwnnw"},{'C', "wnwnnwnnn"},{'D', "nnnnwwnnw"},{'E', "wnnnwwnnn"},
                {'F', "nnwnwwnnn"},{'G', "nnnnnwwnw"},{'H', "wnnnnwwnn"},{'I', "nnwnnwwnn"},{'J', "nnnnwwwnn"},
                {'K', "wnnnnnnww"},{'L', "nnwnnnnww"},{'M', "wnwnnnnwn"},{'N', "nnnnwnnww"},{'O', "wnnnwnnwn"},
                {'P', "nnwnwnnwn"},{'Q', "nnnnnnwww"},{'R', "wnnnnnwwn"},{'S', "nnwnnnwwn"},{'T', "nnnnwnwwn"},
                {'U', "wwnnnnnnw"},{'V', "nwwnnnnnw"},{'W', "wwwnnnnnn"},{'X', "nwnnwnnnw"},{'Y', "wwnnwnnnn"},
                {'Z', "nwwnwnnnn"},{'-', "nwnnnnwnw"},{'.', "wwnnnnwnn"},{' ', "nwwnnnwnn"},{'$', "nwnwnwnnn"},
                {'/', "nwnwnnnwn"},{'+', "nwnnnwnwn"},{'%', "nnnwnwnwn"},{'*', "nwnnwnwnn"}
            };
            int narrowWidth = 2;
            int wideWidth = narrowWidth * 3;
            int interCharGap = narrowWidth;
            int totalWidth = 0;
            foreach (char c in data)
            {
                if (!code39.ContainsKey(c))
                    continue;
                string pattern = code39[c];
                foreach (char p in pattern)
                    totalWidth += (p == 'n' ? narrowWidth : wideWidth);
                totalWidth += interCharGap;
            }
            Bitmap bmp = new(totalWidth, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                int pos = 0;
                foreach (char c in data)
                {
                    if (!code39.ContainsKey(c))
                        continue;
                    string pattern = code39[c];
                    bool isBar = true;
                    foreach (char p in pattern)
                    {
                        int lineWidth = (p == 'n' ? narrowWidth : wideWidth);
                        if (isBar)
                            g.FillRectangle(Brushes.Black, pos, 0, lineWidth, height);
                        pos += lineWidth;
                        isBar = !isBar;
                    }
                    pos += interCharGap;
                }
            }
            return bmp;
        }
        public static XImage GenerateCode39XImage(string data)
        {
            using (Bitmap bmp = GenerateCode39Barcode(data, 25))
            using (MemoryStream ms = new())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
                return XImage.FromStream(() => new MemoryStream(ms.ToArray()));
            }
        }
    }
}