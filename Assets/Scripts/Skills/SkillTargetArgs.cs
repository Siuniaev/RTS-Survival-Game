namespace Assets.Scripts.Skills
{
    /// <summary>
    /// Base class for all arguments of the selected skill target.
    /// </summary>
    internal abstract class SkillTargetArgs
    {
        /// <summary>
        /// Gets the cancelation as selected skill target.
        /// </summary>
        /// <value>The cancelation.</value>
        public static SkillTargetArgs Cancelation => new SkillTargetArgsCanceling();

        /// <summary>
        /// The is cancelation.
        /// </summary>
        public readonly bool IsCancelation;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillTargetArgs"/> class.
        /// </summary>
        /// <param name="isCancelation">if set to <c>true</c> this instance is cancelation.</param>
        protected SkillTargetArgs(bool isCancelation) => IsCancelation = isCancelation;
    }
}
