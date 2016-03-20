namespace Plugin.UriNavigationService.Abstractions
{
    using System;
    using System.Linq;
    using System.Reflection;

    public class NavigationParam
    {
        public NavigationParam(string name, Type type, PropertyInfo property)
        {
            this.Name = name;
            this.Type = type;
            this.ContextProperty = property;
        }

        public static readonly Type[] AuthorizedTypes = new []
        {
            typeof(string),
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(bool)
        };

        public string Name { get; private set; }
        
        private Type type;

        public Type Type
        {
            get { return type; }
            private set
            {
                if (!AuthorizedTypes.Contains(value)) throw new ArgumentException("Navigation parameters could only be : " + string.Join(",", AuthorizedTypes.Select((t) => t.Name)));
                type = value;
            }
        }

        public PropertyInfo ContextProperty { get; private set; }
        
        public bool IsRequired { get; set; }

        public void SetContextValue(object context, string pathValue)
        {
            object value = pathValue;

            if (this.Type == typeof(short)) { value = short.Parse(pathValue); }
            if (this.Type == typeof(int)) { value = int.Parse(pathValue); }
            if (this.Type == typeof(long)) { value = long.Parse(pathValue); }
            if (this.Type == typeof(bool)) { value = bool.Parse(pathValue); }

            this.ContextProperty.SetMethod.Invoke(context, new object[] { value })
        }
    }
}
