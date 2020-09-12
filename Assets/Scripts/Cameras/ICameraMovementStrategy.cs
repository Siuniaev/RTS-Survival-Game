using UnityEngine;

namespace Assets.Scripts.Cameras
{
    /// <summary>
    /// Camera movement strategy that moves the specified camera.
    /// </summary>
    internal interface ICameraMovementStrategy
    {
        /// <summary>
        /// Enables this camera movement strategy instance.
        /// </summary>
        void Enable();

        /// <summary>
        /// Disables this camera movement strategy instance.
        /// </summary>
        void Disable();

        /// <summary>
        /// Moves the specified camera transform with given speed.
        /// </summary>
        /// <param name="transform">The camera transform.</param>
        /// <param name="speed">The speed.</param>
        void Move(Transform transform, float speed);
    }
}
