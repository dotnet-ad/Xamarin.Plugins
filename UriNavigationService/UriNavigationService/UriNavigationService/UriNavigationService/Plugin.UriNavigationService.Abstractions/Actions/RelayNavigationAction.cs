using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.UriNavigationService.Abstractions.Actions
{
    public class RelayNavigationAction : NavigationActionBase
    {
        public RelayNavigationAction(Func<string,Task> push, Func<Task> pop, Type context, IUriNavigationService service) : base(context,service)
        {
            this.push = push;
            this.pop = pop;
        }

        private Func<Task> pop;

        private Func<string, Task> push;

        public override Task Pop()
        {
            if (this.pop != null) return this.pop();
            return Task.FromResult(true);
        }

        public override Task Push(string uri)
        {
            if (this.push != null) return this.push(uri);
            return Task.FromResult(true);
        }
    }
}
