using System;
using UnityEngine;

namespace Assets.Scripts.Cameras
{
    /// <summary>
    /// The additional camera showing the entire playing field as a mini-map.
    /// </summary>
    /// <seealso cref="ICameraMinimap" />
    [RequireComponent(typeof(Camera))]
    internal sealed class CameraMinimap : MonoBehaviour, ICameraMinimap
    {
        public const float DEFAULT_ORTHOGRAPHIC_SIZE = 50.4f;
        public const float DEFAULT_CLIPPLANE_NEAR = 0.3f;
        public const float DEFAULT_CLIPPLANE_FAR = 30f;
        public const int DEFAULT_CULLING_MASK = 1; // 1 = Default layer.
        public static readonly Vector3 DEFAULT_POSITION = new Vector3(0f, 28.83f, 0f);
        public static readonly Vector3 DEFAULT_ROTATION = new Vector3(90f, 0f, 0f);

        private new Camera camera;

        /// <summary>
        /// Is called when the script instance is being loaded.
        /// </summary>
        private void Awake() => SetupCameraMinimap();

        /// <summary>
        /// Setups the camera minimap.
        /// </summary>
        private void SetupCameraMinimap()
        {
            camera = GetComponent<Camera>();
            transform.position = DEFAULT_POSITION;
            transform.rotation = Quaternion.Euler(DEFAULT_ROTATION);
            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = Color.white;
            camera.orthographic = true;
            camera.orthographicSize = DEFAULT_ORTHOGRAPHIC_SIZE;
            camera.cullingMask = DEFAULT_CULLING_MASK;
            camera.nearClipPlane = DEFAULT_CLIPPLANE_NEAR;
            camera.farClipPlane = DEFAULT_CLIPPLANE_FAR;
        }

        /// <summary>
        /// Sets the target texture where the image will be rendered by this camera.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <exception cref="ArgumentNullException">texture</exception>
        public void SetTargetTexture(RenderTexture texture)
        {
            camera.targetTexture = texture ?? throw new ArgumentNullException(nameof(texture));
        }
    }
}
