using System;

namespace Assets.Scripts.Skills
{
    /// <summary>
    /// Arguments of the selected skill target.
    /// </summary>
    /// <seealso cref="SkillTargetArgs" />
    internal class SkillTargetArgsSelecting : SkillTargetArgs
    {
        public readonly ISkilled SkillOwner;
        public readonly ISkill Skill;
        public readonly int IndexButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillTargetArgsSelecting"/> class.
        /// </summary>
        /// <param name="skillOwner">The skill owner.</param>
        /// <param name="skill">The skill.</param>
        /// <param name="indexButton">The skill button index.</param>
        /// <exception cref="ArgumentNullException">
        /// skillOwner
        /// or
        /// skill
        /// </exception>
        public SkillTargetArgsSelecting(ISkilled skillOwner, ISkill skill, int indexButton) : base(false)
        {
            SkillOwner = skillOwner ?? throw new ArgumentNullException(nameof(skillOwner));
            Skill = skill ?? throw new ArgumentNullException(nameof(skill));
            IndexButton = indexButton;
        }
    }
}
