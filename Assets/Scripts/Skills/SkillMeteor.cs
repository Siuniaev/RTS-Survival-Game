using Assets.Scripts.DI;
using System;

namespace Assets.Scripts.Skills
{
    /// <summary>
    /// The Meteor skill. Spawns meteorites in a selected area and deals damage to enemies.
    /// </summary>
    /// <seealso cref="Skill{SkillDataMeteor}" />
    internal class SkillMeteor : Skill<SkillDataMeteor>
    {
        [Injection] private ITargetsProvider TargetsProvider { get; set; }
        [Injection] private IObjectsPool ObjectsPool { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillMeteor"/> class.
        /// </summary>
        /// <param name="data">The skill data.</param>
        public SkillMeteor(SkillDataMeteor data) : base(data) { }

        /// <summary>
        /// Uses skill on specific target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="owner">The skill owner (caster).</param>
        /// <exception cref="ArgumentException">target</exception>
        public override void UseSkill(ITarget target, ISkilled owner)
        {
            var areaTarget = target as AreaTarget ?? throw new ArgumentException(nameof(target));
            data.CreateMeteorShower(owner, TargetsProvider, areaTarget, Level, ObjectsPool);
            StartRecharging();
        }
    }
}
