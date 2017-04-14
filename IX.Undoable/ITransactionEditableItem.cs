using System;
using System.Collections.Generic;
using System.Text;

namespace IX.Undoable
{
    public interface ITransactionEditableItem
    {
        void BeginEdit();

        void CommitEdit();

        void CancelEdit();

        void EndEdit();

        bool IsInEditMode { get; }
    }
}
