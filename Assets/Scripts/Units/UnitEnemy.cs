using Assets.Scripts.Buildings;
using Assets.Scripts.Units.UnitDatas;

namespace Assets.Scripts.Units
{
    /// <summary>
    /// The enemy unit not controlled by the player. Attacks the player’s minions, attacks the throne when there are no
    /// live targets on the map.
    /// UnitEneny instances are created in <see cref="EnemyFactory"/> objects.
    /// </summary>
    /// <seealso cref="Unit{UnitDataEnemy}" />
    internal class UnitEnemy : Unit<UnitDataEnemy>
    {
        /// <summary>
        /// Gets this unit team.
        /// </summary>
        /// <value>The team.</value>
        public override Team Team => Team.Enemies;

        /// <summary>
        /// Determines whether this instance can be attacked by the specified attacker.
        /// </summary>
        /// <param name="attacker">The attacker.</param>
        /// <returns>
        ///   <c>true</c> if this instance can be attacked by the specified attacker; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanBeAttackedBy(ITeamMember attacker) => attacker.Team == Team.Friends;

        /// <summary>
        /// Applies the damage to this unit.
        /// </summary>
        /// <param name="attacker">The attacker.</param>
        /// <param name="damage">The damage value.</param>
        public override void ApplyDamage(ITeamMember attacker, float damage)
        {
            base.ApplyDamage(attacker, damage);

            if (!IsAlive && attacker is IExpCollector collector)
                collector.AddExp(unitData.ExpForDie);
        }

        /// <summary>
        /// Finds the next current target.
        /// </summary>
        /// <returns>The finded target.</returns>
        protected override ITarget FindNextTarget()
        {
            return targets.GetUnitTargetFor(this) ?? targets.GetBuilding<Throne>();
        }
    }
}
