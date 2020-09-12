using UnityEngine;

namespace Assets.Scripts.InputHandling
{
    /// <summary>
    /// Can draw a rectangular selection area.
    /// </summary>
    internal interface ISelectionDrawer
    {
        /// <summary>
        /// Called when the selection needs to be drawn.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        void OnSelectionDrawingHandler(Vector3 start, Vector3 end);

        /// <summary>
        /// Called when the selection does not need to be drawn.
        /// </summary>
        void OnSelectionEndHandler();
    }
}
