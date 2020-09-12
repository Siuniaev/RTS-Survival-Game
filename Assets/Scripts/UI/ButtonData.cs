using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// The button data used to configure <see cref="ButtonControl" /> classes.
    /// </summary>
    [Serializable]
    internal class ButtonData
    {
        private readonly UnityAction<int> onClickAction;

        public readonly int IndexButton;
        public readonly string Text;
        public readonly string PopupDescription;
        public readonly bool IsActive;
        public readonly bool IsFlashing;
        public readonly KeyCode? HotKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonData"/> class.
        /// </summary>
        /// <param name="onClickAction">The on button click callback.</param>
        /// <param name="indexButton">The button index.</param>
        /// <param name="text">The button text.</param>
        /// <param name="popupDescription">The popup description.</param>
        /// <param name="isActive">if set to <c>true</c> button is interactable.</param>
        /// <param name="isFlashing">if set to <c>true</c> button is flashing.</param>
        /// <param name="hotKey">The button hotkey.</param>
        public ButtonData(UnityAction<int> onClickAction, int indexButton, string text, string popupDescription,
            bool isActive, bool isFlashing = false, KeyCode? hotKey = null)
        {
            this.onClickAction = onClickAction;
            IndexButton = indexButton;
            Text = text;
            PopupDescription = popupDescription;
            IsActive = isActive;
            IsFlashing = isFlashing;
            HotKey = hotKey;
        }

        /// <summary>
        /// Invokes button click callback.
        /// </summary>
        public void Action() => onClickAction(IndexButton);
    }
}
