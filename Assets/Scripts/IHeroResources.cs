using System;

namespace Assets.Scripts
{
    /// <summary>
    /// Has the hero resources.
    /// </summary>
    internal interface IHeroResources
    {
        /// <summary>
        /// Occurs when hero resources data has changed.
        /// </summary>
        event Action<HeroResourceData> OnHeroResourcesDataChange;

        /// <summary>
        /// Gets the hero resource data.
        /// </summary>
        /// <returns>The hero resource data.</returns>
        HeroResourceData GetHeroResourceData();
    }
}
