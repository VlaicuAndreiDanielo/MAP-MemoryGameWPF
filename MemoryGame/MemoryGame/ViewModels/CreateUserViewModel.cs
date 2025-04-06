using MemoryGame.Commands;
using MemoryGame.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace MemoryGame.ViewModels
{
    public class CreateUserViewModel : INotifyPropertyChanged
    {
        private string _userName;
        private int _selectedAvatarIndex;
        private ObservableCollection<string> _availableAvatars;

        public CreateUserViewModel()
        {
            _availableAvatars = new ObservableCollection<string>
            {
                "Images/avatar1.png","Images/avatar2.png","Images/avatar3.png","Images/avatar4.png","Images/avatar5.png","Images/avatar6.png",
                "Images/avatar7.png","Images/avatar8.png","Images/avatar9.png","Images/avatar10.png","Images/avatar11.png","Images/avatar12.png",
                "Images/avatar13.png","Images/avatar14.png","Images/avatar15.png","Images/avatar16.png","Images/avatar17.png","Images/avatar18.png",
                "Images/avatar19.png","Images/avatar20.png","Images/avatar21.png","Images/avatar22.png","Images/avatar23.png","Images/avatar24.png",
                "Images/avatar25.png","Images/avatar26.png","Images/avatar27.png","Images/avatar28.png","Images/avatar29.png","Images/avatar30.png",
                "Images/AvatarSpecial.png","Images/AvatarSpecial2.png","Images/AvatarSpecial3.png"
            };
            _selectedAvatarIndex = 0;

            CreateCommand = new RelayCommand(CreateUser);
            CancelCommand = new RelayCommand(Cancel);
            PrevImageCommand = new RelayCommand(PrevImage);
            NextImageCommand = new RelayCommand(NextImage);
        }

        public string UserName
        {
            get => _userName;
            set { _userName = value; OnPropertyChanged(nameof(UserName)); }
        }

        public string SelectedAvatarPath
        {
            get
            {
                if (_availableAvatars.Count > 0 && _selectedAvatarIndex >= 0 && _selectedAvatarIndex < _availableAvatars.Count)
                    return _availableAvatars[_selectedAvatarIndex];
                return null;
            }
        }

        public ICommand CreateCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand PrevImageCommand { get; }
        public ICommand NextImageCommand { get; }
        public User NewUser { get; private set; }

        private void CreateUser(object parameter)
        {
            if (string.IsNullOrWhiteSpace(UserName))
            {
                MessageBox.Show("Please enter a valid user name.");
                return;
            }

            NewUser = new User
            {
                Name = UserName,
                ImagePath = SelectedAvatarPath
            };

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

        private void PrevImage(object parameter)
        {
            _selectedAvatarIndex--;
            if (_selectedAvatarIndex < 0)
                _selectedAvatarIndex = _availableAvatars.Count - 1;
            OnPropertyChanged(nameof(SelectedAvatarPath));
        }

        private void NextImage(object parameter)
        {
            _selectedAvatarIndex++;
            if (_selectedAvatarIndex >= _availableAvatars.Count)
                _selectedAvatarIndex = 0;
            OnPropertyChanged(nameof(SelectedAvatarPath));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
