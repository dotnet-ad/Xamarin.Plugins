using Plugin.Pdf;
using Plugin.Pdf.Abstractions;
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

        Switch useCacheSwitch;

        Entry input;

        ActivityIndicator indicator;

        public App()
        {
            //Edit area
            input = new Entry();
            input.Text = "https://developer.xamarin.com/guides/xamarin-forms/getting-started/introduction-to-xamarin-forms/offline.pdf";

            useCacheSwitch = new Switch()
            {
                IsToggled = true,
            };

            var loadButton = new Button()
            {
                Text = "LOAD",
            };
            
            loadButton.Clicked += LoadButton_Clicked;

            var panel = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Horizontal,
                Children = { input, useCacheSwitch, loadButton }
            };

            //Content area
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

            indicator = new ActivityIndicator()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };


            prev.Clicked += Prev_Clicked;
            next.Clicked += Next_Clicked;

            // Grid layout
            Grid.SetRow(image, 1);
            Grid.SetRow(prev, 1);
            Grid.SetRow(next, 1);
            Grid.SetRow(indicator, 1);

            // The root page of your application
            MainPage = new ContentPage
            {
                Content = new Grid()
                {
                    RowDefinitions = new RowDefinitionCollection() { new RowDefinition() { Height = new GridLength(50) }, new RowDefinition() },
                    Children =
                    {
                        panel,
                        image,
                        prev,
                        next,
                        indicator,
                    }
                }
            };

            this.Generate();
        }

        private void LoadButton_Clicked(object sender, EventArgs e)
        {
            this.Generate();
        }

        private void Next_Clicked(object sender, EventArgs e)
        {
            index = Math.Min(document.Pages.Count() - 1, index + 1);
            image.Source = ImageSource.FromFile(document.Pages.ElementAt(index).Path);
        }

        private void Prev_Clicked(object sender, EventArgs e)
        {
            index = Math.Max(0, index - 1);
            image.Source = ImageSource.FromFile(document.Pages.ElementAt(index).Path);
        }

        private int index;
        private PdfDocument document;

        private async void Generate()
        {
            this.indicator.IsRunning = true;
            this.indicator.IsVisible = true;

            try
            {
                image.Source = null;
                document = await CrossPdf.Current.Rasterize(input.Text, useCacheSwitch.IsToggled);
                image.Source = ImageSource.FromFile(document.Pages.First().Path);
                index = 0;
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                this.indicator.IsRunning = false;
                this.indicator.IsVisible = false;
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
