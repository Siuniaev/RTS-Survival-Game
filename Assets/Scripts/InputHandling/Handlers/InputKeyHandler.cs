using System;
using UnityEngine;

namespace Assets.Scripts.InputHandling.Handlers
{
    /// <summary>
    /// User input buttons handler.
    /// </summary>
    /// <seealso cref="IInputHandlerGeneric{T}.KeyCode}" />
    internal class InputKeyHandler : IInputHandlerGeneric<KeyCode>
    {
        /// <summary>
        /// Occurs when the key has pressed.
        /// </summary>
        public event Action OnKeyPressed;

        /// <summary>
        /// Occurs when the key has downed.
        /// </summary>
        public event Action OnKeyDown;

        /// <summary>
        /// Gets or sets the condition.
        /// The handler will check the input of the specified parameter for this condition.
        /// </summary>
        /// <value>The condition.</value>
        public KeyCode Condition { get; set; }

        /// <summary>
        /// Checks pressed keys.
        /// </summary>
        public void CheckInput()
        {
            if (Input.GetKey(Condition))
                OnKeyPressed?.Invoke();

            if (Input.GetKeyDown(Condition))
                OnKeyDown?.Invoke();
        }

        /// <summary>
        /// Unsubscribes the given observer from tracking events of this handler.
        /// </summary>
        /// <param name="target">The unsubscribing object.</param>
        /// <exception cref="ArgumentNullException">target</exception>
        public void Unsubscribe(object target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            UnsubscribeAllOrTarget(target);
        }

        /// <summary>
        /// Unsubscribes all observers for the events of this handler.
        /// </summary>
        public void UnsubscribeAll() => UnsubscribeAllOrTarget();

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="String" /> that represents this instance.</returns>
        public override string ToString() => $"{GetType().Name} {Condition}";

        /// <summary>
        /// Unsubscribes all observers or the given observer from tracking events of this handler.
        /// </summary>
        /// <param name="target">The unsubscribing object.</param>
        private void UnsubscribeAllOrTarget(object target = null)
        {
            if (OnKeyPressed != null)
                foreach (Action action in OnKeyPressed.GetInvocationList())
                    if (target == null || action.Target == target)
                        OnKeyPressed -= action;

            if (OnKeyDown != null)
                foreach (Action action in OnKeyDown.GetInvocationList())
                    if (target == null || action.Target == target)
                        OnKeyDown -= action;
        }
    }
}
