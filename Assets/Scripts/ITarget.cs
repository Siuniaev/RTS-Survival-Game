using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Has position coordinates and can be highlighted as a target.
    /// </summary>
    internal interface ITarget
    {
        /// <summary>
        /// Gets the position point.
        /// </summary>
        /// <value>The position point.</value>
        Vector3 Position { get; }

        /// <summary>
        /// Highlights the target.
        /// </summary>
        /// <param name="isHighlighted">if set to <c>true</c> is highlighted.</param>
        void HighlightTarget(bool isHighlighted);
    }
}
