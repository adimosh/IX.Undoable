using System;

namespace IX.Undoable
{
    public class ItemAlreadyCapturedIntoUndoContextException : InvalidOperationException
    {
        public ItemAlreadyCapturedIntoUndoContextException()
            : base(Resources.ItemAlreadyCapturedIntoUndoContextException)
        { }

        public ItemAlreadyCapturedIntoUndoContextException(string message)
            : base(message)
        { }

        public ItemAlreadyCapturedIntoUndoContextException(Exception innerException)
            : base(Resources.ItemAlreadyCapturedIntoUndoContextException, innerException)
        { }

        public ItemAlreadyCapturedIntoUndoContextException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}