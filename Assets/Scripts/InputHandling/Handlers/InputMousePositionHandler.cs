using System;
using UnityEngine;

namespace Assets.Scripts.InputHandling.Handlers
{
    /// <summary>
    /// User input mouse position handler.
    /// </summary>
    /// <seealso cref="IInputHandlerGeneric{System.Predicate{UnityEngine.Vector3}}" />
    internal class InputMousePositionHandler : IInputHandlerGeneric<Predicate<Vector3>>
    {
        private Predicate<Vector3> condition;

        /// <summary>
        /// Occurs when [on mouse position satisfy].
        /// </summary>
        public event Action OnMousePositionSatisfy;

        /// <summary>
        /// Gets or sets the condition.
        /// The handler will check the input of the specified parameter for this condition.
        /// </summary>
        /// <value>The condition.</value>
        /// <exception cref="ArgumentNullException">Condition</exception>
        public Predicate<Vector3> Condition
        {
            get => condition;
            set => condition = value ?? throw new ArgumentNullException(nameof(Condition));
        }

        /// <summary>
        /// Checks mouse position.
        /// </summary>
        public void CheckInput()
        {
            if (Condition(Input.mousePosition))
                OnMousePositionSatisfy?.Invoke();
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

            if (OnMousePositionSatisfy != null)
                foreach (Action action in OnMousePositionSatisfy.GetInvocationList())
                    if (action.Target == target)
                        OnMousePositionSatisfy -= action;
        }

        /// <summary>
        /// Unsubscribes all observers for the events of this handler.
        /// </summary>
        public void UnsubscribeAll()
        {
            if (OnMousePositionSatisfy != null)
                foreach (Action action in OnMousePositionSatisfy.GetInvocationList())
                    OnMousePositionSatisfy -= action;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="String" /> that represents this instance.</returns>
        public override string ToString() => $"{GetType().Name} {Condition}";
    }
}
