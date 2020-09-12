namespace Assets.Scripts.Skills.Effects
{
    /// <summary>
    /// Has an <see cref="IEffectsInspector" /> that can take <see cref="IEffect" /> instances.
    /// </summary>
    internal interface IEffectable
    {
        /// <summary>
        /// Gets the effects inspector.
        /// </summary>
        /// <value>The effects inspector.</value>
        IEffectsInspector EffectsInspector { get; }
    }
}
