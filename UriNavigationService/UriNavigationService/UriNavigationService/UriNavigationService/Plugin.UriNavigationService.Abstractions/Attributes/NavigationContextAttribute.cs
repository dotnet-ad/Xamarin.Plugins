using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.UriNavigationService.Abstractions.Attributes
{
    public class NavigationContextAttribute : Attribute
    {
        public NavigationContextAttribute(Type contextType)
        {
            this.ContextType = contextType;
        }

        public Type ContextType { get; set; }
    }
}
