using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Snake.WPF.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region Constructors

        protected ViewModelBase() { }

        #endregion

        #region Events

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Protected Methods

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
