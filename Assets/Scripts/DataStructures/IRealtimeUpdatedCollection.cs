namespace Assets.Scripts.DataStructures
{
    /// <summary>
    /// Collection with manual update in real-time.
    /// </summary>
    internal interface IRealtimeUpdatedCollection
    {
        /// <summary>
        /// Updates collection a specified number of times per second.
        /// </summary>
        /// <param name="rate">The rate.</param>
        void Update(float rate);
    }
}