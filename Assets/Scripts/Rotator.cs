using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Rotates its game object endlessly.
    /// </summary>
    public class Rotator : MonoBehaviour
    {
        public const float DEFAULT_SPEED = 20f;
        public static readonly Vector3 DEFAULT_ROTATING_AXIS = new Vector3(0f, 0f, 1f);

        [SerializeField] private float speed = DEFAULT_SPEED;
        [SerializeField] private Vector3 axis = DEFAULT_ROTATING_AXIS;

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            transform.Rotate(axis, speed * Time.deltaTime);
        }
    }
}
