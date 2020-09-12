using Assets.Scripts.UnityThreadUtil;
using System;
using System.Threading;

namespace Assets.Scripts
{
    /// <summary>
    /// The сountdown timer using <see cref="Timer" /> inside, ticking every second.
    /// Do not forget to Dispose() this instance on OnDestory() where you will use it! Otherwise it will continue to tick in the editor after stopped game.
    /// </summary>
    /// <seealso cref="IDisposable" />
    internal class TimerCountdown : IDisposable
    {
        private readonly Timer timer;
        private readonly WaitForUpdate unityUpdateWaiter;
        private float countdown;

        public readonly object Countdowned;
        public readonly string Description;

        /// <summary>
        /// Occurs when the timer ticks.
        /// </summary>
        public event Action<float> OnTick;

        /// <summary>
        /// Occurs when the timer has finished ticking.
        /// </summary>
        public event Action<object> OnFinish;

        /// <summary>
        /// Gets a value indicating whether this instance is ticking.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is ticking; otherwise, <c>false</c>.
        /// </value>
        public bool IsTicking => countdown > 0f;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerCountdown"/> class.
        /// </summary>
        /// <param name="countdowned">The countdowned object.</param>
        /// <param name="description">The description.</param>
        /// <param name="countdownInitial">The initial value in seconds.</param>
        /// <exception cref="ArgumentNullException">countdowned</exception>
        public TimerCountdown(object countdowned, string description = "", float countdownInitial = 0)
        {
            Countdowned = countdowned ?? throw new ArgumentNullException(nameof(countdowned));
            Description = description;
            countdown = countdownInitial;
            unityUpdateWaiter = new WaitForUpdate();
            timer = new Timer(CountdownTick);

            if (countdownInitial > 0)
                StartCountdown(countdownInitial);
        }

        /// <summary>
        /// Disposes the timer.
        /// </summary>
        public void Dispose() => timer.Dispose();

        /// <summary>
        /// Starts the countdown with the specified initial value in seconds.
        /// </summary>
        /// <param name="countdownInitial">The initial value in seconds.</param>
        /// <exception cref="ArgumentOutOfRangeException">countdownInitial - Must be greater than or equal to 0.</exception>
        public void StartCountdown(float countdownInitial)
        {
            if (countdownInitial < 0)
                throw new ArgumentOutOfRangeException(nameof(countdownInitial), "Must be greater than or equal to 0.");

            countdown = countdownInitial;
            double tickTime = countdown > 1 ? 1 : countdown;
            timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(tickTime));
        }

        /// <summary>
        /// Stops the countdown.
        /// </summary>
        public void StopCountdown()
        {
            countdown = 0;
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            OnFinish?.Invoke(Countdowned);
        }

        /// <summary>
        /// Notifies that the timer has ticked.
        /// Stops the countdown when it is finished.
        /// </summary>
        /// <param name="obj">The countdowned object.</param>
        private async void CountdownTick(object obj)
        {
            await unityUpdateWaiter; // Returning to the Unity thread.

            if (countdown > 0)
            {
                OnTick?.Invoke(countdown);
                countdown -= 1;
            }
            else
                StopCountdown();
        }
    }
}
