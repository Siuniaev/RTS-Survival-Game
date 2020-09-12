using Assets.Scripts.Units;

namespace Assets.Scripts
{
    /// <summary>
    /// Can heals <see cref="IHealed" /> objects.
    /// </summary>
    internal interface IHealer
    {
        /// <summary>
        /// Heals the specified minion.
        /// </summary>
        /// <param name="minion">The minion.</param>
        void Heal(UnitFriendlyMinion minion);

        /// <summary>
        /// Heals the specified hero.
        /// </summary>
        /// <param name="hero">The hero.</param>
        void Heal(Hero hero);
    }
}
