using Assets.Scripts.Extensions;
using Assets.Scripts.Skills;
using System;

namespace Assets.Scripts.Units.UnitTargetHandleStrategies
{
    /// <summary>
    /// The way of handling a target for the <see cref="Unit" /> - cast the spell on the target.
    /// </summary>
    /// <seealso cref="UnitTargetHandleStrategy" />
    internal class TargetHandleStrategyUseSkill : UnitTargetHandleStrategy
    {
        private readonly ISkill skill;
        private readonly ISkilled skilled;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetHandleStrategyUseSkill"/> class.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="skill">The skill.</param>
        /// <exception cref="ArgumentNullException">skill</exception>
        /// <exception cref="ArgumentException">unit</exception>
        public TargetHandleStrategyUseSkill(Unit unit, ISkill skill) : base(unit)
        {
            this.skill = skill ?? throw new ArgumentNullException(nameof(skill));
            skilled = unit as ISkilled ?? throw new ArgumentException(nameof(unit), $"{nameof(unit)} must be {nameof(ISkilled)}.");
        }

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
            var distanceMin = skill.DistanceUsing;

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
            return skill.IsTargetCorrect(target, skilled) && skilled.SkillCanBeUsed(skill);
        }

        /// <summary>
        /// Uses the target in a specific way - Cast the spell on the target.
        /// </summary>
        /// <param name="target">The target.</param>
        protected override void SpecificHandle(ITarget target)
        {
            skilled.UseSkillOnTarget(skill, target);
            unit.ResetTarget();
        }
    }
}
