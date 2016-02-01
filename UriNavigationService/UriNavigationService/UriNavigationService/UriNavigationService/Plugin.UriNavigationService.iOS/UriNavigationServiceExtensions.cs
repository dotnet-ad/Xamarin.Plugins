using Plugin.UriNavigationService.Abstractions;
using Plugin.UriNavigationService.Abstractions.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Plugin.UriNavigationService
{
    public static class UriNavigationServiceExtensions
    {
        public static object GetActivityContext(this IUriNavigationService service, Intent intent)
        {

        }

        public static void RegisterActivity<T>(this IUriNavigationService service)
        {
            var activity = typeof(T);
            var attribute = (NavigationViewAttribute)activity.GetTypeInfo().GetCustomAttribute(typeof(NavigationViewAttribute));
            if (attribute == null) throw new ArgumentException("The activity type should have a \"NavigationContextAttribute\" attribute.");
        }
    }
}
