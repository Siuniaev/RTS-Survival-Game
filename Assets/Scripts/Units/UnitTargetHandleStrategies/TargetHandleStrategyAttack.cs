using Assets.Scripts.Extensions;

namespace Assets.Scripts.Units.UnitTargetHandleStrategies
{
    /// <summary>
    /// The attacking way of handling a target for the <see cref="Unit" />.
    /// </summary>
    /// <seealso cref="UnitTargetHandleStrategy" />
    internal class TargetHandleStrategyAttack : UnitTargetHandleStrategy
    {
        public const float DEFAULT_MINIMAL_DISTANCE_TO_POINT = 0.1f;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetHandleStrategyAttack"/> class.
        /// </summary>
        /// <param name="unit">The unit.</param>
        public TargetHandleStrategyAttack(Unit unit) : base(unit) { }

        /// <summary>
        /// Determines whether the unit is close enough for the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>
        ///   <c>true</c> if the unit is close enough for the target; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsCloseForTarget(ITarget target)
        {
            var distance = unit.Position.FlatDistanceTo(target.Position);
            var distanceMin = target is ITargetAttackable ? unit.Parameters.AttackRange : DEFAULT_MINIMAL_DISTANCE_TO_POINT;

            return distance <= distanceMin;
        }

        /// <summary>
        /// Determines if the target is checked and the unit can process it.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>
        ///   <c>true</c> if the target is checked; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsTargetChecked(ITarget target)
        {
            return target is ITargetAttackable attackable && attackable.CanBeAttackedBy(unit) && attackable.IsAlive;
        }

        /// <summary>
        /// Uses the target in a specific way - Attack the target.
        /// </summary>
        /// <param name="target">The target.</param>
        protected override void SpecificHandle(ITarget target)
        {
            unit.Attack((ITargetAttackable)target);
        }
    }
}
