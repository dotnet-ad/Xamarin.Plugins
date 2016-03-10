using Plugin.PdfRasterizer.Abstractions;
using System;

namespace Plugin.PdfRasterizer
{
  /// <summary>
  /// Cross platform Pdf implemenations
  /// </summary>
  public class CrossPdfRasterizer
  {
    static Lazy<IPdfRasterizer> Implementation = new Lazy<IPdfRasterizer>(() => CreatePdf(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// Current settings to use
    /// </summary>
    public static IPdfRasterizer Current
    {
      get
      {
        var ret = Implementation.Value;
        if (ret == null)
        {
          throw NotImplementedInReferenceAssembly();
        }
        return ret;
      }
    }

    static IPdfRasterizer CreatePdf()
    {
#if WINDOWS_PHONE || WINDOWS_PHONE_APP
        throw new PlatformNotSupportedException();
#elif PORTABLE
        return null;
#else
        return new PdfRasterizerImplementation();
#endif
    }

    internal static Exception NotImplementedInReferenceAssembly()
    {
      return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
  }
}
