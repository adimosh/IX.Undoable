using System;

namespace IX.Undoable
{
    /// <summary>
    /// Event arguments for edit committed.
    /// </summary>
    public class EditCommittedEventArgs : EventArgs
    {
        public EditCommittedEventArgs(StateChange[] stateChanges)
        {
            this.StateChanges = stateChanges;
        }

        public StateChange[] StateChanges { get; private set; }
    }
}