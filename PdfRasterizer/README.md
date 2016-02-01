## ![](Common/icon.png) PdfRasterizer Plugin for Xamarin and Windows

Simple cross platform plugin that renders PDF files to images for easier integration.

### Setup

* Available on NuGet: http://www.nuget.org/packages/Xam.Plugin.PdfRasterizer  [![NuGet](https://img.shields.io/nuget/v/Xam.Plugin.PdfRasterizer.svg?label=NuGet)](https://www.nuget.org/packages/Xam.Plugin.PdfRasterizer/)
* Install into your PCL project and Client projects.

**Platform Support**

|Platform|Supported|Version|
| ------------------- | :-----------: | :------------------: |
|Xamarin.iOS|Yes|iOS 6+|
|Xamarin.iOS Unified|Yes|iOS 6+|
|Xamarin.Android|Yes|API 21+|
|Windows Store RT|Yes|8.1+|
|Windows 10 UWP|Yes|10+|
|Windows Phone Silverlight|No||
|Windows Phone RT|No||
|Xamarin.Mac|No||

### API Usage

Call **CrossPdfRasterizer.Current** from any project or PCL to gain access to APIs.

A `.pdf` document rasterization will render all of its pages as `.png` images in the local storage of the application (by default in the `/.pdf/` subfolder). Local and distant document can be prodvided. If the provided document path starts with `http://` or `https://`, the document will be downloaded first in the local temporary folder of the application.

#### Example

```csharp
try
{
  const string documentUrl = "https://developer.xamarin.com/guides/xamarin-forms/getting-started/introduction-to-xamarin-forms/offline.pdf";
  const bool forceRasterize = false;
  
  var rasterizer = CrossPdfRasterizer.Current;
  
  var document = await rasterizer.Rasterize(documentUrl,forceRasterize);
  var pageImages = document.Pages.Select((p) => p.Path);
}
catch(Exception ex)
{
  Debug.WriteLine("Unable to rasterize provided document: " + ex);
}
```

#### API

```csharp
/// <summary>
/// The directory where all the rasterized PDF's sub-directories containing page images are created (default: "/.pdf/").
/// </summary>
string RasterizationCacheDirectory { get; set; }
```

```csharp
/// <summary>
/// Renders a local PDF file as a set of page images into the local storage.
/// </summary>
/// <param name="pdfPath">The relative path to the PDF file in local storage, or a distant url. If a distant Url is provided, the document will be downloaded first in the temporary folder of the application.</param>
/// <param name="cachePirority">Indicates whether the already rasterized version should be taken, or images must be forced to be rasterized again.</param>
/// <returns>A document containing all the pages with their paths.</returns>
Task<PdfDocument> Rasterize(string pdfPath, bool cachePirority = true);
```

```csharp
/// <summary>
/// Gets the locally rendered document if it has already been rasterized, else it returns null.
/// </summary>
/// <param name="pdfPath"></param>
/// <returns></returns>
Task<PdfDocument> GetRasterized(string pdfPath);
```

#### Q&A

> Why is the rasterizer only available for Android version prior to Lollipop ?

Because the `PdfRenderer` was introduced only in this version. I didn't found any free solution to have the same result of older versions of Android. If you have a solution don't hesitate to contact me !

### Roadmap / Ideas

* Adding custoization for generation location.
* Adding more document meta-data (with maybe the original document available too)
* Adding PDF drawing generation functionality (*iOS and Android only*)
* Creating a Xamarin.Forms SimplePdfViewer

### Contributors
* [aloisdeniel](https://github.com/aloisdeniel)

Thanks!

### License
Licensed under main repo license