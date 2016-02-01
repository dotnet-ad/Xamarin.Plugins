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

        Label message;

        ActivityIndicator indicator;

        public App()
        {
            //Edit area
            input = new Entry();
            input.Text = "https://developer.xamarin.com/guides/xamarin-forms/getting-started/introduction-to-xamarin-forms/offline.pdf";

            var cacheLabel = new Label()
            {
                Text = "Use cache : ",
                TextColor = Color.White,
                VerticalOptions = LayoutOptions.Center,
            };

            useCacheSwitch = new Switch()
            {
                IsToggled = true,

            };

            var loadButton = new Button()
            {
                Text = "LOAD",
            };
            
            loadButton.Clicked += LoadButton_Clicked;

            Grid.SetColumn(cacheLabel, 1);
            Grid.SetColumn(useCacheSwitch, 2);
            Grid.SetColumn(loadButton, 3);

            var panel = new Grid()
            {
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition(),
                    new ColumnDefinition() { Width = new GridLength(1,GridUnitType.Auto) },
                    new ColumnDefinition() { Width = new GridLength(100) },
                    new ColumnDefinition() { Width = new GridLength(80) },
                },
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = { input, cacheLabel, useCacheSwitch, loadButton }
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

            message = new Label()
            {
                TextColor = Color.White,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
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
            Grid.SetRow(message, 1);
            Grid.SetRow(indicator, 1);

            // The root page of your application
            MainPage = new ContentPage
            {
                Content = new Grid()
                {
                    BackgroundColor = Color.Black,
                    RowDefinitions = new RowDefinitionCollection() { new RowDefinition() { Height = new GridLength(50) }, new RowDefinition() },
                    Children =
                    {
                        panel,
                        image,
                        prev,
                        next,
                        message,
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

            message.IsVisible = false;
            message.Text = string.Empty;

            try
            {
                image.Source = null;
                document = await CrossPdf.Current.Rasterize(input.Text, useCacheSwitch.IsToggled);
                image.Source = ImageSource.FromFile(document.Pages.First().Path);
                index = 0;
            }
            catch (Exception e)
            {
                message.Text = e.Message;
                message.IsVisible = true;
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
