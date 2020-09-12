using UnityEngine;

namespace Assets.Scripts.DataStructures
{
    /// <summary>
    /// Struct for checking whether a point is in a given rectangle area.
    /// </summary>
    /// <seealso cref="IPointChecker" />
    internal readonly struct PointCheckerInRectangle : IPointChecker
    {
        private readonly Vector3 min, max;

        /// <summary>
        /// Initializes a new instance of the <see cref="PointCheckerInRectangle"/> struct.
        /// </summary>
        /// <param name="min">The minimum point.</param>
        /// <param name="max">The maximum point.</param>
        public PointCheckerInRectangle(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Determines whether the point is in the area.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///   <c>true</c> if the point is in the area; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInArea(Vector3 point)
        {
            return point.x >= min.x && point.x <= max.x
                && point.z >= min.z && point.z <= max.z;
        }
    }
}
