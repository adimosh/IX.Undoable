namespace IX.Undoable
{
    /// <summary>
    /// A state change belonging to a sub-object.
    /// </summary>
    public class SubItemStateChange : StateChange
    {
        /// <summary>
        /// The instance of the sub-object.
        /// </summary>
        public object SubObject { get; set; }

        /// <summary>
        /// The state changes belonging to the sub-object.
        /// </summary>
        public StateChange[] StateChanges { get; set; }
    }
}