namespace Assets.Scripts.UI
{
    /// <summary>
    /// Can show <see cref="IShowable" /> objects in the UI.
    /// </summary>
    internal interface IShowablesPresenter
    {
        /// <summary>
        /// Sets the showable to show in the UI.
        /// </summary>
        /// <param name="showable">The showable.</param>
        void SetShowable(IShowable showable);

        /// <summary>
        /// Remove the last shown showable from the UI.
        /// </summary>
        void ClearLastShowable();
    }
}
