namespace Plugin.Rate.Abstractions
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for Rate
    /// </summary>
    public interface IRate
    {
        /// <summary>
        /// The time span before the dialog is shown to the user again after he selected "Remind me later" option.
        /// </summary>
        TimeSpan ReminderSpan { get; set; }

        /// <summary>
        /// Indicates whether the user must rate again the app after a version change.
        /// </summary>
        bool MustRateEachVersion { get; set; }

        /// <summary>
        /// Indicates whether the user can disable the rating dialog with a specific option.
        /// </summary>
        bool CanDisableReminder { get; set; }

        /// <summary>
        /// This shows the rating dialog.
        /// </summary>
        /// <param name="force">If True the dialog will be shown, else it will only be shown when needed (not disabled, not delayed).</param>
        /// <returns>An asynchronous task.</returns>
        Task ShowDialog(bool force);
    }
}
