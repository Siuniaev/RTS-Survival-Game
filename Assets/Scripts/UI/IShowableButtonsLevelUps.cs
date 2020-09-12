using System;
using System.Collections.Generic;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Has buttons for selecting improvements that can be shown in the UI.
    /// </summary>
    /// <seealso cref="IShowableButtons" />
    internal interface IShowableButtonsLevelUps : IShowableButtons
    {
        /// <summary>
        /// Occurs when the improvement buttons has changed.
        /// </summary>
        event Action<IEnumerable<ButtonData>> OnButtonsLevelUpsChanges;

        /// <summary>
        /// Gets the improvement buttons.
        /// </summary>
        /// <returns>The buttons.</returns>
        IEnumerable<ButtonData> GetButtonsLevelUps();
    }
}
