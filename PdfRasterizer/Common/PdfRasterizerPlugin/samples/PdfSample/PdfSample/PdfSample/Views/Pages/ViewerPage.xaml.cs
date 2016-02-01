using Plugin.PdfRasterizer;
using Plugin.PdfRasterizer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PdfSample.Views.Pages
{
    public partial class ViewerPage : ContentPage
    {
        private const string defaultPath = "https://developer.xamarin.com/guides/xamarin-forms/getting-started/introduction-to-xamarin-forms/offline.pdf";

        public ViewerPage()
        {
            InitializeComponent();
            pathEntry.Text = defaultPath;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.StartRendering();
        }
        
        private int index;
        private PdfDocument document;

        private async void StartRendering()
        {
            if(!this.activityIndicator.IsRunning)
            {
                this.pageImage.Source = null;

                this.UpdatePageState();

                this.activityIndicator.IsRunning = true;
                this.activityIndicator.IsVisible = true;

                errorMessage.IsVisible = false;
                errorMessage.Text = string.Empty;

                try
                {
                    pageImage.Source = null;
                    this.document = await CrossPdfRasterizer.Current.Rasterize(pathEntry.Text, cacheSwitch.IsToggled);
                    pageImage.Source = ImageSource.FromFile(document.Pages.First().Path);
                    index = 0;
                    this.UpdatePageState();
                }
                catch (Exception e)
                {
                    errorMessage.Text = e.Message;
                    errorMessage.IsVisible = true;
                }
                finally
                {
                    this.activityIndicator.IsRunning = false;
                    this.activityIndicator.IsVisible = false;
                }
            }
        }


        private void OnLoadClicked(object sender, EventArgs e)
        {
            this.StartRendering();
        }

        private void OnNextClicked(object sender, EventArgs e)
        {
            this.index = Math.Min(document.Pages.Count() - 1, index + 1);
            this.UpdatePageState();
        }

        private void OnPreviousClicked(object sender, EventArgs e)
        {
            this.index = Math.Max(0, index - 1);
            this.UpdatePageState();
        }

        private void UpdatePageState()
        {
            if(this.document == null)
            {
                this.pageImage.Source =  null ;
                this.previousButton.IsEnabled = false;
                this.nextButton.IsEnabled = false;
                this.pageNumberLabel.Text = string.Empty;
            }
            else
            {
                var total = this.document.Pages.Count();
                this.pageImage.Source =  ImageSource.FromFile(document.Pages.ElementAt(index).Path);
                this.previousButton.IsEnabled = index > 0;
                this.nextButton.IsEnabled = index < total - 1;
                this.pageNumberLabel.Text = string.Format($"{this.index + 1}/{total}");
            }
        }
    }
}
