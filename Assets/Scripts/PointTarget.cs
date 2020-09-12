using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// The point in space that can be used as a target.
    /// </summary>
    /// <seealso cref="ITarget" />
    internal class PointTarget : ITarget
    {
        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>The position.</value>
        public Vector3 Position { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointTarget"/> class.
        /// </summary>
        /// <param name="point">The point coordinates.</param>
        public PointTarget(Vector3 point) => Position = point;

        /// <summary>
        /// Highlights the target.
        /// </summary>
        /// <param name="isHighlighted">if set to <c>true</c> is highlighted.</param>
        public void HighlightTarget(bool isHighlighted) { }
    }
}
