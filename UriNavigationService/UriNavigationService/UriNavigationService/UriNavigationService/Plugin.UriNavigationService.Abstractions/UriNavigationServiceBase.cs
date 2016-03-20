using Plugin.UriNavigationService.Abstractions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Plugin.UriNavigationService.Abstractions
{
    public abstract class UriNavigationServiceBase : IUriNavigationService
    {
        #region Properties

        public IEnumerable<string> History { get { return history.ToArray(); } }

        public Func<Type, object> ContextLocator { get; set; }

        #endregion

        #region Fields

        private Stack<string> history = new Stack<string>();

        private List<NavigationAction> navigationActions = new List<NavigationAction>();

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
        
        public void RegisterView(Type context, Func<string,Task> push, Func<Task> pop)
        {
            var attribute = (NavigationUriAttribute)context.GetTypeInfo().GetCustomAttribute(typeof(NavigationUriAttribute));
            if (attribute == null) throw new ArgumentException("The context type should have a \"NavigationUriAttribute\" attribute.");
            NavigationParam[] query, segments;
            UriNavigationServiceBase.ExtractNavigationParams(context, out query, out segments);
            this.navigationActions.Add(new NavigationAction(context, attribute.Path, query, segments,push,pop));
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

        private NavigationAction GetNavigationAction(string uri)
        {
            return navigationActions.FirstOrDefault((a) => a.IsValid(uri));
        }

        #endregion

        #region Private functions

        private static void ExtractNavigationParams(Type type, out NavigationParam[] queryResult, out NavigationParam[] segmentsResult)
        {
            var query = new List<NavigationParam>();
            var segments = new List<NavigationParam>();
            
            var typeInfo = type.GetTypeInfo();
            foreach (var p in type.GetRuntimeProperties())
            {
                var queryAtt = (NavigationQueryAttribute)p.GetCustomAttribute(typeof(NavigationQueryAttribute));

                if (queryAtt != null)
                {
                    query.Add(new NavigationParam(queryAtt.Name.ToLower(), p.PropertyType,p) { IsRequired = queryAtt.IsRequired });
                }
                else
                {
                    var segAtt = (NavigationSegmentAttribute)p.GetCustomAttribute(typeof(NavigationSegmentAttribute));

                    if (segAtt != null)
                    {
                        segments.Add(new NavigationParam(segAtt.Name.ToLower(), p.PropertyType, p) { IsRequired = true });
                    }
                }
            }

            queryResult = query.ToArray();
            segmentsResult = segments.ToArray();
        }

        #endregion
    }
}
