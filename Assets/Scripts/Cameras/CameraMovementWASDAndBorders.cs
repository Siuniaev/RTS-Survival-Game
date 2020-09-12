using Assets.Scripts.DI;
using Assets.Scripts.InputHandling;
using Assets.Scripts.InputHandling.Handlers;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Cameras
{
    /// <summary>
    /// Camera movement strategy that is controlled by the player using the WASD buttons and holding the cursor
    /// at the edges of the screen.
    /// </summary>
    /// <seealso cref="ICameraMovementStrategy" />
    internal class CameraMovementWASDAndBorders : ICameraMovementStrategy
    {
        public const float DEFAULT_SCREEN_EDGE_THICKNESS = 10.0f;

        [Injection] private IInputProvider InputProvider { get; set; }

        // Distance from the screen edge where the cursor makes the camera move.
        private float screenEdgeThickness = DEFAULT_SCREEN_EDGE_THICKNESS;
        private Vector3 offset;

        /// <summary>
        /// Enables this camera movement strategy instance: subscribes to player input.
        /// </summary>
        public void Enable()
        {
            Task.Run(SubscribeForInput);
        }

        /// <summary>
        /// Disables this camera movement strategy instance: unsubscribes from player input.
        /// </summary>
        public void Disable()
        {
            InputProvider.Unsubscribe(this);
        }

        /// <summary>
        /// Moves the specified camera transform with given speed towards the accumulated motion vector.
        /// </summary>
        /// <param name="transform">The camera transform.</param>
        /// <param name="speed">The speed.</param>
        public void Move(Transform transform, float speed)
        {
            transform.Translate(offset * Time.deltaTime * speed, Space.World);
            offset = Vector3.zero;
        }

        /// <summary>
        /// Adds upward direction to the accumulated motion vector.
        /// </summary>
        private void MoveUp() => offset += Vector3.forward;

        /// <summary>
        /// Adds a downward direction to the accumulated motion vector.
        /// </summary>
        private void MoveDown() => offset += Vector3.back;

        /// <summary>
        /// Adds a left direction to the accumulated motion vector.
        /// </summary>
        private void MoveLeft() => offset += Vector3.left;

        /// <summary>
        /// Adds the right direction to the accumulated motion vector.
        /// </summary>
        private void MoveRight() => offset += Vector3.right;

        /// <summary>
        /// Subscribes for user input.
        /// </summary>
        private async void SubscribeForInput()
        {
            var W = await InputProvider.GetHandlerAsync<InputKeyHandler, KeyCode>(KeyCode.W);
            var S = await InputProvider.GetHandlerAsync<InputKeyHandler, KeyCode>(KeyCode.S);
            var A = await InputProvider.GetHandlerAsync<InputKeyHandler, KeyCode>(KeyCode.A);
            var D = await InputProvider.GetHandlerAsync<InputKeyHandler, KeyCode>(KeyCode.D);
            var cursorUP = await InputProvider.GetHandlerAsync<InputMousePositionHandler, Predicate<Vector3>>(
                (pos) => pos.y >= Screen.height - screenEdgeThickness);
            var cursorDOWN = await InputProvider.GetHandlerAsync<InputMousePositionHandler, Predicate<Vector3>>(
                (pos) => pos.y <= screenEdgeThickness);
            var cursorLEFT = await InputProvider.GetHandlerAsync<InputMousePositionHandler, Predicate<Vector3>>(
                (pos) => pos.x <= screenEdgeThickness);
            var cursorRIGHT = await InputProvider.GetHandlerAsync<InputMousePositionHandler, Predicate<Vector3>>(
                (pos) => pos.x >= Screen.width - screenEdgeThickness);

            W.OnKeyPressed += MoveUp;
            S.OnKeyPressed += MoveDown;
            A.OnKeyPressed += MoveLeft;
            D.OnKeyPressed += MoveRight;
            cursorUP.OnMousePositionSatisfy += MoveUp;
            cursorDOWN.OnMousePositionSatisfy += MoveDown;
            cursorLEFT.OnMousePositionSatisfy += MoveLeft;
            cursorRIGHT.OnMousePositionSatisfy += MoveRight;
        }
    }
}
