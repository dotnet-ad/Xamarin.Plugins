namespace Plugin.PdfRasterizer.Abstractions
{
    using System.Threading.Tasks;

    /// <summary>
    /// This service renders PDF files to images for easier integration.
    /// </summary>
    public interface IPdfRasterizer
  {
        /// <summary>
        /// The directory where all the rasterized PDF's sub-directories containing page images are created.
        /// </summary>
        string RasterizationCacheDirectory { get; set; }

        /// <summary>
        /// Renders a local PDF file as a set of page images into a local directory.
        /// </summary>
        /// <param name="pdfPath">The relative path to the PDF file in local storage, or a distant url.</param>
        /// <param name="cachePirority">Indicates whether the already rasterized version should be taken, or images must be forced to be rasterized again.</param>
        /// <returns></returns>
        Task<PdfDocument> RasterizeAsync(string pdfPath, bool cachePirority = true);

        /// <summary>
        /// Gets the locally rendered document if it has already been rasterized, else it returns null.
        /// </summary>
        /// <param name="pdfPath"></param>
        /// <returns></returns>
        Task<PdfDocument> GetRasterizedAsync(string pdfPath);
    }
}
