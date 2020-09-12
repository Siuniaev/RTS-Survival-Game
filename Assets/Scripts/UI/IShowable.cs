using System;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Can be shown in the UI.
    /// </summary>
    internal interface IShowable
    {
        /// <summary>
        /// Occurs when showable data changes.
        /// </summary>
        event Action<ShowableData> OnShowableDataChanges;

        /// <summary>
        /// Occurs when this instance does not need to be shown.
        /// </summary>
        event Action<IShowable> OnStopShowing;

        /// <summary>
        /// Gets the showable data.
        /// </summary>
        /// <returns>The showable data.</returns>
        ShowableData GetShowableData();
    }
}
