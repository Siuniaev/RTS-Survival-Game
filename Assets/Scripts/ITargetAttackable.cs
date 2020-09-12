namespace Assets.Scripts
{
    /// <summary>
    /// Can be attacked.
    /// </summary>
    /// <seealso cref="ITarget" />
    internal interface ITargetAttackable : ITarget
    {
        /// <summary>
        /// Gets a value indicating whether this instance is alive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is alive; otherwise, <c>false</c>.
        /// </value>
        bool IsAlive { get; }

        /// <summary>
        /// Determines whether this instance can be attacked by the specified attacker.
        /// </summary>
        /// <param name="attacker">The attacker.</param>
        /// <returns>
        ///   <c>true</c> if this instance can be attacked by the specified attacker; otherwise, <c>false</c>.
        /// </returns>
        bool CanBeAttackedBy(ITeamMember attacker);

        /// <summary>
        /// Applies the damage.
        /// </summary>
        /// <param name="attacker">The attacker.</param>
        /// <param name="damage">The damage.</param>
        void ApplyDamage(ITeamMember attacker, float damage);
    }
}
