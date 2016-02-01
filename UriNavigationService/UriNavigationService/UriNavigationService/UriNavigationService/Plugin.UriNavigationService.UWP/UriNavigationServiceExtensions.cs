using Plugin.UriNavigationService.Abstractions;
using Plugin.UriNavigationService.Abstractions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using System.Reflection;
using Windows.UI.Xaml;

namespace Plugin.UriNavigationService
{
    public static class UriNavigationServiceExtensions
    {
        public static object GetPageContext(this IUriNavigationService service, object navigationArgs)
        {
            return service.GetViewContext(navigationArgs as string);
        }
        
    }
}
