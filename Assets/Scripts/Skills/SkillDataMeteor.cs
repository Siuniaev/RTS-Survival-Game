using System;
using UnityEngine;

namespace Assets.Scripts.Skills
{
    /// <summary>
    /// Skill data for the <see cref="SkillMeteor" /> skill.
    /// </summary>
    /// <seealso cref="SkillData{SkillParametersMeteor, SkillTargetingArea}" />
    [CreateAssetMenu(menuName = "Skills/Meteor", fileName = "New Meteor Skill")]
    internal class SkillDataMeteor : SkillData<SkillParametersMeteor, SkillTargetingArea>
    {
        /// <summary>
        /// Creates the Meteor skill with this skill data.
        /// </summary>
        /// <returns>The Meteor skill.</returns>
        public override ISkill CreateSkill() => new SkillMeteor(this);

        /// <summary>
        /// Creates the meteor shower - helper-object that spawns meteorites in a given area and deals damage to enemies.
        /// </summary>
        /// <param name="owner">The skill owner (caster).</param>
        /// <param name="targetsProvider">The targets provider.</param>
        /// <param name="target">The target.</param>
        /// <param name="skillLevel">The skill level.</param>
        /// <param name="objectsPool">The objects pool.</param>
        /// <exception cref="ArgumentNullException">
        /// owner
        /// or
        /// targetsProvider
        /// or
        /// target
        /// or
        /// objectsPool
        /// </exception>
        public void CreateMeteorShower(ISkilled owner, ITargetsProvider targetsProvider, AreaTarget target, int skillLevel,
            IObjectsPool objectsPool)
        {
            if (owner == null)
                throw new ArgumentNullException(nameof(owner));

            if (targetsProvider == null)
                throw new ArgumentNullException(nameof(targetsProvider));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (objectsPool == null)
                throw new ArgumentNullException(nameof(objectsPool));

            var parameters = GetParameters(skillLevel);
            var meteor = objectsPool.GetOrCreate(parameters.MeteorShowerPrefab);
            meteor.Setup(target, parameters.Damage, targetsProvider, owner);
        }

        /// <summary>
        /// Configures the skill targeting mode with the given skill parameters.
        /// </summary>
        /// <param name="parameters">The skill parameters.</param>
        protected override void ConfigureTargetingMode(SkillParametersMeteor parameters)
        {
            targetingMode.SetupAreaRadius(parameters.BlastRadius);
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
            return target is AreaTarget;
        }
    }
}
