using MemoryGame.Commands;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace MemoryGame.ViewModels
{
    public class ConfirmDeleteViewModel : INotifyPropertyChanged
    {
        private Window _window;
        private string _message;

        public ConfirmDeleteViewModel(string userName, Window window)
        {
            _window = window;
            Message = $"Ești sigur că vrei să ștergi userul '{userName}'?";
            OkCommand = new RelayCommand(_ => Ok());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        public string Message
        {
            get => _message;
            set { _message = value; OnPropertyChanged(nameof(Message)); }
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        private void Ok()
        {
            if (_window != null)
            {
                _window.DialogResult = true;
                _window.Close();
            }
        }

        private void Cancel()
        {
            if (_window != null)
            {
                _window.DialogResult = false;
                _window.Close();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
