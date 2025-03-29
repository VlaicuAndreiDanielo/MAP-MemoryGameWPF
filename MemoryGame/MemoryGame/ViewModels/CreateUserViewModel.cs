using MemoryGame.Commands;
using MemoryGame.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace MemoryGame.ViewModels
{
    public class CreateUserViewModel : INotifyPropertyChanged
    {
        private string _userName;

        public CreateUserViewModel()
        {
            CreateCommand = new RelayCommand(CreateUser);
            CancelCommand = new RelayCommand(Cancel);
        }

        public string UserName
        {
            get => _userName;
            set { _userName = value; OnPropertyChanged(nameof(UserName)); }
        }

        // Aici vei stoca noul utilizator creat
        public User NewUser { get; private set; }

        public ICommand CreateCommand { get; }
        public ICommand CancelCommand { get; }

        private void CreateUser(object parameter)
        {
            // Validează numele, dacă vrei
            if (string.IsNullOrWhiteSpace(UserName))
            {
                MessageBox.Show("Please enter a valid user name.");
                return;
            }

            // Creăm utilizatorul
            NewUser = new User
            {
                Name = UserName,
                ImagePath = "Images/default.png" // Poți schimba cum vrei
            };

            // Închidem fereastra și setăm DialogResult = true
            if (parameter is Window window)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        private void Cancel(object parameter)
        {
            if (parameter is Window window)
            {
                window.DialogResult = false;
                window.Close();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
