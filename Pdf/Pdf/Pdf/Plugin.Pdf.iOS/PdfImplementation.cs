using CoreGraphics;
using Foundation;
using Plugin.Pdf.Abstractions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UIKit;
using System.Diagnostics;

namespace Plugin.Pdf
{
	/// <summary>
	/// Implementation for Pdf
	/// </summary>
	public class PdfImplementation : IPdf
	{
		public PdfImplementation ()
		{
			this.Hash = new Hash ();
		}

		private const string LocalPdfCacheDirectory = ".pdf";

		public IHash Hash { get; set; }

		private static readonly nfloat One = (nfloat)1.0;

		private static UIImage RenderImage (CGPDFPage page)
		{
			var rect = page.GetBoxRect (CGPDFBox.Crop);
			var pageRotation = page.RotationAngle;
			var size = rect.Size;

			UIGraphics.BeginImageContextWithOptions (size, false, (nfloat)(1));

			var context = UIGraphics.GetCurrentContext ();
			context.SaveState ();
			context.TranslateCTM ((nfloat)0.0, size.Height);
			context.ScaleCTM (One, (nfloat)(-1.0));

			context.SetFillColor (One, One);
			context.FillRect (rect);

			var transform = page.GetDrawingTransform (CGPDFBox.Crop, rect, 0, true);
			context.ConcatCTM (transform);
			context.DrawPDFPage (page);

			var image = UIGraphics.GetImageFromCurrentImageContext ();
			context.RestoreState ();
			UIGraphics.EndImageContext ();

			return image;
		}

		private string[] Render (CGPDFDocument pdf, string outputDirectory)
		{
			var result = new string[pdf.Pages];

			for (int i = 0; i < pdf.Pages; i++) {
				var pagePath = string.Format ("{0}/{1}.png", outputDirectory.TrimEnd (new char[] { '/', '\\' }), i);
				var page = pdf.GetPage (i);
				var image = RenderImage (page);
				var data = image.AsPNG ();
				data.Save (pagePath, true);
				result [i] = pagePath;
			}

			return result;
		}

		public Task<Abstractions.PdfDocument> Rasterize (string pdfPath, bool cachePirority = true)
		{
			var pdf = pdfPath.IsDistantUrl () ? CGPDFDocument.FromUrl (pdfPath) : CGPDFDocument.FromFile (pdfPath);

			var documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var hash = this.Hash.Create (pdfPath);
			var path = documents.AppendPath ((LocalPdfCacheDirectory.ToFolderPath () + hash).ToFolderPath ());
			var pagesPaths = this.Render (pdf, path);

			return Task.FromResult (new PdfDocument () {
				Pages = pagesPaths.Select ((p) => new PdfPage () { Path = p }),
			});
		}

		public Task<PdfDocument> GetRasterized (string pdfPath)
		{
			throw new NotImplementedException ();
		}
	}
}