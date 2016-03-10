
using Plugin.PdfRasterizer.Abstractions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.ComponentModel;

#if DROID
using Android.Graphics.Pdf;
using Android.Graphics;
using Android.App;
#else
using UIKit;
using CoreGraphics;
using Foundation;
#endif

namespace Plugin.PdfRasterizer
{
	/// <summary>
	/// Implementation for Pdf
	/// </summary>
	public class PdfRasterizerImplementation : IPdfRasterizer
	{
		public PdfRasterizerImplementation ()
		{
			this.Hash = new Hash ();
        }
        public string RasterizationCacheDirectory { get; set; } = DefaultRasterizationCacheDirectory;

        private const string DefaultRasterizationCacheDirectory = "/.pdf";

		private const string MetaFile = "__meta";

		public IHash Hash { get; set; }


		#if DROID

		private static Bitmap RenderImage (PdfRenderer.Page page)
		{
			var bitmap = Bitmap.CreateBitmap (page.Width, page.Height, Bitmap.Config.Argb8888);

			// Fill with default while color first
			var canvas = new Canvas(bitmap);
			var paint = new Paint ()
			{
				Color = Color.White
			};
			canvas.DrawRect (new Rect (0, 0, page.Width, page.Height), paint);

			// Render content
			page.Render (bitmap, null, null, PdfRenderMode.ForDisplay);
			return bitmap;
		}

		private string[] Render (PdfRenderer pdf, string outputDirectory)
		{
			var result = new string[pdf.PageCount];

			for (int i = 0; i < pdf.PageCount; i++) {
				var pagePath = string.Format ("{0}/{1}.png", outputDirectory.TrimEnd (new char[] { '/', '\\' }), i);
				var page = pdf.OpenPage (i);
				var image = RenderImage (page);

				using (var fs = new FileStream (pagePath, FileMode.CreateNew)) {
					image.Compress (Bitmap.CompressFormat.Png, 95, fs);
				}

				result [i] = pagePath;

				page.Close ();
			}

			pdf.Close ();

			var metaPath = string.Format ("{0}/{1}", outputDirectory.TrimEnd (new char[] { '/', '\\' }), MetaFile);
			File.Create (metaPath);

			return result;
		}

		#else

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
		#endif

		private string GetLocalPath(string pdfPath, bool deleteFirst)
		{
			var documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			var hash = this.Hash.Create (pdfPath);
			var result = documents.AppendPath ((this.RasterizationCacheDirectory.ToFolderPath () + hash).ToFolderPath ());

			// Deletes content and folder first if requested
			if (deleteFirst) {
				var files = Directory.GetFiles (result);
				foreach (var item in files) {
					File.Delete (item);
				}
				Directory.Delete (result);
			}

			Directory.CreateDirectory(result);
			return result;
		}

		public async Task<Abstractions.PdfDocument> Rasterize (string pdfPath, bool cachePirority = true)
		{
			if(cachePirority)
			{
				var existing = await GetRasterized(pdfPath);
				if(existing != null)
				{
					Debug.WriteLine("Using cached images ...");

					return existing;
				}
			}

			Debug.WriteLine("Downloading and generating again ...");

			var localpath = pdfPath.IsDistantUrl () ? await this.DownloadTemporary (pdfPath) : pdfPath;

			//TODO threading the process

			#if DROID
			var f = new Java.IO.File(localpath);
			var fd = Android.OS.ParcelFileDescriptor.Open(f,Android.OS.ParcelFileMode.ReadOnly);
			var pdf = new PdfRenderer (fd);
			#else
			var pdf = CGPDFDocument.FromFile (localpath);
			#endif

			var path = GetLocalPath(pdfPath, !cachePirority);
			var pagesPaths = this.Render (pdf, path);

			return new Plugin.PdfRasterizer.Abstractions.PdfDocument () 
			{
				Pages = pagesPaths.Select ((p) => new PdfPage () { Path = p }),
			};
		}

		public Task<Plugin.PdfRasterizer.Abstractions.PdfDocument> GetRasterized (string pdfPath)
		{
			var path = GetLocalPath(pdfPath,false);
			var metaPath = string.Format ("{0}/{1}", path.TrimEnd (new char[] { '/', '\\' }), MetaFile);
			if (File.Exists (metaPath)) 
			{
				var files = Directory.GetFiles (path);
				var rendered = files.Where ((p) => System.IO.Path.GetFileName(p) != MetaFile);
				return Task.FromResult(new Abstractions.PdfDocument()
					{
						Pages = rendered.Select((p) => new Abstractions.PdfPage() { Path = p }),
					});
			}

			return Task.FromResult((Abstractions.PdfDocument)null);
		}

		#region Download and render

		private async Task<string> DownloadTemporary (string url)
		{
			#if DROID
			var tmp = Application.Context.CacheDir.Path;
			#else
			var documents = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)[0].Path;
			var tmp = Path.Combine(documents, "../", "tmp");
			#endif

			var tempName = System.IO.Path.Combine (tmp, String.Format ("{0}.pdf", Guid.NewGuid ().ToString ("N")));
            
			using (var webClient = new WebClient ()) 
			{
                await webClient.DownloadFileTaskAsync(new Uri(url), tempName);
                return tempName;
            }
		}

		#endregion
	}
}