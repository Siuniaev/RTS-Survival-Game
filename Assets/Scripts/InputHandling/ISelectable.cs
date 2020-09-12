namespace Assets.Scripts.InputHandling
{
    /// <summary>
    /// Can be selected.
    /// </summary>
    internal interface ISelectable
    {
        /// <summary>
        /// Sets the selected.
        /// </summary>
        /// <param name="selected">if set to <c>true</c> is selected.</param>
        void SetSelected(bool selected);
    }
}
