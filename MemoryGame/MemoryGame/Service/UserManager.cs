using MemoryGame.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace MemoryGame.Services
{
    public static class UserManager
    {
        private static string UsersFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");

        public static ObservableCollection<User> LoadUsers()
        {
            if (File.Exists(UsersFilePath))
            {
                try
                {
                    var json = File.ReadAllText(UsersFilePath);
                    var list = JsonConvert.DeserializeObject<ObservableCollection<User>>(json);
                    if (list == null)
                    {
                        MessageBox.Show("Deserialization returned null. Check JSON format.");
                        return new ObservableCollection<User>();
                    }
                    return list;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading users.json: " + ex.Message);
                    return new ObservableCollection<User>();
                }
            }
            else
            {
                return new ObservableCollection<User>();
            }
        }


        public static void SaveUsers(ObservableCollection<User> users)
        {
            var json = JsonConvert.SerializeObject(users, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(UsersFilePath, json);
        }
    }
}
