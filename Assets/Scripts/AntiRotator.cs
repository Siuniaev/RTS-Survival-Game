using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Anchors the initial rotation angle of the game object and does not allow it to rotate.
    /// </summary>
    public class AntiRotator : MonoBehaviour
    {
        private Quaternion fixedRotation;

        /// <summary>
        /// Is called when the script instance is being loaded.
        /// </summary>
        private void Awake() => fixedRotation = transform.rotation;

        /// Is called after all Update functions have been called.
        private void LateUpdate() => transform.rotation = fixedRotation;
    }
}
