using Assets.Scripts.DI;

namespace Assets.Scripts.Skills
{
    /// <summary>
    /// The Ice Bolt skill. Deals damage to a single target and freezes it.
    /// </summary>
    /// <seealso cref="Skill{SkillDataIceBolt}" />
    internal class SkillIceBolt : Skill<SkillDataIceBolt>
    {
        [Injection] private IObjectsPool ObjectsPool { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillIceBolt"/> class.
        /// </summary>
        /// <param name="data">The skill data.</param>
        public SkillIceBolt(SkillDataIceBolt data) : base(data) { }

        /// <summary>
        /// Uses skill on specific target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="owner">The skill owner (caster).</param>
        public override void UseSkill(ITarget target, ISkilled owner)
        {
            data.LaunchIceBolt(owner, target, Level, ObjectsPool);
            StartRecharging();
        }
    }
}
