using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.InputHandling
{
    /// <summary>
    /// Handles user input and provides access to input handlers.
    /// </summary>
    internal interface IInputProvider
    {
        /// <summary>
        /// Get the handler of the specified type and parameter.
        /// </summary>
        /// <typeparam name="THandler">The type of the handler.</typeparam>
        /// <typeparam name="TParam">The type of the handler parameter.</typeparam>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The handler.</returns>
        Task<THandler> GetHandlerAsync<THandler, TParam>(TParam parameter)
            where THandler : IInputHandlerGeneric<TParam>, new();

        /// <summary>
        /// Gets the current cursor position.
        /// </summary>
        /// <value>The cursor position.</value>
        Vector2 CursorPosition { get; }

        /// <summary>
        /// Unsubscribes the given observer from tracking events of all input handlers.
        /// </summary>
        /// <param name="target">The unsubscribing object.</param>
        void Unsubscribe(object target);
    }
}
