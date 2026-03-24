using QRCoder;
using SkiaSharp;

namespace UrlShortener.MVC.Helpers
{
    public static class QRCodeHelper
    {
        public static string GenerateQRCodeBase64(string text, int pixelsPerModule = 10)
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);

            // ✅ Dùng PngByteQRCode - không cần System.Drawing
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(pixelsPerModule);

            return $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";
        }

        /// <summary>
        /// Generate QR Code and save to file
        /// </summary>
        public static async Task<string?> GenerateQRCodeFile(
            string text,
            string wwwRootPath,
            string fileName)
        {
            try
            {
                var qrFolder = Path.Combine(wwwRootPath, "qrcodes");
                if (!Directory.Exists(qrFolder))
                {
                    Directory.CreateDirectory(qrFolder);
                }

                var filePath = Path.Combine(qrFolder, $"{fileName}.png");

                using var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);

                // ✅ Dùng PngByteQRCode thay vì QRCode + Bitmap
                using var qrCode = new PngByteQRCode(qrCodeData);
                var qrCodeBytes = qrCode.GetGraphic(20);

                // ✅ Dùng File.WriteAllBytes thay vì Bitmap.Save
                await File.WriteAllBytesAsync(filePath, qrCodeBytes);

                return $"/qrcodes/{fileName}.png";
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}