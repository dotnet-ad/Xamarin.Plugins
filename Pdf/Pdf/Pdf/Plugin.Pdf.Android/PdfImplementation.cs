using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Pdf;
using Java.IO;
using Plugin.Pdf.Abstractions;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;


namespace Plugin.Pdf
{
  /// <summary>
  /// Implementation for Feature
  /// </summary>
  public class PdfImplementation : IPdf
    {
        // API > 21 :
        // http://developer.android.com/reference/android/graphics/pdf/PdfRenderer.html
        // https://github.com/googlesamples/android-PdfRendererBasic
        // https://github.com/voghDev/PdfViewPager

        public PdfImplementation()
        {
            this.Context = Application.Context;
        }

        public Context Context { get; set; }
        
        private static Bitmap RenderImage(PdfRenderer.Page page, double resolution)
        {
            var bitmap = Bitmap.CreateBitmap (page.Width, page.Height,Bitmap.Config.Argb8888);
            page.Render(bitmap, null, null, PdfRenderMode.ForDisplay);
            return bitmap;
        }

        private string[] Render(PdfRenderer pdf, string outputDirectory, double resolution)
        {
            var result = new string[pdf.PageCount];

            for (int i = 0; i < pdf.PageCount; i++)
            {
                var pagePath = string.Format("{0}/{1}.png", outputDirectory.TrimEnd(new char[] { '/', '\\' }), i);
                var page = pdf.OpenPage(i);
                var image = RenderImage(page, resolution);

                using (var fs = new FileStream(pagePath, FileMode.CreateNew))
                {
                    image.Compress(Bitmap.CompressFormat.Png, 95, fs);
                }

                result[i] = pagePath;

                page.Close();
            }

            pdf.Close();

            return result;
        }

        private static void CheckApiLevel()
        {
            if (((int)Android.OS.Build.VERSION.SdkInt) < 21)
            {
                throw new NotSupportedException("Android API level must be at least 21 to support PDF rendering");
            }
        }

        public Task<string[]> Render(string pdfPath, string outputDirectory, double resolution)
        {
            CheckApiLevel();

            var descriptor = Context.Assets.OpenFd(pdfPath).ParcelFileDescriptor;

            var pdf = new PdfRenderer(descriptor);
            var result = this.Render(pdf, outputDirectory, resolution);

            descriptor.Close();

            return Task.FromResult(result);
        }

        #region Download and render

        private string DownloadTemporary(string url)
        {
            var tempName = System.IO.Path.Combine("/tmp/", String.Format("{0}.pdf", Guid.NewGuid().ToString("N")));

            var webClient = new WebClient();
            webClient.DownloadFile(new Uri(url), tempName);

            return tempName;
        }

        public async Task<string[]> DownloadAndRender(string pdfUrl, string outputDirectory, double resolution)
        {
            var path = this.DownloadTemporary(pdfUrl);
            return await this.Render(path, outputDirectory, resolution);
        }

        #endregion
    }
}