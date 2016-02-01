using Plugin.Rate.Abstractions;
using System;
using System.Threading.Tasks;
using UIKit;
using System.Runtime.CompilerServices;

namespace Plugin.Rate
{
    /// <summary>
    /// Implementation for Rate
    /// </summary>
    public class RateImplementation : RateBase
    {
        const string AppStoreURLFormat = "itms-apps://itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id={0}&pageNumber=0&sortOrdering=2&mt=8";
        const string AppStoreiOS7URLFormat = "itms-apps://itunes.apple.com/app/id{0}";

        protected string AppStoreIdentifier
        {
            get
            {
                // TODO : use itunes service to get this identifier
                throw new NotImplementedException();
            }
        }

        protected override string Version
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override async Task PresentDialog(RatingLabels labels)
        {
            var alert = UIAlertController.Create(labels.Title, labels.Description, UIAlertControllerStyle.Alert);

            var rateAction = UIAlertAction.Create(labels.RateButton, UIAlertActionStyle.Default, (a) => { this.Rate(); });
            alert.AddAction(rateAction);

            if (this.CanDisableReminder)
            {
                var disableAction = UIAlertAction.Create(labels.NoThanksButton, UIAlertActionStyle.Cancel, (a) => { this.Disable(); });
                alert.AddAction(rateAction);
            }

            var remindAction = UIAlertAction.Create(labels.LaterButton, UIAlertActionStyle.Default, (a) => { });
            alert.AddAction(rateAction);

            // Presenting it
            var topController = UIApplication.SharedApplication.Delegate.GetWindow().RootViewController;
            while (topController.PresentedViewController != null)
            {
                topController = topController.PresentedViewController;
            }

            await topController.PresentViewControllerAsync(alert, true);
        }

        protected override Task PresentRatingForm()
        {
            string ratingUrl = null;
            var iOSVersion = float.Parse(UIDevice.CurrentDevice.SystemVersion);
            if (iOSVersion >= 7.0f && iOSVersion < 7.1f)
            {
                ratingUrl = AppStoreiOS7URLFormat;
            }
            else
            {
                ratingUrl = AppStoreURLFormat;
            }

            var nsUrl = new Foundation.NSUrl(ratingUrl);
            if (!UIApplication.SharedApplication.CanOpenUrl(nsUrl))
            {
                UIApplication.SharedApplication.OpenUrl(nsUrl);
            }

            return Task.FromResult(true);
        }

        protected override void SetPreference<T>(T value, [CallerMemberName] string name = null)
        {
            throw new NotImplementedException();
        }
        protected override T GetPreference<T>(T defaultValue, [CallerMemberName] string name = null)
        {
            throw new NotImplementedException();
        }
    }
}