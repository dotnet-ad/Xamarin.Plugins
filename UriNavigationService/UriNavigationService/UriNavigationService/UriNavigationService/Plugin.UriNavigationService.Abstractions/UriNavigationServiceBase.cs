namespace Plugin.UriNavigationService.Abstractions
{
    using Actions;
    using System.Threading.Tasks;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class UriNavigationServiceBase : IUriNavigationService
    {
        #region Properties

        public IEnumerable<string> History { get { return history.ToArray(); } }

        public Func<Type, object> ContextLocator { get; set; }

        #endregion

        #region Fields

        private Stack<string> history = new Stack<string>();

        private List<INavigationAction> navigationActions = new List<INavigationAction>();

        #endregion

        #region Public methods

        public Task Clear()
        {
            throw new NotImplementedException();
        }

        public Task GoBack()
        {
            var lastUri = this.history.Pop();
            var action = this.GetNavigationAction(lastUri);
            return action.Pop();
        }

        public Task Navigate(string uri)
        {
            this.history.Push(uri);
            var action = this.GetNavigationAction(uri);
            return action.Push(uri);
        }
        
        public void Register(INavigationAction action)
        {
            this.navigationActions.Add(action);
        }

        public INavigationAction CreateCustomAction(Type context, Func<string, Task> push, Func<Task> pop)
        {
            return new RelayNavigationAction(push,pop,context, this);
        }

        public object GetViewContext(string uri)
        {
            var action = this.GetNavigationAction(uri);
            var context = this.ContextLocator(action.Context);

            // Exctracting query values
            var queryValues = uri.GetQueryValues();
            foreach (var queryParam in action.Query)
            {
                if (queryValues.ContainsKey(queryParam.Name))
                {
                    var value = queryValues[queryParam.Name];
                    queryParam.SetContextValue(context, value);
                }
                else if (queryParam.IsRequired)
                {
                    throw new ArgumentException($"Required query parameter \"{queryParam.Name}\" is not present");
                }
            }

            // TODO : extract segment values

            throw new NotImplementedException();

            return context;
        }

        #endregion

        #region Private methods

        private INavigationAction GetNavigationAction(string uri)
        {
            return navigationActions.FirstOrDefault((a) => a.IsValid(uri));
        }

        #endregion

    }
}
