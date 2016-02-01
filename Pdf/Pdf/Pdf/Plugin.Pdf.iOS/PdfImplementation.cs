using CoreGraphics;
using Foundation;
using Plugin.Pdf.Abstractions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UIKit;
using System.Diagnostics;
using System.Net;

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

		private const string MetaFile = "__meta";

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
			Debug.WriteLine ("ISNULL:" + (pdf == null));

			var result = new string[pdf.Pages];

			Directory.CreateDirectory(outputDirectory);

			for (int i = 0; i < pdf.Pages; i++) {
				var pagePath = string.Format ("{0}/{1}.png", outputDirectory.TrimEnd (new char[] { '/', '\\' }), i);
				Debug.WriteLine("P:"+pagePath);
				var page = pdf.GetPage (i+1);
				Debug.WriteLine ("PAGE:" + (page == null));
				var image = RenderImage (page);
				var data = image.AsPNG ();
				File.Create (pagePath);
				data.Save (pagePath, true);
				result [i] = pagePath;
			}

			var metaPath = string.Format ("{0}/{1}", outputDirectory.TrimEnd (new char[] { '/', '\\' }), MetaFile);
			File.Create (metaPath);

			return result;
		}

		private string GetLocalPath(string pdfPath)
		{
			var documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var hash = this.Hash.Create (pdfPath);
			return documents.AppendPath ((LocalPdfCacheDirectory.ToFolderPath () + hash).ToFolderPath ());
		}

		public async Task<Abstractions.PdfDocument> Rasterize (string pdfPath, bool cachePirority = true)
		{
			if(cachePirority)
			{
				var existing = await GetRasterized(pdfPath);
				if(existing != null)
				{
					return existing;
				}
			}

			//TODO threading the process

			var localpath = pdfPath.IsDistantUrl () ? this.DownloadTemporary (pdfPath) : pdfPath;
			var pdf = CGPDFDocument.FromFile (localpath);

			var path = GetLocalPath(pdfPath);
			var pagesPaths = this.Render (pdf, path);

			return new PdfDocument () 
			{
				Pages = pagesPaths.Select ((p) => new PdfPage () { Path = p }),
			};
		}

		public Task<PdfDocument> GetRasterized (string pdfPath)
		{
			var path = GetLocalPath(pdfPath);
			var metaPath = string.Format ("{0}/{1}", path.TrimEnd (new char[] { '/', '\\' }), MetaFile);
			if (File.Exists (metaPath)) 
			{
				var rendered = Directory.GetFiles (path).Where ((p) => Path.GetFileName(p) != MetaFile);
				return Task.FromResult(new Abstractions.PdfDocument()
				{
					Pages = rendered.Select((p) => new Abstractions.PdfPage() { Path = p }),
					});
			}

			return null;
		}

		#region Download and render

		private string DownloadTemporary (string url)
		{
			var documents = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)[0].Path;

			var tmp = Path.Combine(documents, "../", "tmp");

			var tempName = System.IO.Path.Combine (tmp, String.Format ("{0}.pdf", Guid.NewGuid ().ToString ("N")));

			var webClient = new WebClient ();
			webClient.DownloadFile (new Uri (url), tempName);

			return tempName;
		}

		#endregion
	}
}