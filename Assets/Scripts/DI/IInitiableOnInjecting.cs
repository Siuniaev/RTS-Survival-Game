namespace Assets.Scripts.DI
{
    /// <summary>
    /// Requiring additional configuration after dependency injection.
    /// </summary>
    internal interface IInitiableOnInjecting
    {
        /// <summary>
        /// Initializes this instance immediately after completion of all dependency injection.
        /// </summary>
        void OnInjected();
    }
}