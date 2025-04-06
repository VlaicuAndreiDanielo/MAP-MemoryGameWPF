using System.Collections.Generic;

namespace MemoryGame.Models
{
    public class Game
    {
        public string Level { get; set; }  
        public string Module { get; set; }
        public int TimeRemaining { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<CardState> Cards { get; set; } = new List<CardState>();

        public bool IsSaved { get; set; } 

    }

    public class CardState
    {
        public string ImagePath { get; set; }
        public bool IsFlipped { get; set; }
        public bool IsMatched { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }

}
