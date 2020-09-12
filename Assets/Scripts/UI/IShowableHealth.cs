using System;
using UnityEngine;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Has health that can be shown in the UI.
    /// </summary>
    internal interface IShowableHealth
    {
        /// <summary>
        /// Occurs when health has changed.
        /// </summary>
        event Action<ChangedHealthArgs> OnHealthChanges;

        /// <summary>
        /// Gets the health bar position.
        /// </summary>
        /// <value>The health bar position.</value>
        Vector3? HealthBarPosition { get; }

        /// <summary>
        /// Gets the health bar height offset.
        /// </summary>
        /// <value>The health bar height offset.</value>
        float HealthBarHeightOffset { get; }

        /// <summary>
        /// Gets the width of the health bar.
        /// </summary>
        /// <value>The width of the health bar.</value>
        float HealthBarWidth { get; }
    }
}
