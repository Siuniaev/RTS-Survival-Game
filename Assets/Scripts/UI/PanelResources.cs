using Assets.Scripts.DI;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Panel that shows current player resources.
    /// </summary>
    /// <seealso cref="UIPanel" />
    /// <seealso cref="IInitiableOnInjecting" />
    internal class PanelResources : UIPanel, IInitiableOnInjecting
    {
        [Injection] private IPlayerResources PlayerResources { get; set; }

        public const string DEFAULT_TEXT_LEVEL = "1";
        public const string DEFAULT_TEXT_EXP = "0";
        public const string DEFAULT_TEXT_GOLD = "0";

        private readonly StringBuilder jumpingTextFrameResult = new StringBuilder();
        [SerializeField] private Text textLevel;
        [SerializeField] private Text textExp;
        [SerializeField] private Text textGold;
        [SerializeField] private TextJumper textGoldJumping;

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
            if (textLevel == null)
                throw new NullReferenceException(nameof(textLevel));

            if (textExp == null)
                throw new NullReferenceException(nameof(textExp));

            if (textGold == null)
                throw new NullReferenceException(nameof(textGold));

            textLevel.text = DEFAULT_TEXT_LEVEL;
            textExp.text = DEFAULT_TEXT_EXP;
            textGold.text = DEFAULT_TEXT_GOLD;
        }

        /// <summary>
        /// Initializes this instance immediately after completion of all dependency injection.
        /// </summary>
        public void OnInjected()
        {
            OnGoldChangeHandler(new ChangedValue<long>(PlayerResources.Gold));
            PlayerResources.OnGoldChange += OnGoldChangeHandler;
            PlayerResources.OnHeroResourcesDataChange += OnHeroResourcesDataChangeHandler;
        }

        /// <summary>
        /// Is called after all Update functions have been called.
        /// </summary>
        private void LateUpdate()
        {
            if (textGoldJumping == false || jumpingTextFrameResult.Length == 0)
                return;

            textGoldJumping.UpdateText(jumpingTextFrameResult.ToString());
            jumpingTextFrameResult.Clear();
        }

        /// <summary>
        /// Gets the jumping text from the given changed value.
        /// </summary>
        /// <param name="value">The changed value.</param>
        /// <returns>The jumping text.</returns>
        private static string GetJumpingText(ChangedValue<long> value)
        {
            var difference = value.NewValue - value.OldValue;
            return difference.ToString("+#;-#;0"); // "+#;-#;0" - Its showing the sign next to the value.
        }

        /// <summary>
        /// Called when [on gold change].
        /// </summary>
        /// <param name="gold">The changed gold.</param>
        private void OnGoldChangeHandler(ChangedValue<long> gold)
        {
            JumpTextGold(gold);
            UpdateTextGold(gold.NewValue.ToString());
        }

        /// <summary>
        /// Called when [on hero resources data change].
        /// </summary>
        /// <param name="data">The hero resource data.</param>
        private void OnHeroResourcesDataChangeHandler(HeroResourceData data)
        {
            textLevel.text = data.Level.ToString();
            textExp.text = data.ExpMax != long.MaxValue ? $"{ data.Exp.ToString() } / {data.ExpMax.ToString()}" : "max";
        }

        /// <summary>
        /// Updates the text of gold.
        /// </summary>
        /// <param name="newText">The new text.</param>
        private void UpdateTextGold(string newText) => textGold.text = newText;

        /// <summary>
        /// Jumps the text of gold.
        /// </summary>
        /// <param name="gold">The changed gold.</param>
        private void JumpTextGold(ChangedValue<long> gold)
        {
            var jumpingText = GetJumpingText(gold);
            jumpingTextFrameResult.Append(jumpingText + "\n");
        }
    }
}
