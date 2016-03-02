using Plugin.Pdf.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Storage;

namespace Plugin.Pdf
{
    /// <summary>
    /// Implementation for Pdf
    /// </summary>
    public class PdfImplementation : IPdf
    {
        private static async Task<IStorageItem> GetLocalItem(string path)
        {
            IStorageFolder folder = ApplicationData.Current.LocalFolder;
            var splits = path.Split(new char[] { '/', '\\' });

            foreach (var segment in splits)
            {
                var item = await folder.GetItemAsync(segment);
                folder = item as IStorageFolder;
                if(folder == null)
                {
                    return item;
                }
            }

            return folder;
         }

        private static async Task<string> RenderPage(PdfPage page, IStorageFolder output)
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

            return pagePath;
        }

        public async Task<string[]> RenderImages(string pdfPath, string outputDirectory, double resolution)
        {
            var file = GetLocalItem(pdfPath) as IStorageFile;
            var output = GetLocalItem(outputDirectory) as IStorageFolder;

            var doc = await PdfDocument.LoadFromFileAsync(file);

            var result = new string[doc.PageCount];

            for (int i = 0; i < doc.PageCount; i++)
            {
                var page = doc.GetPage((uint)i);
                var pageName = await RenderPage(page, output);
                var pagePath = string.Format("{0}/{1}", outputDirectory.TrimEnd(new char[] { '/', '\\' }), pageName);
                result[i] = pagePath;
            }

            return result;
        }
    }
}