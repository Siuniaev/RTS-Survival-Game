using Assets.Scripts.DI;
using UnityEngine;

namespace Assets.Scripts.Cameras
{
    /// <summary>
    /// Camera movement strategy that follows the player, keeping him in the center of the view.
    /// </summary>
    /// <seealso cref="ICameraMovementStrategy" />
    internal class CameraMovementFollowCharacter : ICameraMovementStrategy
    {
        [Injection] private ICameraMain CameraMain { get; set; }
        [Injection] private IPlayerResources PlayerResources { get; set; }

        /// <summary>
        /// Enables this camera movement strategy instance.
        /// </summary>
        public void Enable() { }

        /// <summary>
        /// Disables this camera movement strategy instance.
        /// </summary>
        public void Disable() { }

        /// <summary>
        /// Moves the specified camera transform with given speed.
        /// </summary>
        /// <param name="transform">The camera transform.</param>
        /// <param name="speed">The speed.</param>
        public void Move(Transform transform, float speed)
        {
            var heroPos = PlayerResources.SourceHero.Position;
            var targetPos = new Vector3(heroPos.x, CameraMain.Camera.transform.position.y, heroPos.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
        }
    }
}
