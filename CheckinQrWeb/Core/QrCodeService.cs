using System.Drawing;
using System.Text;
using ZXing;

namespace CheckInQrWeb.Core
{
    public class QrCodeService
    {
        public string Decode(byte[] buffer)
        {
            var stream = new MemoryStream(buffer);
            using (var skiaImage = SkiaSharp.SKBitmap.Decode(stream))
            {
                var skiaReader = new ZXing.SkiaSharp.BarcodeReader();
                var skiaResult = skiaReader.Decode(skiaImage);
                return skiaResult?.Text;
            }
        }

        //// image has one QR code
        //if (dataByteArray.Length == 1) return ForDisplay(QRDecoder.ByteArrayToStr(dataByteArray[0]));

        //// image has more than one QR code
        //StringBuilder str = new StringBuilder();
        //for (int index = 0; index < dataByteArray.Length; index++)
        //{
        //    if (index != 0) str.Append("\r\n");
        //    str.AppendFormat("QR Code {0}\r\n", index + 1);
        //    str.Append(ForDisplay(QRDecoder.ByteArrayToStr(dataByteArray[index])));
        //}
        //return str.ToString();


        //private readonly QRDecoder _QrCodeDecoder = new();

        //public string GetQRCodeResult(Bitmap bitmap)
        //{
        //    var raw = GetRawQRCodeResult(bitmap);

        //    return QRCodeResult(raw);
        //}

        //public byte[][] GetRawQRCodeResult(Bitmap bitmap)
        //{
        //    var QRCodeInputImage = bitmap;
        //    // decode image
        //    //byte[][] dataByteArray = _QrCodeDecoder.ImageDecoder(QRCodeInputImage);

        //    return dataByteArray;
        //}

        //private string ForDisplay(string result)
        //{
        //    int index;
        //    for (index = 0; index < result.Length && (result[index] >= ' ' && result[index] <= '~' || result[index] >= 160); index++) ;
        //    if (index == result.Length) return result;

        //    StringBuilder display = new StringBuilder(result.Substring(0, index));
        //    for (; index < result.Length; index++)
        //    {
        //        char oneChar = result[index];
        //        if (oneChar >= ' ' && oneChar <= '~' || oneChar >= 160)
        //        {
        //            display.Append(oneChar);
        //            continue;
        //        }

        //        if (oneChar == '\r')
        //        {
        //            display.Append("\r\n");
        //            if (index + 1 < result.Length && result[index + 1] == '\n') index++;
        //            continue;
        //        }

        //        if (oneChar == '\n')
        //        {
        //            display.Append("\r\n");
        //            continue;
        //        }

        //        display.Append('¿');
        //    }
        //    return display.ToString();
        //}
    }
}