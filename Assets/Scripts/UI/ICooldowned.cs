namespace Assets.Scripts.UI
{
    /// <summary>
    /// Has the <see cref="TimerCountdown" /> instance.
    /// </summary>
    internal interface ICooldowned
    {
        /// <summary>
        /// Gets the countdown timer.
        /// </summary>
        /// <value>The timer.</value>
        TimerCountdown Timer { get; }
    }
}
