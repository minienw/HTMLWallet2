using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using CheckInQrWeb.Core;
//using Patagames.Pdf.Enums;
//using Patagames.Pdf.Net;
using Xunit;

namespace CheckIn.Shared.Tests
{
    public class PdfParsing
    {
        public Image Render()
        {
            throw new System.Exception();
            //using (var doc = PdfDocument.Load( Paths.SourceDocument1)) // C# Read PDF Document
            //{
            //    var page = doc.Pages.Last();

            //    int width = (int)(page.Width / 72.0 * 96);
            //    int height = (int)(page.Height / 72.0 * 96);
            //    using (var bitmap = new PdfBitmap(width, height, true))
            //    {
            //        bitmap.FillRect(0, 0, width, height, Patagames.Pdf.FS_COLOR.White);
            //        page.Render(bitmap, 0, 0, width, height, PageRotate.Normal, RenderFlags.FPDF_LCD_TEXT);
            //        return bitmap.Image;
            //        //bitmap.Image.Save("D:\\Arse.png", ImageFormat.Png);
            //    }
            //}
        }
    }
}

//        int depth = 0;

        //public void Dump(PdfObject obj)
        //{
        //    try
        //    {
        //        depth++;
        //        if (depth > 10)
        //        {
        //            Trace.WriteLine("Too Deep");
        //            return;
        //        }
        //        if (obj.GetObjectType() == PdfObject.STREAM)
        //        {
        //            Trace.WriteLine("Stream {");
        //            Trace.Indent();
        //            var s = obj as PdfStream;
        //            Trace.WriteLine(obj.ToString());
        //            Trace.Unindent();
        //            Trace.WriteLine("} Stream ");
        //            return;
        //        }
        //        if (obj.GetObjectType() == PdfObject.NAME)
        //        {
        //            Trace.WriteLine("Name {");
        //            Trace.Indent();
        //            var s = obj as PdfStream;
        //            Trace.WriteLine(obj.ToString());
        //            Trace.Unindent();
        //            Trace.WriteLine("} Name");
        //            return;
        //        }
        //        if (obj.GetObjectType() == PdfObject.NUMBER)
        //        {
        //            Trace.WriteLine("NUMBER {");
        //            Trace.Indent();
        //            var s = obj as PdfStream;
        //            Trace.WriteLine(obj.ToString());
        //            Trace.Unindent();
        //            Trace.WriteLine("} NUMBER");
        //            return;
        //        }
        //        if (obj.GetObjectType() == PdfObject.DICTIONARY)
        //        {
        //            Trace.WriteLine("Dic {");
        //            Trace.Indent();
        //            foreach (var i in (obj as PdfDictionary).Values())
        //                Dump(i);

        //            Trace.Unindent();
        //            Trace.WriteLine("} Dic ");
        //            return;
        //        }
        //        if (obj.GetObjectType() == PdfObject.ARRAY)
        //        {
        //            Trace.WriteLine("Array {");
        //            Trace.Indent();

        //            Trace.WriteLine(obj.ToString());

        //            foreach (var i in (obj as PdfArray))
        //                Dump(i);

        //            //    Dump(i);

        //            Trace.WriteLine("} Array");
        //            Trace.Unindent();
        //            return;
        //        }
        //        Trace.WriteLine("{{{");
        //        Trace.WriteLine(obj.GetObjectType());
        //        Trace.WriteLine(obj.ToString());
        //        Trace.WriteLine("}}}");
        //    }
        //    finally{ depth--; }
        //}

        //public void Dump(PdfPage page)
        //{
        //    Trace.WriteLine("Page >>>");
        //    var dict = page.GetPdfObject();
        //    Trace.WriteLine(page.GetContentStream(0));
        //    Trace.Indent();
        //    foreach (var o in dict.Values(true))
        //    {
        //        Dump(o);
        //    }
        //    Trace.Unindent();
        //    Trace.WriteLine("<<<< Page");
        //}

        //[Fact]
        //public void Argle2()
        //{
            //var memory = new MemoryStream();
            //var r= new BinaryReader(memory);
            //iText.Kernel.Pdf.PdfReader iTextReader = new iText.Kernel.Pdf.PdfReader(memory);
            //iText.Kernel.Pdf.PdfDocument pdfDoc = new iText.Kernel.Pdf.PdfDocument(iTextReader);
            //Dump(pdfDoc);

            //using (var stream = new MemoryStream(File.ReadAllBytes(Paths.SourceDocument2)))
            //{
            //    using (var reader = new PdfReader(stream))
            //    {
            //        using (var document = new PdfDocument(reader))
            //        {
            //            var pages = document.GetNumberOfPages();
            //            for (var i = 4; i <= pages; i++)
            //            {
            //                var page = document.GetPage(i);

            //                Dump(page);

            //                //var dict = page.GetPdfObject();
            //                //Trace.WriteLine(page.GetContentStream(0));
            //                //foreach (var o in dict.Values(true))
            //                //{
            //                //    Trace.Indent();
            //                //    Trace.WriteLine(o.GetObjectType());
            //                //    if (o.GetObjectType() == PdfObject.DICTIONARY)
            //                //    {
            //                //        foreach (var p in dict.Values(true))
            //                //        {
            //                //            Trace.Indent();
            //                //            Trace.WriteLine(p.GetObjectType());
            //                //            if (p.GetObjectType() == PdfObject.DICTIONARY)
            //                //            {
            //                //                foreach (var q in dict.Values(true))
            //                //                {
            //                //                    Trace.Indent();
            //                //                    Trace.WriteLine(q.GetObjectType());
            //                //                    if (q.GetObjectType() == PdfObject.DICTIONARY)
            //                //                    {

            //                //                    }
            //                //                    Trace.Unindent();
            //                //                }

            //                //            }
            //                //            Trace.Unindent();
            //                //        }
            //                //    }

            //                //    if (o.GetObjectType() == PdfObject.STREAM)
            //                //    {
            //                //        Trace.WriteLine($"Stream...");
            //                //    }
            //                //    Trace.Unindent();
            //                }


            //                //var xcontent = dict.Get(PdfName.Layout);
            //                //if (xcontent != null)
            //                //{
            //                //    var thearray = xcontent as PdfArray;
            //                //    if (thearray != null)
            //                //    foreach (PdfObject obj in thearray)
            //                //    {
            //                //        // these objects actually are PdfIndirectReferences
            //                //        // converting them leads nowhere, so here is the point
            //                //        // where I would have to resolve the reference and use whatever
            //                //        // objects I might obtain that way.
            //                //        PdfStream strm = obj as PdfStream;
            //                //        if (strm != null)
            //                //        {
            //                //            byte[] data = strm.GetBytes();
            //                //            UTF8Encoding enc = new UTF8Encoding();

            //                //            string test = enc.GetString(data);
            //                //            Trace.WriteLine(test);
            //                //        }
            //                //    }
            //                //}
            //            }
            //        }
            //    }
            //}
            ////int numberofpages = pdfDoc.GetNumberOfPages();
            //for (int page = 1; page <= numberofpages; page++)
            //{
            //    var p = iText.Kernel.Pdf.Canvas.Parser.PdfDocumentContentParser();

            //    iText.Kernel.Pdf.Canvas.Parser.Listener.ITextExtractionStrategy strategy = new iText.Kernel.Pdf.Canvas.Parser.Listener.SimpleTextExtractionStrategy();
            //    string currentText = iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy);
            //}

            //var reader = new PdfWriter("C:\\Testing\\Documentation\\Infomercial.pdf");
            //PdfDocument pdf = new PdfDocument(writer);
            //Document document = new Document(pdf);
            //iText.Barcodes.BarcodeQRCode qrcode = new BarcodeQRCode("{\"item\":\"123\",\"id\":\"123\"}");
            //var codeFormObject = qrcode.CreateFormXObject(pdf);


            //var file = "test.pdf";
            //var reader = new PdfReader(file);

            //var streamBytes = reader.GetPageContent(2);
            //var tokenizer = new PRTokeniser(new RandomAccessFileOrArray(streamBytes));
            //var ps = new PdfContentParser(tokenizer);

            //List<PdfObject> operands = new List<PdfObject>();
            //while (ps.Parse(operands).Count > 0)
            //{
            //    PdfLiteral oper = (PdfLiteral)operands[operands.Count - 1];
            //    var cmd = oper.ToString();

            //    switch (cmd)
            //    {
            //        case "q":
            //            Console.WriteLine("SaveGraphicsState(); //q");
            //            break;

            //        case "Q":
            //            Console.WriteLine("RestoreGraphicsState(); //Q");
            //            break;

            //            // good luck with the rest!

            //    }
            //}
        //}


        //[Fact]
        //public void Argle()
        //{
        //    var d = PdfDocument.Open(Paths.SourceDocument2);

        //    foreach(var i in d.GetPages().Last().Operations.OfType<UglyToad.PdfPig.Graphics.Operations.PathConstruction.AppendStraightLineSegment>())
        //    {
        //        Trace.WriteLine(i.X);
        //        //Trace.WriteLine(i.GetType());
        //    }


        //    foreach (var i in d.GetPages().Last())
        //    {
        //        Trace.WriteLine(i.X);



        //        Dump(d);


                
        //    //var images =  d.GetPages().SelectMany(p => p.GetImages()).ToArray();

        //    ////var found = PdfDocument.Open(Paths.SourceDocument1).GetPages().SelectMany(x => x.GetImages()).ToArray();
        //    //int j = 0;
        //    //foreach (var i in images)
        //    //{
        //    //    i.TryGetPng(out var stuff1);
        //    //    using (var ms = new MemoryStream(stuff1))
        //    //    {
        //    //        var pngImage = Image.FromStream(ms);
        //    //        pngImage.Save($"D:\\{++j}.png");

        //    //    }
        //    //}

        //    //var stuff = new PdfParser().Parse(Paths.SourceDocument1);
        //    //using (var ms = new MemoryStream(stuff))
        //    //{
        //    //    var pngImage = Image.FromStream(ms);
        //    //    pngImage.Save("D:\\i.png");
        //    //    using (var bms = new MemoryStream())
        //    //    {
        //    //        pngImage.Save(bms, ImageFormat.Bmp);
        //    //        var b = new Bitmap(bms);
        //    //        var decoder = new QrCodeService();
        //    //        var decoded = decoder.GetQRCodeResult(b);
        //    //        Trace.WriteLine(decoded);
        //    //    }
        //    }
        //}

        //private void Dump(PdfDocument pdfDocument)
        //{
        //    //pdfDocument.TryGetXmpMetadata(out var xmpMetadata);
        //    foreach (var i in pdfDocument.
        //    {
        //        Trace.WriteLine($"{i.GetRole()}");
        //        //Trace.WriteLine($"{i.Value}");
        //    }
        //}

    //}

    //public class PdfParser
    //{
    //    private PdfDocument _Document;

    //    public byte[] Parse(string filePath)
    //    {
    //        using (var d = PdfDocument.Open(filePath))
    //        {
    //            return d.GetPages().SelectMany(x => x.GetImages()).OrderBy(x => x.Bounds.Width).First().TryGetPng(out var pngBytes) ? pngBytes : null;
    //        }
    //    }
    //}
//}