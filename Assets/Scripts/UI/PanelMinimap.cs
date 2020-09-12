using Assets.Scripts.Cameras;
using Assets.Scripts.DI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Panel that shows mini-map.
    /// </summary>
    /// <seealso cref="UIPanel" />
    /// <seealso cref="IInitiableOnInjecting" />
    [RequireComponent(typeof(RawImage))]
    internal class PanelMinimap : UIPanel, IInitiableOnInjecting
    {
        [Injection] private ICameraMinimap CameraMinimap { get; set; }

        private RawImage mapImage;

        /// <summary>
        /// Gets a value indicating whether panel is always showed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if always showed; otherwise, <c>false</c>.
        /// </value>
        public override bool AlwaysShowed => true;

        /// <summary>
        /// Is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            mapImage = GetComponent<RawImage>();

            if (mapImage.texture == null)
                Debug.LogWarning($"There is no {nameof(mapImage.texture)} in ({nameof(RawImage)}){mapImage}");
        }

        /// <summary>
        /// Initializes this instance immediately after completion of all dependency injection.
        /// </summary>
        public void OnInjected()
        {
            CameraMinimap.SetTargetTexture((RenderTexture)mapImage.texture);
        }
    }
}
