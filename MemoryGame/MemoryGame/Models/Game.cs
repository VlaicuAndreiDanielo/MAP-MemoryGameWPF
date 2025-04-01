using System.Collections.Generic;

namespace MemoryGame.Models
{
    public class Game
    {
        public string Level { get; set; }  // "easy", "medium", "hard", "custom"
        public string Module { get; set; } // "flori", "animale", "memes", etc.
        public int TimeRemaining { get; set; }

        // Dacă jocul are un layout M x N
        public int Rows { get; set; }
        public int Columns { get; set; }

        // Lista cărților (poziție, path imagine, stare flipped etc.)
        public List<CardState> Cards { get; set; } = new List<CardState>();
    }

    public class CardState
    {
        public string ImagePath { get; set; }
        public bool IsFlipped { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }
}
