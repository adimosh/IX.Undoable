using IX.Undoable.Aides;
using System;
using System.Collections.Generic;

namespace IX.Undoable
{
    /// <summary>
    /// A base class for editable items that can be edited in a transactional-style pattern.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <seealso cref="IX.Undoable.ITransactionEditableItem" />
    /// <seealso cref="IX.Undoable.IUndoableItem" />
    public abstract class EditableItemBase<TItem> : ITransactionEditableItem, IUndoableItem
    {
        /// <summary>
        /// The value indicating whether the item is in edit mode
        /// </summary>
        private bool isInEditMode;

        /// <summary>
        /// The history levels
        /// </summary>
        private int historyLevels;

        /// <summary>
        /// The undo stack
        /// </summary>
        private Stack<TItem> undoStack;

        /// <summary>
        /// The redo stack
        /// </summary>
        private Stack<TItem> redoStack;

        /// <summary>
        /// The parent context
        /// </summary>
        private IUndoableItem parentContext;

        /// <summary>
        /// The data
        /// </summary>
        private TItem data;

        /// <summary>
        /// The comparison data
        /// </summary>
        private TItem comparisonData;

        /// <summary>
        /// The cloning function
        /// </summary>
        private Func<TItem, TItem> cloningFunction;

        /// <summary>
        /// The equals function
        /// </summary>
        private Func<TItem, TItem, bool> equalsFunction;

        /// <summary>
        /// Occurs when an edit on this item is committed.
        /// </summary>
        public event EventHandler<EditCommittedEventArgs> EditCommitted;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableItemBase{TItem}" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        protected EditableItemBase(TItem data)
            : this(data,
                  ItemCloningAide.GenerateCloningFunction<TItem>(),
                  ItemEqualityAide.GenerateEqualityFunction<TItem>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableItemBase{TItem}" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="cloningFunction">The cloning function.</param>
        /// <param name="equalsFunction">The equals function.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="cloningFunction" />
        /// or
        /// <paramref name="equalsFunction" />
        /// is <c>null</c> (<c>Nothing</c> in Visual Basic).</exception>
        protected EditableItemBase(TItem data, Func<TItem, TItem> cloningFunction, Func<TItem, TItem, bool> equalsFunction)
        {
            this.cloningFunction = cloningFunction ?? throw new ArgumentNullException(nameof(cloningFunction));
            this.equalsFunction = equalsFunction ?? throw new ArgumentNullException(nameof(equalsFunction));

            this.undoStack = new Stack<TItem>();
            this.redoStack = new Stack<TItem>();

            this.data = data;
        }

        /// <summary>
        /// The number of levels to keep undo or redo information.
        /// </summary>
        /// <value>The history levels.</value>
        /// <remarks><para>If this value is set, for example, to 7, then the implementing object should allow the <see cref="Undo" /> method
        /// to be called 7 times to change the state of the object. Upon calling it an 8th time, there should be no change in the
        /// state of the object.</para>
        /// <para>Any call beyond the limit imposed here should not fail, but it should also not change the state of the object.</para></remarks>
        public int HistoryLevels
        {
            get => historyLevels;
            set
            {
                if (value < 0)
                {
                    this.historyLevels = 0;
                }
                else
                {
                    if (this.historyLevels != value)
                    {
                        this.historyLevels = value;

                        RaisePropertyChanged(nameof(HistoryLevels));
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is in edit mode.
        /// </summary>
        /// <value><c>true</c> if this instance is in edit mode; otherwise, <c>false</c>.</value>
        public bool IsInEditMode => this.isInEditMode;

        /// <summary>
        /// Gets a value indicating whether this instance is captured in undo context.
        /// </summary>
        /// <value><c>true</c> if this instance is captured in undo context; otherwise, <c>false</c>.</value>
        public bool IsCapturedInUndoContext => this.parentContext != null;

        /// <summary>
        /// Gets whether or not an undo can be performed on this item.
        /// </summary>
        /// <value><c>true</c> if the call to the <see cref="Undo" /> method would result in a state change, <c>false</c> otherwise.</value>
        public bool CanUndo => this.IsCapturedInUndoContext || this.undoStack.Count > 0;

        /// <summary>
        /// Gets whether or not a redo can be performed on this item.
        /// </summary>
        /// <value><c>true</c> if the call to the <see cref="Redo" /> method would result in a state change, <c>false</c> otherwise.</value>
        public bool CanRedo => this.IsCapturedInUndoContext || this.redoStack.Count > 0;

        /// <summary>
        /// Begins the editing of an item.
        /// </summary>
        public void BeginEdit()
        {
            if (this.isInEditMode)
            {
                return;
            }

            this.isInEditMode = true;

            this.comparisonData = this.cloningFunction(this.data);

            RaisePropertyChanged(nameof(IsInEditMode));
        }

        /// <summary>
        /// Discards all changes to the item, reloading the state at the last commit or at the beginning of the edit transaction, whichever occurred last.
        /// </summary>
        /// <exception cref="IX.Undoable.ItemNotInEditModeException"></exception>
        public void CancelEdit()
        {
            if (!this.isInEditMode)
            {
                throw new ItemNotInEditModeException();
            }

            if (!this.equalsFunction(this.data, this.comparisonData))
            {
                SetChangedValues(this.data, this.comparisonData);
            }
        }

        /// <summary>
        /// Commits the changes to the item as they are, without ending the editing.
        /// </summary>
        /// <exception cref="IX.Undoable.ItemNotInEditModeException"></exception>
        public void CommitEdit()
        {
            if (!this.isInEditMode)
            {
                throw new ItemNotInEditModeException();
            }

            CommitEditInternal();
        }

        /// <summary>
        /// Ends the editing of an item.
        /// </summary>
        /// <exception cref="IX.Undoable.ItemNotInEditModeException"></exception>
        public void EndEdit()
        {
            if (!this.isInEditMode)
            {
                throw new ItemNotInEditModeException();
            }

            this.isInEditMode = false;

            RaisePropertyChanged(nameof(IsInEditMode));
        }

        /// <summary>
        /// Allows the item to be captured by a containing undo-/redo-capable object so that undo and redo operations
        /// can be coordinated across a larger scope.
        /// </summary>
        /// <param name="parent">The parent undo and redo context.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="parent" /> is <c>null</c> (<c>Nothing</c> in Visual Basic).</exception>
        /// <exception cref="IX.Undoable.ItemIsInEditModeException">The item is in edit mode, and this operation cannot be performed at this time.</exception>
        /// <remarks>This method is meant to be used by containers, and should not be called directly.</remarks>
        public void CaptureIntoUndoContext(IUndoableItem parent)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            if (parent == this.parentContext)
            {
                return;
            }

            if (this.isInEditMode)
            {
                throw new ItemIsInEditModeException();
            }

            this.parentContext = parent;
            this.undoStack.Clear();
            this.redoStack.Clear();

            RaisePropertyChanged(nameof(IsCapturedInUndoContext));
        }

        /// <summary>
        /// Releases the item from being captured into an undo and redo context.
        /// </summary>
        /// <remarks>This method is meant to be used by containers, and should not be called directly.</remarks>
        public void ReleaseFromUndoContext()
        {
            if (this.parentContext == null)
            {
                return;
            }

            this.parentContext = null;

            RaisePropertyChanged(nameof(IsCapturedInUndoContext));
        }

        /// <summary>
        /// Has the last operation performed on the item undone.
        /// </summary>
        /// <remarks><para>If the object is captured, the method will call the capturing parent's Undo method, which can bubble down to
        /// the last instance of an undo-/redo-capable object.</para>
        /// <para>If that is the case, the capturing object is solely responsible for ensuring that the inner state of the whole
        /// system is correct. Implementing classes should not expect this method to also handle state.</para>
        /// <para>If the object is released, it is expected that this method once again starts ensuring state when called.</para></remarks>
        public void Undo()
        {
            if (this.parentContext != null)
            {
                // We are captured by a parent context, let's invoke that context's Undo.
                this.parentContext.Undo();
                return;
            }

            // We are not captured, let's proceed with Undo.

            if (this.undoStack.Count == 0)
            {
                // We don't have anything to Undo.
                return;
            }

            TItem undoData = this.undoStack.Pop();
            this.redoStack.Push(this.cloningFunction(this.data));
            SetChangedValues(this.data, undoData);

            RaisePropertyChanged(nameof(CanUndo));
            RaisePropertyChanged(nameof(CanRedo));
        }

        /// <summary>
        /// Has the last undone operation performed on this item, presuming that it has not changed since then, redone.
        /// </summary>
        /// <remarks><para>If the object is captured, the method will call the capturing parent's Redo method, which can bubble down to
        /// the last instance of an undo-/redo-capable object.</para>
        /// <para>If that is the case, the capturing object is solely responsible for ensuring that the inner state of the whole
        /// system is correct. Implementing classes should not expect this method to also handle state.</para>
        /// <para>If the object is released, it is expected that this method once again starts ensuring state when called.</para></remarks>
        public void Redo()
        {
            if (this.parentContext != null)
            {
                // We are captured by a parent context, let's invoke that context's Redo.
                this.parentContext.Redo();
                return;
            }

            // We are not captured, let's proceed with Redo.

            if (this.redoStack.Count == 0)
            {
                // We don't have anything to Redo.
                return;
            }

            TItem redoData = this.redoStack.Pop();
            this.undoStack.Push(this.cloningFunction(this.data));
            SetChangedValues(this.data, redoData);

            RaisePropertyChanged(nameof(CanUndo));
            RaisePropertyChanged(nameof(CanRedo));
        }

        /// <summary>
        /// Captures a sub item into the present context.
        /// </summary>
        /// <typeparam name="TSubItem">The type of the sub item.</typeparam>
        /// <param name="item">The item.</param>
        /// <remarks>
        /// <para>
        /// This method is intended to capture only objects that are directly sub-objects that can have their own internal state and undo/redo
        /// capabilities and are also transactional in nature when being edited. Using this method on any other object may yield unwanted
        /// commits.
        /// </para>
        /// </remarks>
        protected void CaptureSubItemIntoPresentContext<TSubItem>(TSubItem item)
            where TSubItem : IUndoableItem, ITransactionEditableItem
        {
            item.CaptureIntoUndoContext(this);

            item.EditCommitted += this.Item_EditCommitted;
        }

        /// <summary>
        /// Handles the EditCommitted event of the sub-item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EditCommittedEventArgs"/> instance containing the event data.</param>
        private void Item_EditCommitted(object sender, EditCommittedEventArgs e) => CommitEditInternal();

        /// <summary>
        /// When implemented in a child class, raises the property changed event of <see cref="INotifyPropertyChanged" />.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
        }

        /// <summary>
        /// Sets the changed values.
        /// </summary>
        /// <param name="currentItem">The current item.</param>
        /// <param name="originalData">The original data.</param>
        protected abstract void SetChangedValues(TItem currentItem, TItem originalData);

        /// <summary>
        /// Gets the data item.
        /// </summary>
        /// <value>The data item.</value>
        protected TItem DataItem => this.data;

        /// <summary>
        /// Commits the edit internal.
        /// </summary>
        private void CommitEditInternal()
        {
            if (this.parentContext != null)
            {
                this.undoStack.Push(this.comparisonData);
            }

            this.comparisonData = this.cloningFunction(this.data);
            this.redoStack.Clear();

            RaisePropertyChanged(nameof(CanUndo));
            RaisePropertyChanged(nameof(CanRedo));

            EditCommitted?.Invoke(this, new EditCommittedEventArgs());
        }
    }
}