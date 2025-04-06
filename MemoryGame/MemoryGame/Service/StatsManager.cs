using MemoryGame.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace MemoryGame.Services
{
    public static class StatsManager
    {
        private static string StatsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "stats.json");

        public static void UpdateStats(User user, Game game, bool win)
        {
            var stats = LoadStats();
            var userStats = stats.Find(s => s.UserName.Equals(user.Name, StringComparison.OrdinalIgnoreCase));
            if (userStats == null)
            {
                userStats = new UserStats { UserName = user.Name };
                stats.Add(userStats);
            }

            userStats.TotalGames++;

            if (win)
            {
                userStats.Wins++;
                if (!userStats.Difficulties.ContainsKey(game.Level))
                    userStats.Difficulties[game.Level] = new OutcomeStats();
                userStats.Difficulties[game.Level].Wins++;

                if (!userStats.Categories.ContainsKey(game.Module))
                    userStats.Categories[game.Module] = new OutcomeStats();
                userStats.Categories[game.Module].Wins++;
            }
            else
            {
                userStats.Losses++;
                if (!userStats.Difficulties.ContainsKey(game.Level))
                    userStats.Difficulties[game.Level] = new OutcomeStats();
                userStats.Difficulties[game.Level].Losses++;

                if (!userStats.Categories.ContainsKey(game.Module))
                    userStats.Categories[game.Module] = new OutcomeStats();
                userStats.Categories[game.Module].Losses++;
            }

            SaveStats(stats);
        }

        public static List<UserStats> LoadStats()
        {
            if (File.Exists(StatsFilePath))
            {
                var json = File.ReadAllText(StatsFilePath);
                return JsonConvert.DeserializeObject<List<UserStats>>(json) ?? new List<UserStats>();
            }
            return new List<UserStats>();
        }

        private static void SaveStats(List<UserStats> stats)
        {
            var json = JsonConvert.SerializeObject(stats, Formatting.Indented);
            File.WriteAllText(StatsFilePath, json);
        }
    }

    public class UserStats
    {
        public string UserName { get; set; }
        public int TotalGames { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public Dictionary<string, OutcomeStats> Difficulties { get; set; } = new Dictionary<string, OutcomeStats>();
        public Dictionary<string, OutcomeStats> Categories { get; set; } = new Dictionary<string, OutcomeStats>();
    }

    public class OutcomeStats
    {
        public int Wins { get; set; }
        public int Losses { get; set; }
    }
}
