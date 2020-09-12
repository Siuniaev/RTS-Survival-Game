using Assets.Scripts.DI;
using Assets.Scripts.InputHandling;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Panel that can draw a rectangular selection area.
    /// </summary>
    /// <seealso cref="UIPanel" />
    /// <seealso cref="ISelectionDrawer" />
    [RequireComponent(typeof(Image), typeof(RectTransform))]
    internal class PanelSelectionRectangle : UIPanel, ISelectionDrawer
    {
        public static readonly Color DEFAULT_IMAGE_COLOR = new Color(1f, 1f, 1f, 0.3f);

        [Injection] private IHUD HUD { get; set; }

        private Image image;
        private RectTransform rectTrans;

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
            image = GetComponent<Image>();
            rectTrans = GetComponent<RectTransform>();

            if (image.sprite == null)
                SetImageDefault();

            OnSelectionEndHandler();
        }

        /// <summary>
        /// Called when the selection needs to be drawn.
        /// Stretches the rectangle sprite to show the specified selection area.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        public void OnSelectionDrawingHandler(Vector3 start, Vector3 end)
        {
            rectTrans.position = Vector3.Min(start, end);
            var sizeX = Mathf.Abs(start.x - end.x);
            var sizeY = Mathf.Abs(start.y - end.y);
            var factor = HUD.GetScaleFactor();
            rectTrans.sizeDelta = new Vector2(sizeX / factor, sizeY / factor);
            image.enabled = true;
        }

        /// <summary>
        /// Called when the selection has ended.
        /// Hides the rectangle sprite.
        /// </summary>
        public void OnSelectionEndHandler() => image.enabled = false;

        /// <summary>
        /// Sets the default image to the drawn sprite.
        /// </summary>
        private void SetImageDefault()
        {
            var whiteTexture = new Texture2D(1, 1);
            whiteTexture.SetPixel(0, 0, Color.white);
            whiteTexture.Apply();
            image.sprite = Sprite.Create(whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
            image.color = DEFAULT_IMAGE_COLOR;
        }
    }
}
