using System;

namespace IX.Undoable
{
    public class ItemIsInEditModeException : InvalidOperationException
    {
        public ItemIsInEditModeException()
            : base(Resources.ItemIsInEditModeExceptionDefaultMessage)
        { }

        public ItemIsInEditModeException(string message)
            : base(message)
        { }

        public ItemIsInEditModeException(Exception innerException)
            : base(Resources.ItemIsInEditModeExceptionDefaultMessage, innerException)
        { }

        public ItemIsInEditModeException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}