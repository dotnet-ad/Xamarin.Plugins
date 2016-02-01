using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Plugin.Rate.Abstractions;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace Plugin.Rate
{
    /// <summary>
    /// Implementation for Feature
    /// </summary>
    public class RateImplementation : RateBase
    {
        protected override string Version
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override T GetPreference<T>(T defaultValue, [CallerMemberName] string name = null)
        {
            throw new NotImplementedException();
        }

        protected override async Task PresentDialog(RatingLabels labels)
        {
            var dialog = new ContentDialog()
            {
                Title = labels.Title,
                MaxWidth = Window.Current.Bounds.Width // required for mobile
            };

            var panel = new StackPanel();

            panel.Children.Add(new TextBlock
            {
                Text = labels.Description,
                TextWrapping = TextWrapping.Wrap,
            });


            // Add Buttons
            dialog.PrimaryButtonText = labels.RateButton;
            dialog.PrimaryButtonClick += delegate { this.Rate(); };

            if (this.CanDisableReminder)
            {
                var disableButton = new Button
                {
                    Content = labels.NoThanksButton,
                };
                disableButton.Click += (e,a) => { this.Disable(); };
                panel.Children.Add(disableButton);
            }

            dialog.SecondaryButtonText = labels.LaterButton;

            // Showing the dialog
            await dialog.ShowAsync();
        }

        protected override Task PresentRatingForm()
        {
            throw new NotImplementedException();
        }

        protected override void SetPreference<T>(T value, [CallerMemberName] string name = null)
        {
            throw new NotImplementedException();
        }
    }
}