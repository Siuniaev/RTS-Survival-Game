using System;
using UnityEngine;

namespace Assets.Scripts.Skills
{
    /// <summary>
    /// Skill data for the <see cref="SkillIceBolt" /> skill.
    /// </summary>
    /// <seealso cref="SkillData{SkillParametersIceBolt, SkillTargetingUnit}" />
    [CreateAssetMenu(menuName = "Skills/IceBolt", fileName = "New IceBolt Skill")]
    internal class SkillDataIceBolt : SkillData<SkillParametersIceBolt, SkillTargetingUnit>
    {
        /// <summary>
        /// Creates the Ice Bolt skill with this skill data.
        /// </summary>
        /// <returns>The Ise Bolt skill.</returns>
        public override ISkill CreateSkill() => new SkillIceBolt(this);

        /// <summary>
        /// Launches the ice bolt at the given target (magic arrow with freeze effect).
        /// </summary>
        /// <param name="owner">The skill owner (caster).</param>
        /// <param name="target">The target.</param>
        /// <param name="skillLevel">The skill level.</param>
        /// <param name="objectsPool">The objects pool.</param>
        /// <exception cref="ArgumentNullException">
        /// owner
        /// or
        /// target
        /// or
        /// objectsPool
        /// </exception>
        public void LaunchIceBolt(ISkilled owner, ITarget target, int skillLevel, IObjectsPool objectsPool)
        {
            if (owner == null)
                throw new ArgumentNullException(nameof(owner));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (objectsPool == null)
                throw new ArgumentNullException(nameof(objectsPool));

            var parameters = GetParameters(skillLevel);
            var arrow = objectsPool.GetOrCreate(parameters.ArrowPrefab, owner.transform.position, owner.transform.rotation);
            var effect = parameters.EffectFreeze.CreateEffect(owner, objectsPool);
            arrow.MagicArrowSetup(target, owner, parameters.ArrowSpeed, effect);
        }

        /// <summary>
        /// Configures the skill targeting mode with the given skill parameters.
        /// </summary>
        /// <param name="parameters">The skill parameters.</param>
        protected override void ConfigureTargetingMode(SkillParametersIceBolt parameters)
        {
            targetingMode.SetupUsingRadius(parameters.DistanceUsing);
        }

        /// <summary>
        /// Checks the skill target for correctness.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="skilled">The skill owner (caster).</param>
        /// <returns>
        ///   <c>true</c> if the target is correct; otherwise, <c>false</c>.
        /// </returns>
        public override bool VerifyTarget(ITarget target, ISkilled skilled)
        {
            return target != null && skilled != null && target is ITargetAttackable attackable &&
                attackable.IsAlive && attackable.CanBeAttackedBy(skilled);
        }
    }
}
