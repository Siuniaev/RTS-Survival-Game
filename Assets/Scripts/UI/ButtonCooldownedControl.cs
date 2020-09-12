using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// The button with displayed counter.
    /// </summary>
    /// <seealso cref="ButtonControl" />
    [RequireComponent(typeof(Button), typeof(Image), typeof(CanvasRenderer))]
    internal class ButtonCooldownedControl : ButtonControl
    {
        [SerializeField] private Text textCooldown;
        private ICooldowned lastCooldowned;

        /// <summary>
        /// Is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if (textCooldown == null)
            {
                var texts = GetComponentsInChildren<Text>();
                textCooldown = texts.FirstOrDefault(text => text.name == "TextCooldown") ?? gameObject.AddComponent<Text>();
            }

            ClearCooldownText();
        }

        /// <summary>
        /// Setups the button with the specified button data.
        /// </summary>
        /// <param name="data">The button data.</param>
        public override void Setup(ButtonData data)
        {
            base.Setup(data);

            if (lastCooldowned != null)
            {
                lastCooldowned.Timer.OnTick -= OnCooldownTickHandler;
                lastCooldowned.Timer.OnFinish -= OnFinishHandler;
                lastCooldowned = null;
            }

            if (data is ButtonDataCooldowned cooldownedData)
            {
                lastCooldowned = cooldownedData.Cooldowned;
                lastCooldowned.Timer.OnTick += OnCooldownTickHandler;
                lastCooldowned.Timer.OnFinish += OnFinishHandler;

                if (lastCooldowned.Timer.IsTicking)
                    ActivateButton(false);
            }

            ClearCooldownText();
        }

        /// <summary>
        /// Called when the cooldown has ticked.
        /// </summary>
        /// <param name="cooldown">The cooldown value.</param>
        private void OnCooldownTickHandler(float cooldown)
        {
            ActivateButton(false);
            textCooldown.text = Mathf.Ceil(cooldown).ToString();
        }

        /// <summary>
        /// Called when the cooldown has finished.
        /// </summary>
        /// <param name="obj">The cooldowned object.</param>
        private void OnFinishHandler(object obj) => ClearCooldownText();

        /// <summary>
        /// Clears the cooldown text.
        /// </summary>
        private void ClearCooldownText() => textCooldown.text = "";
    }
}





