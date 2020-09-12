using System;
using UnityEngine;

namespace Assets.Scripts.InputHandling
{
    /// <summary>
    /// Performs the selecting.
    /// </summary>
    internal interface ISelector
    {
        /// <summary>
        /// Called when the selection needs to be blocked.
        /// </summary>
        /// <param name="block">if set to <c>true</c> the selecting is blocked.</param>
        void OnSelectionBlockHandler(bool block);

        /// <summary>
        /// Occurs when select the selectable.
        /// </summary>
        event Action<ISelectable> OnSelect;

        /// <summary>
        /// Occurs when the selection needs to be drawn.
        /// </summary>
        event Action<Vector3, Vector3> OnSelectionDrawing;

        /// <summary>
        /// Occurs when the selection has ended.
        /// </summary>
        event Action OnSelectionEnd;
    }
}
