namespace IX.Undoable
{
    /// <summary>
    /// A state change of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of object in the state change.</typeparam>
    public class StateChange<T> : StateChange
    {
        /// <summary>
        /// Gets or sets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the old value.
        /// </summary>
        /// <value>The old value.</value>
        public T OldValue { get; set; }

        /// <summary>
        /// Gets or sets the new value.
        /// </summary>
        /// <value>The new value.</value>
        public T NewValue { get; set; }
    }
}