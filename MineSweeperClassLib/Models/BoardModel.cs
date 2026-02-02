/*
 * Christopher Zion
 * CST - 250
 * 01/30/2026
 * Milestone 3
 * Activity 3
 */
using System;

namespace MineSweeperClassLib.Models
{
    public class BoardModel
    {
        public int Size { get; set; }
        public CellModel[,] Grid { get; set; }
        public int Difficulty { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int RewardsRemaining { get; set; } = 0;
        public GameState GameState { get; set; } = GameState.InProgress;

        public BoardModel(int size)
        {
            Size = size;
            Grid = new CellModel[Size, Size];
            
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    Grid[r, c] = new CellModel(r, c);
                }
            }
        }
    }
}
