using IX.Undoable.Aides;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

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
        private bool isInEditMode;
        private int historyLevels;
        private Stack<TItem> undoStack;
        private Stack<TItem> redoStack;
        private IUndoableItem parentContext;
        private TItem data;
        private TItem comparisonData;
        private Func<TItem, TItem> cloningFunction;
        private Func<TItem, TItem, bool> equalsFunction;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableItemBase{TItem}"/> class.
        /// </summary>
        protected EditableItemBase()
            : this(ItemCloningAide.GenerateCloningFunction<TItem>(),
                  ItemEqualityAide.GenerateEqualityFunction<TItem>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableItemBase{TItem}"/> class.
        /// </summary>
        /// <param name="cloningFunction">The cloning function.</param>
        /// <param name="equalsFunction">The equals function.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="cloningFunction"/>
        /// or
        /// <paramref name="equalsFunction"/>
        /// is <c>null</c> (<c>Nothing</c> in Visual Basic).
        /// </exception>
        protected EditableItemBase(Func<TItem, TItem> cloningFunction, Func<TItem, TItem, bool> equalsFunction)
        {
            this.cloningFunction = cloningFunction ?? throw new ArgumentNullException(nameof(cloningFunction));
            this.equalsFunction = equalsFunction ?? throw new ArgumentNullException(nameof(equalsFunction));

            this.undoStack = new Stack<TItem>();
            this.redoStack = new Stack<TItem>();
        }

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

        public bool IsInEditMode => this.isInEditMode;

        public bool IsCapturedInUndoContext => this.parentContext != null;

        public bool CanUndo => this.IsCapturedInUndoContext || this.undoStack.Count > 0;

        public bool CanRedo => this.IsCapturedInUndoContext || this.redoStack.Count > 0;

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

        public void CancelEdit()
        {
            if (!this.isInEditMode)
            {
                throw new ItemNotInEditModeException();
            }

            if (!this.equalsFunction(this.data, this.comparisonData))
            {
                RestoreOriginalValues(this.data, this.comparisonData);
            }
        }

        public void CommitEdit()
        {
            if (!this.isInEditMode)
            {
                throw new ItemNotInEditModeException();
            }

            this.comparisonData = this.cloningFunction(data);
        }

        public void EndEdit()
        {
            if (!this.isInEditMode)
            {
                throw new ItemNotInEditModeException();
            }

            this.isInEditMode = false;

            RaisePropertyChanged(nameof(IsInEditMode));
        }

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

        public void ReleaseFromUndoContext()
        {
            if (this.parentContext == null)
            {
                return;
            }

            this.parentContext = null;

            RaisePropertyChanged(nameof(IsCapturedInUndoContext));
        }

        public void Undo() => throw new NotImplementedException();

        public void Redo() => throw new NotImplementedException();

        /// <summary>
        /// When implemented in a child class, raises the property changed event of <see cref="INotifyPropertyChanged"/>.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
        }

        protected abstract void RestoreOriginalValues(TItem currentItem, TItem originalData);

        protected TItem DataItem => this.data;
    }
}