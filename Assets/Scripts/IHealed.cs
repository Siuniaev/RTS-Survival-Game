namespace Assets.Scripts
{
    /// <summary>
    /// Can be healed by <see cref="IHealer" /> objects.
    /// </summary>
    internal interface IHealed
    {
        /// <summary>
        /// Accepts the healer.
        /// </summary>
        /// <param name="healer">The healer.</param>
        void AcceptHealer(IHealer healer);
    }
}
