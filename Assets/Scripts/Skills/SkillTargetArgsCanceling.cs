namespace Assets.Scripts.Skills
{
    /// <summary>
    /// The cancelation as selected skill target.
    /// </summary>
    /// <seealso cref="SkillTargetArgs" />
    internal class SkillTargetArgsCanceling : SkillTargetArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkillTargetArgsCanceling"/> class.
        /// </summary>
        public SkillTargetArgsCanceling() : base(true) { }
    }
}
