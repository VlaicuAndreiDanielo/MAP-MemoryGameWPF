using MemoryGame.Commands;
using MemoryGame.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Newtonsoft.Json;
using System.IO;  // Added for File operations
using System.Windows;
using MemoryGame.Views;

namespace MemoryGame.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private User _selectedUser;
        private const string UsersFilePath = "users.json";

        public ObservableCollection<User> Users { get; set; }

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand CreateUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand CancelCommand { get; }  // Added initialization in constructor

        public LoginViewModel()
        {
            LoadUsers();

            CreateUserCommand = new RelayCommand(CreateUser);
            DeleteUserCommand = new RelayCommand(DeleteUser, CanModifyUser);
            PlayCommand = new RelayCommand(PlayGame, CanModifyUser);
            CancelCommand = new RelayCommand(CloseWindow);  // Initialize CancelCommand
        }

        private bool CanModifyUser(object parameter)
        {
            return SelectedUser != null;
        }

        private void CreateUser(object parameter)
        {
            // Instanțiere fereastră, nu ViewModel
            var createUserView = new CreateUserView();

            // Acum poți apela ShowDialog() fiindcă createUserView e un Window
            bool? result = createUserView.ShowDialog();

            if (result == true)
            {
                // Preiei ViewModel-ul din DataContext
                var vm = createUserView.DataContext as CreateUserViewModel;
                if (vm != null && vm.NewUser != null)
                {
                    Users.Add(vm.NewUser);
                    SaveUsers();
                }
            }
        }



        private void SaveUsers()
        {
            // Resolved Formatting ambiguity by fully qualifying Newtonsoft.Json
            var json = JsonConvert.SerializeObject(Users, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(UsersFilePath, json);
        }

        private void DeleteUser(object parameter)
        {
            if (SelectedUser != null)
            {
                Users.Remove(SelectedUser);
                SelectedUser = null;
                SaveUsers();  // Added to persist changes
            }
        }

        private void LoadUsers()
        {
            if (File.Exists(UsersFilePath))
            {
                var json = File.ReadAllText(UsersFilePath);
                Users = JsonConvert.DeserializeObject<ObservableCollection<User>>(json);
            }
            else
            {
                Users = new ObservableCollection<User>
                {
                    new User { Name = "Alice", ImagePath = "Images/alice.png" },
                    new User { Name = "Bob", ImagePath = "Images/bob.png" }
                };
            }
        }

        private void PlayGame(object parameter)
        {
            if (SelectedUser != null)
            {
                var gameView = new GameView();
                // Create GameViewModel with parameterless constructor temporarily
                gameView.DataContext = new GameViewModel();
                gameView.Show();

                foreach (Window window in Application.Current.Windows)
                {
                    if (window is LoginView)
                    {
                        window.Close();
                        break;
                    }
                }
            }
        }

        private void CloseWindow(object parameter)
        {
            Application.Current.Shutdown();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}