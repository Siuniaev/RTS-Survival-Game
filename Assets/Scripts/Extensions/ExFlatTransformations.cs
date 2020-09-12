using UnityEngine;

namespace Assets.Scripts.Extensions
{
    /// <summary>
    /// Extension methods for flat transformations (with ignoring the Y-axis).
    /// </summary>
    public static class ExFlatTransformations
    {
        /// <summary>
        /// Calculates the flat distance between two points.
        /// </summary>
        /// <param name="first">The first point.</param>
        /// <param name="second">The second point.</param>
        /// <returns>The flat distance.</returns>
        public static float FlatDistanceTo(this Vector3 first, Vector3 second)
        {
            var a = new Vector2(first.x, first.z);
            var b = new Vector2(second.x, second.z);

            return Vector2.Distance(a, b);
        }

        /// <summary>
        /// Moves a point toward the target point with a given speed, ignoring the Y-axis.
        /// </summary>
        /// <param name="start">The started point.</param>
        /// <param name="target">The target point.</param>
        /// <param name="speed">The moving speed.</param>
        /// <returns>The moved point.</returns>
        public static Vector3 FlatMoveTowardsTo(this Vector3 start, Vector3 target, float speed)
        {
            var targetYAligned = new Vector3(target.x, start.y, target.z);

            return Vector3.MoveTowards(start, targetYAligned, speed);
        }

        /// <summary>
        /// Rotates a point around toward the target direction with a given speed, ignoring the Y-axis.
        /// </summary>
        /// <param name="rotation">The started rotation.</param>
        /// <param name="target">The target direction.</param>
        /// <param name="speed">The speed.</param>
        /// <returns>The resulting rotation.</returns>
        public static Quaternion FlatRotateTowardsTo(this Quaternion rotation, Vector3 target, float speed)
        {
            target.y = 0;

            if (target == Vector3.zero)
                return rotation;

            var targetRot = Quaternion.LookRotation(target);

            return Quaternion.Slerp(rotation, targetRot, speed);
        }
    }
}
