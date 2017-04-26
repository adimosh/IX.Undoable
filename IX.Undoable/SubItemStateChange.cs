namespace IX.Undoable
{
    public class SubItemStateChange : StateChange
    {
        public object SubObject { get; set; }

        public StateChange[] StateChanges { get; set; }
    }
}