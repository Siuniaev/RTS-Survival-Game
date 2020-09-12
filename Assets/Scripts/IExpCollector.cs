namespace Assets.Scripts
{
    /// <summary>
    /// Can collect experience.
    /// </summary>
    internal interface IExpCollector
    {
        /// <summary>
        /// Adds the experience value to this instance.
        /// </summary>
        /// <param name="exp">The experience value.</param>
        void AddExp(long exp);
    }
}
