namespace Assets.Scripts.UI
{
    /// <summary>
    /// Creates and controls all UI objects in the game.
    /// </summary>
    internal interface IHUD
    {
        /// <summary>
        /// Gets the scale factor for UI objects.
        /// </summary>
        /// <returns>The scale factor.</returns>
        float GetScaleFactor();

        /// <summary>
        /// Creates the health bar for healthed object.
        /// </summary>
        /// <typeparam name="T">The healthed object type.</typeparam>
        /// <param name="targetObj">The target object.</param>
        void CreateHealthBarFor<T>(T targetObj) where T : IShowableHealth;
    }
}
