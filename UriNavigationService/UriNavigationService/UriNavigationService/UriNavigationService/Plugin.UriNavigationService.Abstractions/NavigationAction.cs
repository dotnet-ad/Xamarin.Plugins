using Plugin.UriNavigationService.Abstractions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.UriNavigationService.Abstractions
{
    public class NavigationAction
    {
        public NavigationAction(Type context, string path, IEnumerable<NavigationParam> query, IEnumerable<NavigationParam> segments, Func<string, Task> push, Func<Task> pop)
        {
            this.Context = context;
            this.Query = query.ToArray();
            this.Segments = segments.ToArray();
            this.Push = push;
            this.Pop = pop;
        }

        public string Path { get; private set; }

        public Type Context { get; private set; }

        public IEnumerable<NavigationParam> Segments { get; private set; }

        public IEnumerable<NavigationParam> Query { get; private set; }
        
        public Func<string, Task> Push { get; private set; }

        public Func<Task> Pop { get; private set; }

        public bool IsValid(string uri)
        {
            throw new NotImplementedException();
        }

    }
}
