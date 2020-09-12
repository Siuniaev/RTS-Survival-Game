namespace Assets.Scripts.DI
{
    /// <summary>
    /// Makes injections into newly created objects in real-time.
    /// </summary>
    internal interface IDependencyInjector
    {
        /// <summary>
        /// Injects dependencies into the specified object.
        /// </summary>
        /// <param name="injectable">The injectable object.</param>
        void MakeInjections(object injectable);
    }
}