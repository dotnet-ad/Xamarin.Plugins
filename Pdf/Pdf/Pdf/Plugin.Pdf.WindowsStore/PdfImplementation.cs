using Plugin.Pdf.Abstractions;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.Web.Http;

namespace Plugin.Pdf
{
    /// <summary>
    /// Implementation for Pdf
    /// </summary>
    public class PdfImplementation : IPdf
    {
        public PdfImplementation()
        {
            this.HttpClient = new HttpClient();
        }

        public HttpClient HttpClient { get; set; }

        private static async Task<StorageFolder> GetOrCreateLocalFolder(string path)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            var splits = path.Split(new char[] { '/', '\\' });

            for (int i = 0; i < splits.Length; i++)
            {
                var segment = splits[i];
                folder = await folder.CreateFolderAsync(segment, CreationCollisionOption.OpenIfExists);
            }

            return folder;
        }

        private static async Task<StorageFile> GetLocalFile(string path)
        {
            var splits = path.Split(new char[] { '/', '\\' });

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

        private static async Task<string> RenderPage(PdfPage page, StorageFolder output)
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

            return output.Path.Replace('\\','/').TrimEnd(new char[] { '/' }) + "/" + pagePath;
        }

        private async Task<string[]> Render(StorageFile file, string outputDirectory, double resolution)
        {
            var output = await GetOrCreateLocalFolder(outputDirectory);

            var doc = await PdfDocument.LoadFromFileAsync(file);

            var result = new string[doc.PageCount];

            for (int i = 0; i < doc.PageCount; i++)
            {
                var page = doc.GetPage((uint)i);
                result[i] = await RenderPage(page, output);
            }

            return result;
        }

        public async Task<string[]> Render(string pdfPath, string outputDirectory, double resolution)
        {
            var file = await GetLocalFile(pdfPath);
            return await Render(file,outputDirectory,resolution);
        }


        public async Task<string[]> DownloadAndRender(string pdfUrl, string outputDirectory, double resolution)
        {
            var file = await DownloadTemporary(pdfUrl);
            return await Render(file, outputDirectory, resolution);
        }
    }
}