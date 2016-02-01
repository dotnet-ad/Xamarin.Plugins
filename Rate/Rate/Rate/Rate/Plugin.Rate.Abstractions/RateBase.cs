namespace Plugin.Rate.Abstractions
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public abstract class RateBase : IRate
    {
        public RateBase()
        {
        }

        #region Properties

        public bool CanDisableReminder { get; set; } = true;

        public TimeSpan ReminderSpan { get; set; } = TimeSpan.FromDays(5);

        public bool MustRateEachVersion { get; set; } = true;

        /// <summary>
        /// The last date time the alert shown up.
        /// </summary>
        private DateTime LastShownDate
        {
            get { return this.GetPreference(DateTime.MinValue); }
            set { this.SetPreference(value); }
        }

        /// <summary>
        /// Indicates whether the user disabled ratings.
        /// </summary>
        private bool IsDisabled
        {
            get { return this.GetPreference(false); }
            set { this.SetPreference(value); }
        }

        /// <summary>
        /// The last application version the user rated.
        /// </summary>
        private string LastRatedVersion
        {
            get { return this.GetPreference(string.Empty); }
            set { this.SetPreference(value); }
        }

        /// <summary>
        /// Indicates whether the dialog must be shown to the user.
        /// </summary>
        protected bool DialogMustBeShown
        {
            get
            {
                if(this.IsDisabled)
                {
                    return false;
                }

                var checkVersion = (!MustRateEachVersion && string.IsNullOrEmpty(this.LastRatedVersion) || (MustRateEachVersion && this.LastRatedVersion != this.Version));
                var checkDate = (this.LastShownDate < DateTime.Now - ReminderSpan);
                return checkVersion && checkDate;
            }
        }

        #endregion

        #region Methods

        public async Task ShowDialog(bool force)
        {
            if (DialogMustBeShown)
            {
                var labels = this.GetLabels("en");
                await this.PresentDialog(labels);
                this.LastShownDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Gets the labels used for alert.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        protected RatingLabels GetLabels(string culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The user selected the disable option.
        /// </summary>
        protected void Disable()
        {
            this.IsDisabled = true;
        }

        /// <summary>
        /// The user selected the rate option.
        /// </summary>
        protected void Rate()
        {
            this.LastRatedVersion = this.Version;
            this.PresentRatingForm();
        }

        #endregion

        #region Asbtract methods

        /// <summary>
        /// Gets the current application version.
        /// </summary>
        protected abstract string Version { get; }

        /// <summary>
        /// Shows the alert dialog with the options.
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        protected abstract Task PresentDialog(RatingLabels labels);

        /// <summary>
        /// Shows thepage where the user can actually rate the application.
        /// </summary>
        /// <returns></returns>
        protected abstract Task PresentRatingForm();

        /// <summary>
        /// Sets a local preference value that will be saved.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        protected abstract void SetPreference<T>(T value, [CallerMemberName]string name = null);

        /// <summary>
        /// Loads a saved preference value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultValue"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected abstract T GetPreference<T>(T defaultValue, [CallerMemberName]string name = null);

        #endregion
    }
}
