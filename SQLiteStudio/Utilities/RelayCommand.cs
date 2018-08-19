using System;
using System.Windows.Input;

namespace SQLiteStudio.Utilities
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged = delegate { };

        public RelayCommand(Action execute)
        {
            _execute = execute;
        }
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
            {
                return _canExecute();
            }
            if (_execute != null)
            {
                return true;
            }
            return false;
        }

        public void Execute(object parameter)
        {
            if (_execute != null)
            {
                _execute.Invoke();
            }
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<bool> _canExecute;
        public event EventHandler CanExecuteChanged;
        public RelayCommand(Action<T> execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public RelayCommand(Action<T> execute)
        {
            _execute = execute;
        }
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
            {
                return _canExecute();
            }
            if (_execute != null)
            {
                return true;
            }
            return false;
        }

        public void Execute(object parameter)
        {
            if (_execute != null)
            {
                _execute.Invoke((T)parameter);
            }
        }
    }

}
