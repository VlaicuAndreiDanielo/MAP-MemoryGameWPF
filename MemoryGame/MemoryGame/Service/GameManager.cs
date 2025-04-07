using MemoryGame.Models;
using MemoryGame.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace MemoryGame.Service
{
    public static class GameManager
    {
        public static void ClearSavedGame(User user)
        {
            user.SavedGame = null;
            var users = UserManager.LoadUsers();
            foreach (var u in users)
            {
                if (u.Name.Equals(user.Name, StringComparison.OrdinalIgnoreCase))
                {
                    u.SavedGame = null;
                    break;
                }
            }
            UserManager.SaveUsers(users);
            CommandManager.InvalidateRequerySuggested();
            MessageBox.Show("Saved game was deleted!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

}
