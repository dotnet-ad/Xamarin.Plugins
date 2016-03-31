using Plugin.UriNavigationService.Abstractions.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.UriNavigationService.Abstractions.Attributes
{
    public class NavigationViewAttribute : Attribute
    {
        public NavigationViewAttribute(INavigationAction navAction)
        {
            this.Action = navAction;
        }
        
        public INavigationAction Action { get; private set; }
    }
}
