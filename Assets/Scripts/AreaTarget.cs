using System;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// The circular area that can be used as a target.
    /// </summary>
    /// <seealso cref="PointTarget" />
    internal class AreaTarget : PointTarget
    {
        /// <summary>
        /// Gets the radius.
        /// </summary>
        /// <value>The radius.</value>
        public float Radius { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaTarget"/> class.
        /// </summary>
        /// <param name="point">The center point coordinates.</param>
        /// <param name="radius">The radius value.</param>
        /// <exception cref="ArgumentException">radius - Must be greater than or equal to 0.</exception>
        public AreaTarget(Vector3 point, float radius) : base(point)
        {
            if (radius < 0)
                throw new ArgumentException(nameof(radius), "Must be greater than or equal to 0.");

            Radius = radius;
        }
    }
}
