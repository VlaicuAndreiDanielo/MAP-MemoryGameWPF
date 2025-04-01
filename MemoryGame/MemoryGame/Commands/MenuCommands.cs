using System;
using System.Windows;
using System.Windows.Input;
using MemoryGame.Views;
using MemoryGame.ViewModels;
using MemoryGame.Models;
using MemoryGame.Services;

namespace MemoryGame.Commands
{
    public static class MenuCommands
    {
        public static ICommand NewUserCommand { get; } = new RelayCommand(ExecuteNewUser);
        public static ICommand DeleteUserCommand { get; } = new RelayCommand(ExecuteDeleteUser);
        public static ICommand PlayCommand { get; } = new RelayCommand(ExecutePlay);
        public static ICommand CancelCommand { get; } = new RelayCommand(ExecuteCancel);

        private static void ExecuteNewUser(object parameter)
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

        private static void ExecuteDeleteUser(object parameter)
        {
            if (parameter is User user)
            {
                var users = UserManager.LoadUsers();
                if (users.Contains(user))
                {
                    users.Remove(user);
                    UserManager.SaveUsers(users);
                    MessageBox.Show("User deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("User not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("No user selected for deletion.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private static void ExecutePlay(object parameter)
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

        private static void ExecuteCancel(object parameter)
        {
            Application.Current.Shutdown();
        }
    }
}
