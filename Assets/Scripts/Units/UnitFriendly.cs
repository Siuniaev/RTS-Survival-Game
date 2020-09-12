using Assets.Scripts.Buildings;
using Assets.Scripts.Extensions;
using Assets.Scripts.InputHandling;
using Assets.Scripts.Units.UnitDatas;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Units
{
    /// <summary>
    /// The base class for friendly units (minions) controlled by the player. Attacks enemies, defends the throne,
    /// heals at the fountain when there are no living enemies on the map.
    /// </summary>
    /// <typeparam name="T">The unit data type.</typeparam>
    /// <seealso cref="Unit{T}" />
    /// <seealso cref="IHealed" />
    /// <seealso cref="IPlayerControllableUnits" />
    abstract class UnitFriendly<T> : Unit<T>, IHealed, IPlayerControllableUnits
        where T : UnitData
    {
        protected bool targetIsAssigned;

        /// <summary>
        /// Gets this unit team.
        /// </summary>
        /// <value>The team.</value>
        public override Team Team => Team.Friends;

        /// <summary>
        /// Gets the controllable units from this unit.
        /// </summary>
        /// <value>The controllable units.</value>
        public IEnumerable<Unit> ControllableUnits => Enumerable.Repeat(this, 1); // Make enumerable from this one unit.

        /// <summary>
        /// Determines whether this instance can be attacked by the specified attacker.
        /// </summary>
        /// <param name="attacker">The attacker.</param>
        /// <returns>
        ///   <c>true</c> if this instance can be attacked by the specified attacker; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanBeAttackedBy(ITeamMember attacker) => attacker.Team == Team.Enemies;

        /// <summary>
        /// Sets the target manually by decree of the player controlling the unit.
        /// </summary>
        /// <param name="target">The target.</param>
        public override void SetTargetManually(ITarget target)
        {
            base.SetTargetManually(target);
            targetIsAssigned = true;
        }

        /// <summary>
        /// Resets the current target.
        /// </summary>
        public override void ResetTarget()
        {
            base.ResetTarget();
            targetIsAssigned = false;
        }

        /// <summary>
        /// Accepts the healer.
        /// </summary>
        /// <param name="healer">The healer.</param>
        public abstract void AcceptHealer(IHealer healer);

        /// <summary>
        /// Finds the next target.
        /// </summary>
        /// <returns>The finded target.</returns>
        protected override ITarget FindNextTarget()
        {
            if (IsAssignedTargetExists())
                return Target;

            return targets.GetUnitTargetFor(this) ?? targets.GetBuilding<Fountain>();
        }

        /// <summary>
        /// Determines whether is the assigned target exists.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the is assigned target exists; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsAssignedTargetExists()
        {
            return targetIsAssigned && !Target.IsNullOrMissing();
        }
    }
}
