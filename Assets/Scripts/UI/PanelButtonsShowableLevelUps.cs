using Assets.Scripts.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Panel with buttons and improvement buttons, that can show <see cref="IShowable" /> objects.
    /// </summary>
    /// <seealso cref="PanelButtonsShowable" />
    internal class PanelButtonsShowableLevelUps : PanelButtonsShowable
    {
        /// <summary>
        /// Called when the attached Behaviour is destroying.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (!lastShowableButtons.IsNullOrMissing() && lastShowableButtons is IShowableButtonsLevelUps buttons)
                buttons.OnButtonsLevelUpsChanges -= OnButtonsLevelUpsChangesHandler;
        }

        /// <summary>
        /// Remove the last shown showable from the UI.
        /// </summary>
        public override void ClearLastShowable()
        {
            if (lastShowableButtons != null && lastShowableButtons is IShowableButtonsLevelUps showable)
            {
                showable.OnButtonsLevelUpsChanges -= OnButtonsLevelUpsChangesHandler;
                lastShowableButtons = null;
            }
        }

        /// <summary>
        /// Keeps the new showable.
        /// </summary>
        /// <param name="showable">The showable.</param>
        protected override void KeepNewShowable(IShowable showable)
        {
            var showableButtons = showable as IShowableButtonsLevelUps;
            var data = showableButtons?.GetButtonsLevelUps();
            var show = data != null && data.Any();

            SetVisible(show);

            if (showableButtons == null)
                return;

            UpdateButtons(data);
            showableButtons.OnButtonsLevelUpsChanges += OnButtonsLevelUpsChangesHandler;
            lastShowableButtons = showableButtons;
        }

        /// <summary>
        /// Called when the improvement buttons has changed.
        /// </summary>
        /// <param name="datas">The button datas.</param>
        private void OnButtonsLevelUpsChangesHandler(IEnumerable<ButtonData> datas)
        {
            SetVisible(datas != null && datas.Any());
            UpdateButtons(datas);
        }
    }
}
