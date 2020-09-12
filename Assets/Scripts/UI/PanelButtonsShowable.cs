using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Panel with buttons, that can show <see cref="IShowable" /> objects.
    /// </summary>
    /// <seealso cref="PanelButtons" />
    /// <seealso cref="IShowablesPresenter" />
    internal class PanelButtonsShowable : PanelButtons, IShowablesPresenter
    {
        public const int NO_BUTTON_FLASHING = -1;

        private int lastFlashingButtonIndex = NO_BUTTON_FLASHING;
        protected IShowableButtons lastShowableButtons;

        /// <summary>
        /// Called when the attached Behaviour is destroying.
        /// </summary>
        protected virtual void OnDestroy() => ClearLastShowable();

        /// <summary>
        /// Sets the showable to show in the UI.
        /// </summary>
        /// <param name="showable">The showable.</param>
        public void SetShowable(IShowable showable)
        {
            if (lastShowableButtons != null && lastShowableButtons == showable)
                return;

            ClearLastShowable();
            KeepNewShowable(showable);
        }

        /// <summary>
        /// Updates the buttons with the given button datas.
        /// </summary>
        /// <param name="datas">The datas.</param>
        public override void UpdateButtons(IEnumerable<ButtonData> datas)
        {
            var flashingButton = datas?.FirstOrDefault(data => data.IsFlashing);
            ActualizeFlashingButton(flashingButton);
            base.UpdateButtons(datas);
        }

        /// <summary>
        /// Updates the button with the given button data.
        /// </summary>
        /// <param name="data">The data.</param>
        public override void UpdateButton(ButtonData data)
        {
            if (lastFlashingButtonIndex == data.IndexButton)
            {
                if (!data.IsFlashing) // Is flashing now, but have to turn off.
                    ResetFlashingButton();
            }
            else
            {
                if (data.IsFlashing) // Is not flashing now, but have to turn on.
                {
                    ResetFlashingButton();
                    SetFlashingForButton(data.IndexButton, true);
                }
            }

            base.UpdateButton(data);
        }

        /// <summary>
        /// Remove the last shown showable from the UI.
        /// </summary>
        public virtual void ClearLastShowable()
        {
            if (lastShowableButtons == null)
                return;

            lastShowableButtons.OnButtonsChanges -= UpdateButtons;
            lastShowableButtons.OnButtonChange -= UpdateButton;
            lastShowableButtons = null;
        }

        /// <summary>
        /// Keeps the new showable.
        /// </summary>
        /// <param name="showable">The showable.</param>
        protected virtual void KeepNewShowable(IShowable showable)
        {
            var showableButtons = showable as IShowableButtons;
            var show = showable != null && showableButtons != null;

            SetVisible(show);

            if (showableButtons == null)
                return;

            var data = showableButtons.GetButtons();
            UpdateButtons(data);
            showableButtons.OnButtonsChanges += UpdateButtons;
            showableButtons.OnButtonChange += UpdateButton;
            lastShowableButtons = showableButtons;
        }

        /// <summary>
        /// Actualizes the current flashing button.
        /// </summary>
        /// <param name="flashingButton">The flashing button data.</param>
        private void ActualizeFlashingButton(ButtonData flashingButton)
        {
            if (flashingButton == null || lastFlashingButtonIndex != flashingButton.IndexButton)
                ResetFlashingButton();

            if (flashingButton != null)
                SetFlashingForButton(flashingButton.IndexButton, true);
        }

        /// <summary>
        /// Disables flashing for the current flashing button.
        /// </summary>
        private void ResetFlashingButton()
        {
            if (lastFlashingButtonIndex != NO_BUTTON_FLASHING)
                SetFlashingForButton(lastFlashingButtonIndex, false);
        }

        /// <summary>
        /// Sets the flashing for button.
        /// </summary>
        /// <param name="indexButton">The index button.</param>
        /// <param name="flashingOn">if set to <c>true</c> button is flashing.</param>
        /// <exception cref="ArgumentOutOfRangeException">indexButton</exception>
        private void SetFlashingForButton(int indexButton, bool flashingOn)
        {
            if (indexButton < 0 || indexButton >= buttons.Length)
                throw new ArgumentOutOfRangeException(nameof(indexButton));

            var button = buttons[indexButton];
            button.FlashingColor(neededFlashing: flashingOn);
            lastFlashingButtonIndex = flashingOn ? indexButton : NO_BUTTON_FLASHING;
        }
    }
}
