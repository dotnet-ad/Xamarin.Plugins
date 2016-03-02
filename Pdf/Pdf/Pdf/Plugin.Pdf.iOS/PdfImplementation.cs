using CoreGraphics;
using Foundation;
using Plugin.Pdf.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;
using UIKit;

namespace Plugin.Pdf
{
    /// <summary>
    /// Implementation for Pdf
    /// </summary>
    public class PdfImplementation : IPdf
    {
        private static readonly nfloat One = (nfloat)1.0;

        private static UIImage RenderImage(CGPDFPage page, double resolution)
        {
            var rect = page.GetBoxRect(CGPDFBox.Crop);
            var pageRotation = page.RotationAngle;
            var size = rect.Size;

            UIGraphics.BeginImageContextWithOptions(size, false, (nfloat)(resolution / 72));
            
            var context = UIGraphics.GetCurrentContext();
            context.SaveState();
            context.TranslateCTM((nfloat)0.0, size.Height);
            context.ScaleCTM(One, (nfloat)(-1.0));

            context.SetFillColor(One, One);
            context.FillRect(rect);

            var transform = page.GetDrawingTransform(CGPDFBox.Crop, rect, 0, true);
            context.ConcatCTM(transform);
            context.DrawPDFPage(page);

            var image = UIGraphics.GetImageFromCurrentImageContext();
            context.RestoreState();
            UIGraphics.EndImageContext();

            return image;
        }

        public Task<string[]> RenderImages(string pdfPath, string outputDirectory, double resolution)
        {
            var pdf = CGPDFDocument.FromFile(pdfPath);
            var result = new string[pdf.Pages];

            for (int i = 0; i < pdf.Pages; i++)
            {
                var pagePath = string.Format("{0}/{1}.png", outputDirectory.TrimEnd(new char[] { '/', '\\' }), i);
                var page = pdf.GetPage(i);
                var image = RenderImage(page,resolution);
                var data = image.AsPNG();
                data.Save(pagePath, true);
                result[i] = pagePath;
            }
            
            return Task.FromResult(result);
        }
    }
}