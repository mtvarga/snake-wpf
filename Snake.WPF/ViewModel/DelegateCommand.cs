using System;
using System.Windows.Input;

namespace Snake.WPF.ViewModel
{
    public class DelegateCommand : ICommand
    {
        #region Private Members

        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        #endregion

        #region Events

        public event EventHandler? CanExecuteChanged;

        #endregion

        #region Constructors

        public DelegateCommand(Action<object?> execute) : this(null, execute) { }

        public DelegateCommand(Predicate<object?>? canExecute, Action<object?> execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        #endregion

        #region Public Methods

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
                throw new InvalidOperationException("Command execution is disabled.");

            _execute(parameter);
        }

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        #endregion
    }
}
