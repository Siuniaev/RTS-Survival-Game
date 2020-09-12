using Assets.Scripts.Units;
using System;

namespace Assets.Scripts
{
    /// <summary>
    /// Has the player resources.
    /// </summary>
    /// <seealso cref="IGoldResources" />
    /// <seealso cref="IHeroResources" />
    internal interface IPlayerResources : IGoldResources, IHeroResources
    {
        /// <summary>
        /// Occurs when the new hero setted as a source of hero resources.
        /// </summary>
        event Action<Hero> OnNewSourceHero;

        /// <summary>
        /// Gets the current source hero.
        /// </summary>
        /// <value>The hero.</value>
        Hero SourceHero { get; }

        /// <summary>
        /// Sets the hero as a source of hero resources.
        /// </summary>
        /// <param name="hero">The hero.</param>
        void SetSourceHero(Hero hero);
    }
}
