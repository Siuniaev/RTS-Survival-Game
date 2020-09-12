namespace Assets.Scripts
{
    /// <summary>
    /// Can attack the specified target.
    /// </summary>
    /// <seealso cref="ITeamMember" />
    internal interface IAttacker : ITeamMember
    {
        /// <summary>
        /// Gets the damage value.
        /// </summary>
        /// <value>The damage value.</value>
        float DamageValue { get; }

        /// <summary>
        /// Attacks the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        void Attack(ITargetAttackable target);
    }
}
