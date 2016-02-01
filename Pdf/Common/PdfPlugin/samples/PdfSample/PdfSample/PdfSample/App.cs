using Plugin.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace PdfSample
{
    public class App : Application
    {
        Image image;

        public App()
        {
            image = new Image();

            var prev = new Button()
            {
                Text = "<",
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
            };

            var next = new Button()
            {
                Text = ">",
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
            };

            prev.Clicked += Prev_Clicked;
            next.Clicked += Next_Clicked;

            // The root page of your application
            MainPage = new ContentPage
            {
                Content = new Grid()
                {
                    Children =
                    {
                        image,
                        prev,
                        next,
                    }
                }
            };

            this.Generate();
        }

        private void Next_Clicked(object sender, EventArgs e)
        {
            index = Math.Min(paths.Length - 1, index + 1);
            image.Source = ImageSource.FromFile(paths[index]);
        }

        private void Prev_Clicked(object sender, EventArgs e)
        {
            index = Math.Max(0, index - 1);
            image.Source = ImageSource.FromFile(paths[index]);
        }

        private int index;
        private string[] paths;

        private async void Generate()
        {
            try
            {
                paths = await CrossPdf.Current.DownloadAndRender("https://developer.xamarin.com/guides/xamarin-forms/getting-started/introduction-to-xamarin-forms/offline.pdf", "offline", 72);
                image.Source = ImageSource.FromFile(paths.First());
                index = 0;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
