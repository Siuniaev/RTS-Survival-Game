using Assets.Scripts.InputHandling;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Moves its object to the point under the cursor.
    /// </summary>
    internal class FollowCursor : MonoBehaviour
    {
        private IInputProvider inputProvider;
        private Camera cameraMain;

        /// <summary>
        /// Setups this <see cref="FollowCursor"/> instance.
        /// </summary>
        /// <param name="inputProvider">The input provider.</param>
        /// <param name="cameraMain">The camera main.</param>
        /// <exception cref="ArgumentNullException">
        /// inputProvider
        /// or
        /// cameraMain
        /// </exception>
        public void Setup(IInputProvider inputProvider, Camera cameraMain)
        {
            this.inputProvider = inputProvider ?? throw new ArgumentNullException(nameof(inputProvider));
            this.cameraMain = cameraMain ?? throw new ArgumentNullException(nameof(cameraMain));
        }

        /// <summary>
        /// Is called after all Update functions have been called.
        /// </summary>
        private void LateUpdate()
        {
            if (inputProvider != null)
                Follow();
        }

        /// <summary>
        /// Follows this instance to the point under the cursor.
        /// </summary>
        private void Follow()
        {
            var input = inputProvider.CursorPosition;
            var screenPos = new Vector3(input.x, input.y, Camera.main.transform.position.y);
            var worldPos = cameraMain.ScreenToWorldPoint(screenPos);
            worldPos.y = transform.position.y;
            transform.position = worldPos;
        }
    }
}
