using UnityEngine;

namespace Assets.Scripts.Cameras
{
    /// <summary>
    /// The additional camera showing the entire playing field as a mini-map.
    /// </summary>
    internal interface ICameraMinimap
    {
        /// <summary>
        /// Sets the target texture where the image will be rendered by this camera.
        /// </summary>
        /// <param name="texture">The texture.</param>
        void SetTargetTexture(RenderTexture texture);
    }
}
