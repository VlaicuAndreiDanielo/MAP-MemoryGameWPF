using MemoryGame.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;

namespace MemoryGame.Services
{
    public static class UserManager
    {
        private const string UsersFilePath = "users.json";

        public static ObservableCollection<User> LoadUsers()
        {
            if (File.Exists(UsersFilePath))
            {
                var json = File.ReadAllText(UsersFilePath);
                return JsonConvert.DeserializeObject<ObservableCollection<User>>(json);
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
