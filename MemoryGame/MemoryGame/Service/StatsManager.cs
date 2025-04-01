using MemoryGame.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace MemoryGame.Services
{
    public static class StatsManager
    {
        private const string StatsFilePath = "stats.json";

        public static List<Stats> LoadStats()
        {
            if (File.Exists(StatsFilePath))
            {
                var json = File.ReadAllText(StatsFilePath);
                return JsonConvert.DeserializeObject<List<Stats>>(json);
            }
            else
            {
                return new List<Stats>();
            }
        }

        public static void SaveStats(List<Stats> statsList)
        {
            var json = JsonConvert.SerializeObject(statsList, Formatting.Indented);
            File.WriteAllText(StatsFilePath, json);
        }
    }
}
