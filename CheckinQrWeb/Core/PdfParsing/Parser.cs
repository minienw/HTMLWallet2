using System.Drawing;
using Patagames.Pdf.Enums;
using Patagames.Pdf.Net;

namespace CheckInQrWeb.Core.PdfParsing
{
    public class Parser
    {
        public Image Render(Stream buffer)
        {
            using (var doc = PdfDocument.Load(buffer)) // C# Read PDF Document
            {
                var page = doc.Pages.Last();

                int width = (int)(page.Width / 72.0 * 96);
                int height = (int)(page.Height / 72.0 * 96);
                using (var bitmap = new PdfBitmap(width, height, true))
                {
                    bitmap.FillRect(0, 0, width, height, Patagames.Pdf.FS_COLOR.White);
                    page.Render(bitmap, 0, 0, width, height, PageRotate.Normal, RenderFlags.FPDF_LCD_TEXT);
                    return bitmap.Image;
                    //bitmap.Image.Save("D:\\Arse.png", ImageFormat.Png);
                }
            }
        }
    }
}
