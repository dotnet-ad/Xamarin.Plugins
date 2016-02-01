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
        public Task<PdfDocument> GetRasterized(string pdfPath)
        {
            throw new NotImplementedException();
        }

        public Task<PdfDocument> Rasterize(string pdfPath, bool cachePirority = true)
        {
            throw new NotImplementedException();
        }
    }
}