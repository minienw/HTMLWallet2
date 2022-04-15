using System;
using System.IO;
using System.Diagnostics;
using Xunit;

using DtronixPdf;
using SixLabors.ImageSharp.Formats.Bmp;
using ZXing;

namespace CheckIn.Shared.Tests;

public class PdfParsing
{

    [Fact]
    public async void NotBob()
    {
        //var buffer = File.ReadAllBytes(Paths.SourceDocument1);
        await using var doc = await PdfDocument.Load(Paths.SourceDocument2, "");
        await using var p = await doc.GetPage(doc.Pages - 1);
        var r = new SixLabors.ImageSharp.RectangleF(30, 510, 240, 235);
        await using var pdfBitmap = await p.Render(1, SixLabors.ImageSharp.Color.White, r, PDFiumCore.RenderFlags.DisableImageAntialiasing | PDFiumCore.RenderFlags.DisablePathAntialiasing);

        await using (var ms = new MemoryStream())
        {
            pdfBitmap.Image.Save(ms, new BmpEncoder()); //Windows image

            //File.WriteAllBytes("D:\\1 - Page.bmp", ms.ToArray());
            //Top left 30,510 Bottom right 265,743
            //var bmpImage = new Bitmap(ms);
            //var cropRect = new Rectangle(30, 510, 240, 235);
            //var cropped = bmpImage.Clone(cropRect, bmpImage.PixelFormat);
            //cropped.Save("D:\\1 - Page.bmp");
            //cropped.Save("D:\\2 - QR.bmp", ImageFormat.Bmp);
            //using var resultStream = new MemoryStream();
            //{
            //    cropped.Save(resultStream, ImageFormat.Bmp);
            //var args = new SkiaSharp.SKImageInfo();
            //args.Width = pdfBitmap.Image.Width;
            //args.Height = pdfBitmap.Image.Height;
            //args.AlphaType = SkiaSharp.SKAlphaType.Opaque;

            //var skBitmap = SkiaSharp.SKBitmap.Decode(ms, );
            //Assert.NotNull(skBitmap);
            //}

            var skiaReader = new ZXing.SkiaSharp.BarcodeReader();

            Result? skiaResult = null;

            //var skiaResult = skiaReader.Decode(skBitmap);
            foreach (RGBLuminanceSource.BitmapFormat i in Enum.GetValues(typeof(RGBLuminanceSource.BitmapFormat)))
            {
                try
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    skiaResult = skiaReader.Decode(ms.ToArray(), (int)r.Width, (int)r.Height, i);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
            }

            Assert.True(skiaResult != null);

            //var decoded = skiaResult?.Text;
            //Assert.True(decoded.StartsWith("HC1"));
        }



        //var _document = fpdfview.FPDF_LoadMemDocument(new IntPtr(buffer), null, null);
        //Assert.Equal(1, fpdfview.FPDF_GetPageCount(_document));
    }
}
