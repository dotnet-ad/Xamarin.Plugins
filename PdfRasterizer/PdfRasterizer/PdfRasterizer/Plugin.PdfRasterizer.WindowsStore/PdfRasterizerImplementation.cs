using Plugin.PdfRasterizer.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.Web.Http;

namespace Plugin.PdfRasterizer
{
    /// <summary>
    /// Implementation for Pdf
    /// </summary>
    public class PdfRasterizerImplementation : IPdfRasterizer
    {
        public PdfRasterizerImplementation()
        {
            this.HttpClient = new HttpClient();
            this.Hash = new Hash();
        }

        public string RasterizationCacheDirectory { get; set; } = DefaultRasterizationCacheDirectory;

        private const string DefaultRasterizationCacheDirectory = "/.pdf";

        private const string MetaFile = "__meta";

        public IHash Hash { get; set; }
        
        public HttpClient HttpClient { get; set; }

        private static async Task<StorageFolder> GetOrCreateLocalFolder(string path)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            var splits = path.SplitPath();

            for (int i = 0; i < splits.Length; i++)
            {
                var segment = splits[i];
                folder = await folder.CreateFolderAsync(segment, CreationCollisionOption.OpenIfExists);
            }

            return folder;
        }

        private static async Task<StorageFile> GetLocalFile(string path)
        {
            var splits = path.SplitPath();

            var folder = await GetOrCreateLocalFolder(string.Join("/",splits.Take(splits.Count() - 1).ToArray()));
            var last = splits.LastOrDefault();

            return await folder.GetFileAsync(last);
        }

        private async Task<StorageFile> DownloadTemporary(string url)
        {
            var tempName = String.Format("{0}.pdf", Guid.NewGuid().ToString("N"));
            var uri = new Uri(url);
            var fileName = Path.GetFileName(uri.LocalPath);
            var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(tempName, CreationCollisionOption.ReplaceExisting);

            var buffer = await this.HttpClient.GetBufferAsync(uri);
            await FileIO.WriteBufferAsync(file, buffer);

            return file;
        }

        private static async Task<string> RenderPage(Windows.Data.Pdf.PdfPage page, StorageFolder output)
        {
            var pagePath = string.Format("{0}.png", page.Index);
            var pageFile = await output.CreateFileAsync(pagePath, CreationCollisionOption.ReplaceExisting);
            
            using (var imageStream = await pageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                PdfPageRenderOptions pdfPageRenderOptions = new PdfPageRenderOptions();
                pdfPageRenderOptions.IsIgnoringHighContrast = false;
                await page.RenderToStreamAsync(imageStream, pdfPageRenderOptions);
                await imageStream.FlushAsync();
            }

            return output.Path.ToFolderPath() + pagePath;
        }

        private async Task<string[]> Rasterize(StorageFile file, StorageFolder outputDirectory)
        {            
            var doc = await Windows.Data.Pdf.PdfDocument.LoadFromFileAsync(file);
            
            var result = new string[doc.PageCount];

            for (int i = 0; i < doc.PageCount; i++)
            {
                var page = doc.GetPage((uint)i);
                result[i] = await RenderPage(page, outputDirectory);
            }

            await outputDirectory.CreateFileAsync(MetaFile, CreationCollisionOption.ReplaceExisting);

            return result;
        }
        
        private Task<StorageFolder> GetOutputFolder(string output, string url)
        {
            var hash = this.Hash.Create(url);
            var path = (output.ToFolderPath() + hash).ToFolderPath();
            return GetOrCreateLocalFolder(path);
        }
                
        public async Task<Abstractions.PdfDocument> GetRasterizedAsync(string pdfPath)
        {
            var output = await GetOutputFolder(this.RasterizationCacheDirectory, pdfPath);
            
            var files = await output.GetFilesAsync();

            if (files.Any((p) => p.Name == MetaFile))
            {
                var rendered = files.Where((p) => p.Name != MetaFile).Select((f) => f.Path.ToFilePath()).ToArray();
                return new Abstractions.PdfDocument()
                {
                    Pages = rendered.Select((p) => new Abstractions.PdfPage() { Path = p }),
                };
            }

            return null;
        }

        public async Task<Abstractions.PdfDocument> RasterizeAsync(string pdfPath, bool cachePirority = true)
        {
            if(cachePirority)
            {
                var existing = await GetRasterizedAsync(pdfPath);
                if(existing != null)
                {
                    return existing;
                }
            }

            var output = await GetOutputFolder(this.RasterizationCacheDirectory, pdfPath);
            
            // First we remove any existing file
            var files = await output.GetFilesAsync();
            await Task.WhenAll(files.Select((f) => f.DeleteAsync().AsTask()).ToArray());
            
            var file = pdfPath.IsDistantUrl() ? await DownloadTemporary(pdfPath) : await GetLocalFile(pdfPath);
            
            var pagesPaths = await Rasterize(file, output);
          
            return new Abstractions.PdfDocument()
            {
                Pages = pagesPaths.Select((p) => new Abstractions.PdfPage() { Path = p }),
            };
        }
        
    }
}