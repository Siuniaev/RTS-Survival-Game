using UnityEngine;

namespace Assets.Scripts.DataStructures
{
    /// <summary>
    /// Checks whether a point is in a given area.
    /// </summary>
    internal interface IPointChecker
    {
        /// <summary>
        /// Determines whether the point is in the area.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>
        ///   <c>true</c> if the point is in the area; otherwise, <c>false</c>.
        /// </returns>
        bool IsInArea(Vector3 point);
    }
}
