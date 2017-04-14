using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace IX.Undoable.WPF
{
    public abstract class UndoableViewModelBase<TItem> : EditableItemBase<TItem>, INotifyPropertyChanged
    {
        protected UndoableViewModelBase(TItem data)
            : base(data)
        {
        }

        protected UndoableViewModelBase(TItem data, Func<TItem, TItem> cloningFunction, Func<TItem, TItem, bool> equalsFunction)
            : base(data)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected override void RaisePropertyChanged(string propertyName)
        {
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