using Plugin.Pdf.Abstractions;
using System;


namespace Plugin.Pdf
{
  /// <summary>
  /// Implementation for Feature
  /// </summary>
  public class PdfImplementation : IPdf
    {
        public Task<string[]> Render(string pdfPath, string outputDirectory, double resolution)
        {
            throw new NotImplementedException();
        }

        public Task<string[]> DownloadAndRender(string pdfUrl, string outputDirectory, double resolution)
        {
            throw new NotImplementedException();
        }
    }
}