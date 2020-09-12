namespace Assets.Scripts.InputHandling
{
    /// <summary>
    /// User input handler.
    /// </summary>
    internal interface IInputHandler
    {
        /// <summary>
        /// Checks the current input.
        /// </summary>
        void CheckInput();

        /// <summary>
        /// Unsubscribes the given observer from tracking events of this handler.
        /// </summary>
        /// <param name="target">The unsubscribing object.</param>
        void Unsubscribe(object target);

        /// <summary>
        /// Unsubscribes all observers for the events of this handler.
        /// </summary>
        void UnsubscribeAll();
    }
}
