namespace Assets.Scripts.Skills.Effects
{
    /// <summary>
    /// Effect data, which is set in <see cref="SkillParameters" /> to create an <see cref="IEffect" />
    /// for the <see cref="IEffectable" /> target when using the <see cref="ISkill" /> on it.
    /// </summary>
    internal interface IEffectData
    {
        /// <summary>
        /// Effect duration in seconds.
        /// </summary>
        /// <value>The duration.</value>
        float Duration { get; }

        /// <summary>
        /// Checks the correctness of the entered data.
        /// </summary>
        void OnValidate();

        /// <summary>
        /// Creates the effect.
        /// </summary>
        /// <param name="effectOwner">The effect owner.</param>
        /// <param name="objectsPool">The objects pool.</param>
        /// <returns>The effect using this effect date.</returns>
        IEffect CreateEffect(ITeamMember effectOwner, IObjectsPool objectsPool);

        /// <summary>
        /// Applies an effect with this data to the selected effectable target.
        /// </summary>
        /// <param name="effectable">The effectable target.</param>
        /// <param name="effect">The effect.</param>
        void UseOn(IEffectable effectable, IEffect effect);
    }
}
