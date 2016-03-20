using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.UriNavigationService.Abstractions
{
  /// <summary>
  /// Interface for UriNavigationService
  /// </summary>
  public interface IUriNavigationService
  {
        IEnumerable<string> History { get; }

        /// <summary>
        /// Gets the context instance form its type (ie: with an IoC container).
        /// </summary>
        Func<Type,object> ContextLocator { get; set; }
        
        /// <summary>
        /// Registers a navigation push and pop actions for a given association context.
        /// </summary>
        /// <param name="context">The context type that must be marked with a NavigationUriAttribute.</param>
        /// <param name="push">The action called when navigating to the view.</param>
        /// <param name="pop">The action called when exiting the view.</param>
        void RegisterView(Type context, Func<string, Task> push, Func<Task> pop);

        /// <summary>
        /// Called when a view just appeared, to get its context and update context's navigation param properties values.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="uri"></param>
        object GetViewContext(string uri);
        
        Task GoBack();

        Task Clear();

        /// <summary>
        /// Starts a navigation to the registered view.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task Navigate(string uri);
    }
}
