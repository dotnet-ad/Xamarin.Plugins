using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.UriNavigationService.Abstractions.Attributes
{
    public class NavigationUriAttribute : Attribute
    {
        public NavigationUriAttribute(string path)
        {
            this.Path = path;
        }

        public string Path { get; set; }
    }
}
