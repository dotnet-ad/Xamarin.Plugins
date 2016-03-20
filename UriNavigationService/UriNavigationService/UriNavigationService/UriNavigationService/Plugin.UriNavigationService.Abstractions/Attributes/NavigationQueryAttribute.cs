using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.UriNavigationService.Abstractions.Attributes
{
    public class NavigationQueryAttribute : Attribute
    {
        public NavigationQueryAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }

        public bool IsRequired { get; set; }
    }
}
