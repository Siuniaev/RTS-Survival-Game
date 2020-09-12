using System;

namespace Assets.Scripts.InputHandling
{
    /// <summary>
    /// Can prohibit and allow the selecting for <see cref="ISelector" />.
    /// </summary>
    internal interface ISelectionBlocker
    {
        /// <summary>
        /// Occurs when the selection with <see cref="ISelector" /> needs to be blocked.
        /// </summary>
        event Action<bool> OnSelectionBlock;
    }
}
