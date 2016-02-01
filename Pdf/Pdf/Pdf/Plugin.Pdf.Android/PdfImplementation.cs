using Plugin.Pdf.Abstractions;
using System;
using System.Threading.Tasks;


namespace Plugin.Pdf
{
  /// <summary>
  /// Implementation for Feature
  /// </summary>
  public class PdfImplementation : IPdf
  {

        public Task<string[]> RenderImages(string pdfPath, string outputDirectory, double resolution)
        {
            // API > 21 :
            // http://developer.android.com/reference/android/graphics/pdf/PdfRenderer.html
            // https://github.com/googlesamples/android-PdfRendererBasic
            // https://github.com/voghDev/PdfViewPager

            // Testing api level : android.os.Build.VERSION

        }
    }
}