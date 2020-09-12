namespace Assets.Scripts.Skills.Effects
{
    /// <summary>
    /// Takes <see cref="IEffect" /> instances and oversees them.
    /// </summary>
    internal interface IEffectsInspector
    {
        /// <summary>
        /// Tries the apply effect.
        /// </summary>
        /// <param name="effect">The effect.</param>
        /// <returns>
        ///   <c>true</c> if the effect was applied successfully; otherwise, <c>false</c>.
        /// </returns>
        bool TryApplyEffect(IEffect effect);

        /// <summary>
        /// Clears the current taken effects.
        /// </summary>
        void ClearEffects();
    }
}
