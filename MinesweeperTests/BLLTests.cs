using Xunit;
using MineSweeperClassLib.Models;
using MineSweeperClassLib.BLL;

namespace MinesweeperTests
{
    public class BLLTests
    {
        [Fact]
        public void SetupBombs_PlacesExpectedNumberOfBombs()
        {
            BoardModel board = new BoardModel(10);
            board.Difficulty = 10;
            BoardBLL bll = new BoardBLL();
            bll.SetupBombs(board);

            int bombCount = 0;
            for (int r = 0; r < 10; r++)
                for (int c = 0; c < 10; c++)
                    if (board.Grid[r, c].IsLive) bombCount++;

            Assert.Equal(10, bombCount);
        }

        [Fact]
        public void CountBombsNearby_CorrectlyCountsNeighbors()
        {
            BoardModel board = new BoardModel(3);
            BoardBLL bll = new BoardBLL();
            
            // Manual placement for deterministic testing
            board.Grid[0, 0].IsLive = true;
            board.Grid[1, 1].IsLive = true;
            
            bll.CountBombsNearby(board);

            // Center cell is live, neighbor count should be 9
            Assert.Equal(9, board.Grid[1, 1].NumberOfBombNeighbors);
            
            // (0,1) should see 2 bombs
            Assert.Equal(2, board.Grid[0, 1].NumberOfBombNeighbors);
        }
    }
}
