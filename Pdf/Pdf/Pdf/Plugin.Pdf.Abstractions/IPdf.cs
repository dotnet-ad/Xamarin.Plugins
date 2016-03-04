using System;
using System.Threading.Tasks;

namespace Plugin.Pdf.Abstractions
{
  /// <summary>
  /// Interface for Pdf
  /// </summary>
  public interface IPdf
  {
        /// <summary>
        /// Renders a local PDF file as a set of page images.
        /// </summary>
        /// <param name="pdfPath">The relative path to the PDF file in local storage, or a distant url.</param>
        /// <param name="outputDirectory">The relative path to the output directory where the images will be generated.</param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        Task<PdfDocument> Rasterize(string pdfPath, bool cachePirority = true);

        Task<PdfDocument> GetRasterized(string pdfPath);
    }
}
