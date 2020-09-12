using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// The button data with displayed counter used to configure <see cref="ButtonCooldownedControl" /> classes.
    /// </summary>
    /// <seealso cref="ButtonData" />
    [Serializable]
    internal class ButtonDataCooldowned : ButtonData
    {
        public readonly ICooldowned Cooldowned;

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonDataCooldowned"/> class.
        /// </summary>
        /// <param name="cooldowned">The cooldowned object.</param>
        /// <param name="onClickAction">The on button click callback.</param>
        /// <param name="indexButton">The button index.</param>
        /// <param name="text">The button text.</param>
        /// <param name="popupDescription">The popup description.</param>
        /// <param name="isActive">if set to <c>true</c> button is interactable.</param>
        /// <param name="isFlashing">if set to <c>true</c> button is flashing.</param>
        /// <param name="hotKey">The button hotkey.</param>
        public ButtonDataCooldowned(ICooldowned cooldowned, UnityAction<int> onClickAction, int indexButton, string text,
            string popupDescription, bool isActive, bool isFlashing = false, KeyCode? hotKey = null)
            : base(onClickAction, indexButton, text, popupDescription, isActive, isFlashing, hotKey)
        {
            Cooldowned = cooldowned;
        }
    }
}
