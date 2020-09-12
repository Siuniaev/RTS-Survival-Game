using System;
using System.Collections.Generic;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Has buttons that can be shown in the UI.
    /// </summary>
    internal interface IShowableButtons
    {
        /// <summary>
        /// Occurs when one button nas changed.
        /// </summary>
        event Action<ButtonData> OnButtonChange;

        /// <summary>
        /// Occurs when buttons has changed.
        /// </summary>
        event Action<IEnumerable<ButtonData>> OnButtonsChanges;

        /// <summary>
        /// Gets the buttons.
        /// </summary>
        /// <returns>The buttons.</returns>
        IEnumerable<ButtonData> GetButtons();
    }
}
