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

        public static void RegisterPage<T>(this IUriNavigationService service)
        {
            var page = typeof(T);
            var attribute = (NavigationContextAttribute)page.GetTypeInfo().GetCustomAttribute(typeof(NavigationContextAttribute));
            if (attribute == null) throw new ArgumentException("The page type should have a \"NavigationContextAttribute\" attribute.");

            service.RegisterView(attribute.ContextType, (a) =>
            {
                var frame = (Frame)Window.Current.Content;
                frame.Navigate(page, a);
                return Task.FromResult(true);
            }, () =>
             {
                 var frame = (Frame)Window.Current.Content;
                 frame.GoBack();
                 return Task.FromResult(true);
             });
        }
    }
}
