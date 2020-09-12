using System;
using UnityEngine;

namespace Assets.Scripts.HelperObjects
{
    /// <summary>
    /// Flat translucent helper-object showing a circular zone on the ground.
    /// </summary>
    /// <seealso cref="IPoolableObject" />
    [RequireComponent(typeof(Rotator))]
    [Serializable]
    internal class HelperCircleArea : MonoBehaviour, IPoolableObject
    {
        public const float DEFAULT_ABOVE_THE_GROUND_Y_POSITION = 0.1f;
        public const float DEFAULT_RADIUS_TO_SCALE_MULTIPLIER = 2f;
        public readonly static Vector3 DEFAULT_ROTATION = new Vector3(90f, 0f, 0f);

        /// <summary>
        /// Occurs when the attached Component is destroying as <see cref="IPoolableObject" />.
        /// </summary>
        public event Action<Component> OnDestroyAsPoolableObject;

        /// <summary>
        /// Gets or sets the prefab instance identifier.
        /// </summary>
        /// <value>The prefab instance identifier.</value>
        public int PrefabInstanceID { get; set; }

        /// <summary>
        /// Configures this instance.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="parent"></param>
        /// <param name="rotation"></param>
        /// <exception cref="ArgumentOutOfRangeException">radius - Must be greater than or equal to 0.</exception>
        public void Setup(float radius, Transform parent = null, Vector3? rotation = null)
        {
            if (radius < 0)
                throw new ArgumentOutOfRangeException(nameof(radius), "Must be greater than or equal to 0.");

            var pickedParent = parent ?? transform.parent;
            transform.SetParent(null);
            var scale = radius * DEFAULT_RADIUS_TO_SCALE_MULTIPLIER;
            transform.localScale = new Vector3(scale, scale, scale);
            transform.SetParent(pickedParent);

            var pos = pickedParent != null ? pickedParent.position : Vector3.zero;
            pos.y = DEFAULT_ABOVE_THE_GROUND_Y_POSITION;
            transform.position = pos;

            transform.eulerAngles = rotation ?? DEFAULT_ROTATION;
        }

        /// <summary>
        /// Returns this object to the object pool.
        /// </summary>
        public void DestroyAsPoolableObject() => OnDestroyAsPoolableObject?.Invoke(this);
    }
}
