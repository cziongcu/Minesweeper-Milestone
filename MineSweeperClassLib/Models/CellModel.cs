/*
 * Christopher Zion
 * CST - 250
 * 01/30/2026
 * Milestone 3
 * Activity 3
 */
namespace MineSweeperClassLib.Models
{
    public class CellModel
    {
        public int Row { get; set; } = -1;
        public int Column { get; set; } = -1;
        public bool IsVisited { get; set; } = false;
        public bool IsLive { get; set; } = false;
        public bool IsFlagged { get; set; } = false;
        public int NumberOfBombNeighbors { get; set; } = 0;
        public bool HasSpecialReward { get; set; } = false;

        public CellModel() { }

        public CellModel(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}
