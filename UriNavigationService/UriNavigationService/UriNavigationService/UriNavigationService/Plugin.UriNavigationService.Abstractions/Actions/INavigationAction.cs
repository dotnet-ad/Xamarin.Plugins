using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.UriNavigationService.Abstractions.Actions
{
    public interface INavigationAction
    {
        string Path { get;  }

        Type Context { get;  }

        IEnumerable<NavigationParam> Segments { get; }

        IEnumerable<NavigationParam> Query { get; }

        Task Push(string uri);

        Task Pop();

        bool IsValid(string uri);

    }
}
