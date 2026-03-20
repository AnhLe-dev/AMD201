using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace UrlShortener.MVC.Helpers
{
    public static class QRCodeHelper
    {
        /// <summary>
        /// Generate QR Code as Base64 string
        /// </summary>
        public static string GenerateQRCodeBase64(string text, int pixelsPerModule = 10)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    using (Bitmap qrCodeImage = qrCode.GetGraphic(pixelsPerModule))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            qrCodeImage.Save(ms, ImageFormat.Png);
                            byte[] byteImage = ms.ToArray();
                            string base64String = Convert.ToBase64String(byteImage);
                            return $"data:image/png;base64,{base64String}";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generate QR Code and save to file
        /// </summary>
        public static async Task<string?> GenerateQRCodeFile(string text, string wwwRootPath, string fileName)
        {
            try
            {
                var qrFolder = Path.Combine(wwwRootPath, "qrcodes");
                if (!Directory.Exists(qrFolder))
                {
                    Directory.CreateDirectory(qrFolder);
                }

                var filePath = Path.Combine(qrFolder, $"{fileName}.png");

                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                {
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                    using (QRCode qrCode = new QRCode(qrCodeData))
                    {
                        using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
                        {
                            qrCodeImage.Save(filePath, ImageFormat.Png);
                        }
                    }
                }

                return $"/qrcodes/{fileName}.png";
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
