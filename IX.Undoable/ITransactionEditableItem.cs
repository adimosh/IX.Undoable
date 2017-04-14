using System;
using System.Collections.Generic;
using System.Text;

namespace IX.Undoable
{
    /// <summary>
    /// A contract for an item that is editable in a transactional-style way.
    /// </summary>
    public interface ITransactionEditableItem
    {
        /// <summary>
        /// Begins the editing of an item.
        /// </summary>
        void BeginEdit();

        /// <summary>
        /// Commits the changes to the item as they are, without ending the editing.
        /// </summary>
        void CommitEdit();

        /// <summary>
        /// Discards all changes to the item, reloading the state at the last commit or at the beginning of the edit transaction, whichever occurred last.
        /// </summary>
        void CancelEdit();

        /// <summary>
        /// Ends the editing of an item.
        /// </summary>
        void EndEdit();

        /// <summary>
        /// Gets a value indicating whether this instance is in edit mode.
        /// </summary>
        /// <value><c>true</c> if this instance is in edit mode; otherwise, <c>false</c>.</value>
        bool IsInEditMode { get; }

        /// <summary>
        /// Occurs when an edit on this item is committed.
        /// </summary>
        event EventHandler<EditCommittedEventArgs> EditCommitted;
    }
}
