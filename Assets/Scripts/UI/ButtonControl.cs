using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Base class for all buttons in the game.
    /// </summary>
    [RequireComponent(typeof(Button), typeof(Image), typeof(CanvasRenderer))]
    internal class ButtonControl : MonoBehaviour
    {
        public const float DEFAULT_FLASHING_DELAY = 0.3f;
        public static readonly Color DEFAULT_FLASHING_COLOR = new Color(0.8f, 1f, 0.7f);

        [SerializeField] protected Button button;
        [SerializeField] protected Text text;
        [SerializeField] protected Text textHotKey;
        protected Color earlyColor;
        protected bool isFlashing;

        /// <summary>
        /// Gets or sets the hotkey of this button.
        /// </summary>
        /// <value>The hotkey.</value>
        public KeyCode? HotKey { get; protected set; }

        /// <summary>
        /// Is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();

            if (text == null)
            {
                var texts = GetComponentsInChildren<Text>();
                text = texts.FirstOrDefault(text => text.name == "Text") ?? gameObject.AddComponent<Text>();
            }

            if (textHotKey == null)
            {
                var texts = GetComponentsInChildren<Text>();
                textHotKey = texts.FirstOrDefault(text => text.name == "TextHotKey") ?? gameObject.AddComponent<Text>();
            }

            earlyColor = button.colors.normalColor;
        }

        /// <summary>
        /// Setups the button with the specified button data.
        /// </summary>
        /// <param name="data">The button data.</param>
        public virtual void Setup(ButtonData data)
        {
            text.text = data?.Text;
            HotKey = data?.HotKey;
            textHotKey.text = HotKey?.ToString();

            button.onClick.RemoveAllListeners();

            if (data != null)
            {
                button.onClick.AddListener(data.Action);
                ActivateButton(data.IsActive);
            }
        }

        /// <summary>
        /// Invokes the button click.
        /// </summary>
        public virtual void InvokeButtonClick() => button.onClick?.Invoke();

        /// <summary>
        /// Flashings the button color.
        /// </summary>
        /// <param name="neededFlashing">if set to <c>true</c> the button should flashing.</param>
        public virtual void FlashingColor(bool neededFlashing)
        {
            if (isFlashing)
            {
                if (!neededFlashing) // Is flashing now, but have to turn off.
                {
                    StopAllCoroutines();
                    ResetColor();
                    isFlashing = false;
                }
            }
            else
            {
                if (neededFlashing) // Is not flashing now, but have to turn on.
                {
                    StartCoroutine(Flashing());
                    isFlashing = true;
                }
            }
        }

        /// <summary>
        /// Flashings this button.
        /// </summary>
        /// <returns>The yield instruction.</returns>
        protected IEnumerator Flashing()
        {
            while (true)
            {
                button.image.color = button.image.color != DEFAULT_FLASHING_COLOR ? DEFAULT_FLASHING_COLOR : earlyColor;
                yield return new WaitForSeconds(DEFAULT_FLASHING_DELAY);
            }
        }

        /// <summary>
        /// Activates the button.
        /// </summary>
        /// <param name="active">if set to <c>true</c> the button is interactable.</param>
        protected void ActivateButton(bool active) => button.interactable = active;

        /// <summary>
        /// Resets the button color to the originally set.
        /// </summary>
        protected virtual void ResetColor()
        {
            var colors = button.colors;
            colors.normalColor = earlyColor;
            button.colors = colors;
            button.image.color = earlyColor;
        }
    }
}
