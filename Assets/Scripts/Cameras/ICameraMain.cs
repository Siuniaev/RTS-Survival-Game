using UnityEngine;

namespace Assets.Scripts.Cameras
{
    /// <summary>
    /// The main camera showing the playing field. Used for ray casting.
    /// </summary>
    internal interface ICameraMain
    {
        /// <summary>
        /// Gets the Camera component.
        /// </summary>
        /// <value>The Camera component.</value>
        Camera Camera { get; }

        /// <summary>
        /// Gets the camera speed.
        /// </summary>
        /// <value>The camera speed.</value>
        float CameraSpeed { get; }

        /// <summary>
        /// Switches the camera movement strategy.
        /// </summary>
        void SwitchMovementStrategy();
    }
}
