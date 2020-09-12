namespace Assets.Scripts.InputHandling
{
    /// <summary>
    /// User input handler, depending on the type of handler parameter.
    /// </summary>
    /// <typeparam name="T">The any type of the handler parameter.</typeparam>
    /// <seealso cref="IInputHandler" />
    internal interface IInputHandlerGeneric<T> : IInputHandler
    {
        /// <summary>
        /// Gets or sets the condition.
        /// The handler will check the input of the specified parameter for this condition.
        /// </summary>
        /// <value>The condition.</value>
        T Condition { get; set; }
    }
}
