using System.ComponentModel;
using System.Windows.Threading;

namespace IX.Undoable.WPF
{
    /// <summary>
    /// A base class for undo-able view models.
    /// </summary>
    public abstract class UndoableViewModelBase : EditableItemBase, INotifyPropertyChanged, IEditableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UndoableViewModelBase"/> class.
        /// </summary>
        protected UndoableViewModelBase()
        {
        }

        /// <summary>
        /// Triggered by a change in a property.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that should trigger the event with.</param>
        protected override void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
            {
                return;
            }

            if (Dispatcher.CurrentDispatcher == null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            else
            {
                Dispatcher.CurrentDispatcher.InvokeAsync(
                    () => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)),
                    DispatcherPriority.ApplicationIdle);
            }
        }
    }
}