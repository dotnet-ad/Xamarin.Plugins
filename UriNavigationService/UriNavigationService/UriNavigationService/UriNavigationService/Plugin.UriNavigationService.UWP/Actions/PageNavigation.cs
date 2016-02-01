namespace Plugin.UriNavigationService.Actions
{
    using Abstractions.Actions;
    using Abstractions.Attributes;
    using System;
    using System.Threading.Tasks;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public class PageNavigation<T> : NavigationActionBase where T : Page
    {
        public PageNavigation(Type context) : base(context, CrossUriNavigationService.Current)
        {

        }

        public override Task Push(string uri)
        {
            var page = typeof(T);
            var frame = (Frame)Window.Current.Content;
            if(frame != null)
            {
                frame.Navigate(page, uri);
            }
            return Task.FromResult(true);
        }

        public override Task Pop()
        {
            var frame = (Frame)Window.Current.Content;
            if(frame != null && frame.CanGoBack)
            {
                frame.GoBack();
            }
            return Task.FromResult(true);
        }

    }
}
