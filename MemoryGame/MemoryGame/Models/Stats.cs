namespace MemoryGame.Models
{
    public class Stats
    {
        public string UserName { get; set; }

        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }

        public int EasyGamesPlayed { get; set; }
        public int MediumGamesPlayed { get; set; }
        public int HardGamesPlayed { get; set; }
        public int CustomGamesPlayed { get; set; }

        public Dictionary<string, int> GamesPerModule { get; set; } = new Dictionary<string, int>();
    }
}
