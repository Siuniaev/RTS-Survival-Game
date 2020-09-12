namespace Assets.Scripts.Skills
{
    /// <summary>
    /// Uses skill targeting mode (<see cref="SkillTargetingMode" />).
    /// </summary>
    internal interface ISkillTargetingHandler
    {
        /// <summary>
        /// Handles the specified area targeting mode.
        /// </summary>
        /// <param name="mode">The area targeting mode.</param>
        void Handle(SkillTargetingArea mode);

        /// <summary>
        /// Handles the specified unit targeting mode.
        /// </summary>
        /// <param name="mode">The unit targeting mode.</param>
        void Handle(SkillTargetingUnit mode);
    }
}
