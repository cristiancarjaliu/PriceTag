using PdfSharpCore.Drawing;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceTag.App.Services
{
    public static class GenerateQRCodeHelper
    {
        public static Bitmap GenerateQRCodeBitmap(string data)
        {
            using (QRCodeGenerator qrGenerator = new())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCoder.QRCodeGenerator.ECCLevel.Q))
            using (QRCode qrCode = new(qrCodeData))
                return qrCode.GetGraphic(10, Color.Black, Color.White, true);
        }
        public static XImage GenerateQRCode(string data)
        {
            using (QRCodeGenerator qrGenerator = new())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCoder.QRCodeGenerator.ECCLevel.Q))
            using (QRCode qrCode = new(qrCodeData))
            using (Bitmap qrBitmap = qrCode.GetGraphic(10, Color.Black, Color.Transparent, true))
            using (MemoryStream ms = new())
            {
                qrBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
                return XImage.FromStream(() => new MemoryStream(ms.ToArray()));
            }
        }
    }
}
