using System;
using UnityEngine;

namespace Assets.Scripts.InputHandling.Handlers
{
    /// <summary>
    /// User input mouse buttons handler.
    /// </summary>
    /// <seealso cref="IInputHandlerGeneric{Int32}" />
    internal class InputMouseButtonsHandler : IInputHandlerGeneric<int>
    {
        private int condition;

        /// <summary>
        /// Occurs when the mouse button has downed.
        /// </summary>
        public event Action OnMouseButtonDown;

        /// <summary>
        /// Occurs when the mouse button has upped.
        /// </summary>
        public event Action OnMouseButtonUp;

        /// <summary>
        /// Occurs when the mouse button being pressed.
        /// </summary>
        public event Action OnMouseButtonBeingPressed;

        /// <summary>
        /// Gets or sets the condition.
        /// The handler will check the input of the specified parameter for this condition.
        /// </summary>
        /// <value>The condition.</value>
        /// <exception cref="ArgumentOutOfRangeException">Condition - Wrong mouse button.</exception>
        public int Condition
        {
            get => condition;

            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Condition), "Wrong mouse button.");

                condition = value;
            }
        }

        /// <summary>
        /// Checks pressed mouse buttons.
        /// </summary>
        public void CheckInput()
        {
            // Call order is important here ↓↓↓.

            if (Input.GetMouseButtonUp(Condition))
                OnMouseButtonUp?.Invoke();

            if (Input.GetMouseButton(Condition))
                OnMouseButtonBeingPressed?.Invoke();

            if (Input.GetMouseButtonDown(Condition))
                OnMouseButtonDown?.Invoke();
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
            if (OnMouseButtonDown != null)
                foreach (Action action in OnMouseButtonDown.GetInvocationList())
                    if (target == null || action.Target == target)
                        OnMouseButtonDown -= action;

            if (OnMouseButtonBeingPressed != null)
                foreach (Action action in OnMouseButtonBeingPressed.GetInvocationList())
                    if (target == null || action.Target == target)
                        OnMouseButtonBeingPressed -= action;

            if (OnMouseButtonUp != null)
                foreach (Action action in OnMouseButtonUp.GetInvocationList())
                    if (target == null || action.Target == target)
                        OnMouseButtonUp -= action;
        }
    }
}
