using Plugin.Pdf.Abstractions;
using System;
using System.Threading.Tasks;

namespace Plugin.Pdf
{
    /// <summary>
    /// Implementation for Pdf
    /// </summary>
    public class PdfImplementation : IPdf
    {
        public Task<string[]> RenderImages(string pdfPath, string outputDirectory, double resolution)
        {
            throw new NotImplementedException();
        }
    }
}