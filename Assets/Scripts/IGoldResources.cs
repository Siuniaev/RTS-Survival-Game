using System;

namespace Assets.Scripts
{
    /// <summary>
    /// Has the gold resources.
    /// </summary>
    internal interface IGoldResources
    {
        /// <summary>
        /// Occurs when current gold has changed.
        /// </summary>
        event Action<ChangedValue<long>> OnGoldChange;

        /// <summary>
        /// Gets the current gold value.
        /// </summary>
        /// <value>The gold value.</value>
        long Gold { get; }

        /// <summary>
        /// Adds the gold to this instance.
        /// </summary>
        /// <param name="gold">The gold value.</param>
        void AddGold(long gold);

        /// <summary>
        /// Tries the spend gold.
        /// </summary>
        /// <param name="gold">The gold value.</param>
        /// <returns>
        ///   <c>true</c> if the gold was spent successfully; otherwise, <c>false</c>.
        /// </returns>
        bool TrySpendGold(long gold);
    }
}
