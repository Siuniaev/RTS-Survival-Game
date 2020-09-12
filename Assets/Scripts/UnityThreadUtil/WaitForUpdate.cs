namespace Assets.Scripts.UnityThreadUtil
{
    /// <summary>
    /// The awaitable type that schedules to continue executing code in the Unity thread.
    /// </summary>
    internal class WaitForUpdate
    {
        /// <summary>
        /// Gets the scheduled in the Unity thread awaiter.
        /// </summary>
        /// <returns>The Unity thread awaiter.</returns>
        public UnityThreadAwaiter GetAwaiter() => new UnityThreadAwaiter();
    }
}
