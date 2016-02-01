namespace Plugin.UriNavigationService.Abstractions.Actions
{
    using Attributes;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public abstract class NavigationActionBase : INavigationAction
    {
        public NavigationActionBase(Type context, IUriNavigationService service)
        {
            this.Context = context;

            var attribute = (NavigationUriAttribute)context.GetTypeInfo().GetCustomAttribute(typeof(NavigationUriAttribute));
            if (attribute == null) throw new ArgumentException("The context type should have a \"NavigationUriAttribute\" attribute.");
            this.Path = attribute.Path;

            NavigationParam[] query, segments;
            NavigationActionBase.ExtractNavigationParams(context, out query, out segments);
            this.Query = query.ToArray();
            this.Segments = segments.ToArray();

            service.Register(this);
        }

        public string Path { get; private set; }

        public Type Context { get; private set; }

        public IEnumerable<NavigationParam> Segments { get; private set; }

        public IEnumerable<NavigationParam> Query { get; private set; }

        public bool IsValid(string uri)
        {
            throw new NotImplementedException();
        }

        public abstract Task Push(string uri);

        public abstract Task Pop();

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
                    query.Add(new NavigationParam(queryAtt.Name.ToLower(), p.PropertyType, p) { IsRequired = queryAtt.IsRequired });
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
