using UnityEngine;

namespace Assets.Scripts.DataStructures
{
    /// <summary>
    /// Struct for checking whether a point is in a given circle area.
    /// </summary>
    /// <seealso cref="IPointChecker" />
    internal readonly struct PointCheckerInCircle : IPointChecker
    {
        private readonly Vector3 center;
        private readonly float radius;

        /// <summary>
        /// Initializes a new instance of the <see cref="PointCheckerInCircle"/> struct.
        /// </summary>
        /// <param name="center">The center point.</param>
        /// <param name="radius">The radius.</param>
        public PointCheckerInCircle(Vector3 center, float radius)
        {
            this.center = center;
            this.radius = radius;
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
            return Mathf.Pow(point.x - center.x, 2) + Mathf.Pow(point.z - center.z, 2) <= radius * radius;
        }
    }
}
