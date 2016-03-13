using Plugin.Rate.Abstractions;
using System;

namespace Plugin.Rate
{
  /// <summary>
  /// Cross platform Rate implemenations
  /// </summary>
  public class CrossRate
  {
    static Lazy<IRate> Implementation = new Lazy<IRate>(() => CreateRate(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// Current settings to use
    /// </summary>
    public static IRate Current
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

    static IRate CreateRate()
    {
#if PORTABLE
        return null;
#else
        return new RateImplementation();
#endif
    }

    internal static Exception NotImplementedInReferenceAssembly()
    {
      return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
  }
}
