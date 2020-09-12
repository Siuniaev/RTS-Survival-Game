namespace Assets.Scripts
{
    /// <summary>
    /// The structure for the changed value.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    internal readonly struct ChangedValue<T>
        where T : struct
    {
        public readonly T OldValue, NewValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangedValue{T}"/> struct.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public ChangedValue(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangedValue{T}"/> struct.
        /// </summary>
        /// <param name="values">The current value.</param>
        public ChangedValue(T values)
        {
            OldValue = NewValue = values;
        }
    }
}
