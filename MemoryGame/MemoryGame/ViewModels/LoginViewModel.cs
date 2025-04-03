using MemoryGame.Commands;
using MemoryGame.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Newtonsoft.Json;
using System.IO;  // Added for File operations
using System.Windows;
using MemoryGame.Views;
using MemoryGame.Services;

namespace MemoryGame.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private User _selectedUser;
        private string UsersFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");

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
                                                // Noile comenzi
        public ICommand HelpCommand { get; }
        public ICommand StatisticsCommand { get; }
        public ICommand ThemeCommand { get; }
        public LoginViewModel()
        {
            LoadUsers();

            CreateUserCommand = new RelayCommand(CreateUser);
            DeleteUserCommand = new RelayCommand(DeleteUser, CanModifyUser);
            PlayCommand = new RelayCommand(PlayGame, CanModifyUser);

            HelpCommand = new RelayCommand(ShowHelp);
            StatisticsCommand = new RelayCommand(OpenStatistics);
            CancelCommand = new RelayCommand(CloseWindow);  // Initialize CancelCommand
            ThemeCommand = new RelayCommand(SwitchTheme);
        }

        private bool CanModifyUser(object parameter)
        {
            return SelectedUser != null;
        }

        private void CreateUser(object parameter)
        {
            // Deschide fereastra de creare a user-ului
            var createUserView = new CreateUserView();
            bool? result = createUserView.ShowDialog();
            if (result == true)
            {
                // Preia ViewModel-ul din fereastră
                var vm = createUserView.DataContext as CreateUserViewModel;
                if (vm != null && vm.NewUser != null)
                {
                    // Încarcă lista de useri din JSON
                    var users = UserManager.LoadUsers();
                    // Adaugă noul user
                    users.Add(vm.NewUser);
                    // Salvează modificările
                    UserManager.SaveUsers(users);
                    MessageBox.Show("User created successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Dacă parametru este LoginViewModel, actualizează colecția de useri din UI
                    if (parameter is LoginViewModel loginVM)
                    {
                        var updatedUsers = UserManager.LoadUsers();
                        loginVM.Users.Clear();
                        foreach (var user in updatedUsers)
                        {
                            loginVM.Users.Add(user);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("User creation failed.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteUser(object parameter)
        {
            if (SelectedUser != null)
            {
                // Deschide fereastra de confirmare cu numele userului
                var confirmWindow = new ConfirmDeleteView(SelectedUser.Name);
                bool? result = confirmWindow.ShowDialog();

                if (result == true)
                {
                    var users = UserManager.LoadUsers();
                    if (users == null)
                    {
                        MessageBox.Show("Users is null!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    bool found = false;
                    // Parcurgem lista și ștergem userul care corespunde după nume
                    for (int i = 0; i < users.Count; i++)
                    {
                        if (users[i].Name == SelectedUser.Name)
                        {
                            users.RemoveAt(i);
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        // Salvează lista actualizată în JSON
                        UserManager.SaveUsers(users);
                        // Actualizează colecția locală pentru ca UI-ul să se reîmprospăteze
                        Users.Clear();
                        foreach (var user in users)
                        {
                            Users.Add(user);
                        }
                        SelectedUser = null;
                        MessageBox.Show("User deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("User not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("No user selected for deletion.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadUsers()
        {
            if (File.Exists(UsersFilePath))
            {
                var json = File.ReadAllText(UsersFilePath);
                Users = JsonConvert.DeserializeObject<ObservableCollection<User>>(json);
                if (Users == null)
                {
                    Users = new ObservableCollection<User>();
                }
            }
            else
            {
                Users = new ObservableCollection<User>();
            }
        }
        // Noua comandă Help – afișează un mesaj cu numele și grupa ta
        private void ShowHelp(object parameter)
        {
            MessageBox.Show("Your Name: Vlaicu Andrei Danielo\n" +
                            "Group: 10LF234", "Help", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void SwitchTheme(object parameter)
        {

            var pickerView = new ThemePickerView();
            pickerView.DataContext = new ThemePickerViewModel();
            pickerView.ShowDialog();
        }
        // Comandă pentru deschiderea ferestrei de Statistics
        private void OpenStatistics(object parameter)
        {
            var statsView = new StatisticsView();
            statsView.DataContext = new StatisticsViewModel();
            statsView.Show();
        }
        private void PlayGame(object parameter)
        {
            if (parameter is User user)
            {
                if (user.SavedGame != null)
                {
                    MessageBoxResult res = MessageBox.Show("A saved game exists. Do you want to continue it?",
                                                             "Continue Game",
                                                             MessageBoxButton.YesNo,
                                                             MessageBoxImage.Question);
                    if (res == MessageBoxResult.Yes)
                    {
                        var gameView = new GameView();
                        gameView.DataContext = new GameViewModel(user, user.SavedGame);
                        gameView.Show();
                    }
                    else
                    {
                        var setupView = new GameSetupView();
                        setupView.DataContext = new GameSetupViewModel(user);
                        setupView.Show();
                    }
                }
                else
                {
                    var setupView = new GameSetupView();
                    setupView.DataContext = new GameSetupViewModel(user);
                    setupView.Show();
                }
            }
            else
            {
                MessageBox.Show("Please select a user to play.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
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