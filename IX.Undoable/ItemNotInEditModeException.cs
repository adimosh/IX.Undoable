using System;

namespace IX.Undoable
{
    public class ItemNotInEditModeException : InvalidOperationException
    {
        public ItemNotInEditModeException()
            : base(Resources.ItemNotInEditModeExceptionDefaultMessage)
        { }

        public ItemNotInEditModeException(string message)
            : base(message)
        { }

        public ItemNotInEditModeException(Exception innerException)
            : base(Resources.ItemNotInEditModeExceptionDefaultMessage, innerException)
        { }

        public ItemNotInEditModeException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}