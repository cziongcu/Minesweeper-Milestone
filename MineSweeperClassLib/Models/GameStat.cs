using System;

namespace MineSweeperClassLib.Models
{
    // Minimal comments to preserve user style
    public class GameStat : IComparable<GameStat>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public string GameTime { get; set; }
        public DateTime Date { get; set; }
        public double AverageActionTime { get; set; }

        public GameStat()
        {
        }

        public GameStat(int id, string name, int score, string gameTime, DateTime date, double avgActionTime = 0)
        {
            Id = id;
            Name = name;
            Score = score;
            GameTime = gameTime;
            Date = date;
            AverageActionTime = Math.Round(avgActionTime, 2);
        }

        public int CompareTo(GameStat other)
        {
            return other.Score.CompareTo(this.Score);
        }
    }
}
