using Plugin.UriNavigationService.Abstractions;
using System;

namespace Plugin.UriNavigationService
{
  /// <summary>
  /// Cross platform UriNavigationService implemenations
  /// </summary>
  public class CrossUriNavigationService
  {
    static Lazy<IUriNavigationService> Implementation = new Lazy<IUriNavigationService>(() => CreateUriNavigationService(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// Current settings to use
    /// </summary>
    public static IUriNavigationService Current
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

    static IUriNavigationService CreateUriNavigationService()
    {
#if PORTABLE
        return null;
#else
        return new UriNavigationServiceImplementation();
#endif
    }

    internal static Exception NotImplementedInReferenceAssembly()
    {
      return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
  }
}
