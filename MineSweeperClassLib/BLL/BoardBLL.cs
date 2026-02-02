/*
 * Christopher Zion
 * CST - 250
 * 01/30/2026
 * Milestone 3
 * Activity 3
 */
using System;
using MineSweeperClassLib.Models;

namespace MineSweeperClassLib.BLL
{
    public class BoardBLL
    {
        public void SetupBombs(BoardModel board)
        {
            Random rand = new Random();
            int bombsToPlace = (int)(board.Size * board.Size * (board.Difficulty / 100.0));
            for (int i = 0; i < bombsToPlace; i++)
            {
                int r = rand.Next(board.Size);
                int c = rand.Next(board.Size);
                if (!board.Grid[r, c].IsLive) board.Grid[r, c].IsLive = true;
                else i--;
            }

            // Place one special reward cell
            int rr = rand.Next(board.Size);
            int rc = rand.Next(board.Size);
            board.Grid[rr, rc].HasSpecialReward = true;
        }

        public void CountBombsNearby(BoardModel board)
        {
            for (int r = 0; r < board.Size; r++)
            {
                for (int c = 0; c < board.Size; c++)
                {
                    if (board.Grid[r, c].IsLive)
                    {
                        board.Grid[r, c].NumberOfBombNeighbors = 9;
                        continue;
                    }

                    int count = 0;
                    for (int dr = -1; dr <= 1; dr++)
                    {
                        for (int dc = -1; dc <= 1; dc++)
                        {
                            int nr = r + dr;
                            int nc = c + dc;
                            if (nr >= 0 && nr < board.Size && nc >= 0 && nc < board.Size)
                            {
                                if (board.Grid[nr, nc].IsLive) count++;
                            }
                        }
                    }
                    board.Grid[r, c].NumberOfBombNeighbors = count;
                }
            }
        }
        public GameState DetermineGameState(BoardModel board)
        {
            bool allSafeVisited = true;
            for (int r = 0; r < board.Size; r++)
            {
                for (int c = 0; c < board.Size; c++)
                {
                    CellModel cell = board.Grid[r, c];
                    if (cell.IsLive && cell.IsVisited) return GameState.Lost;
                    if (!cell.IsLive && !cell.IsVisited) allSafeVisited = false;
                }
            }
            return allSafeVisited ? GameState.Won : GameState.InProgress;
        }
        public void FloodFill(int row, int col, BoardModel board)
        {
            // Check bounds
            if (row < 0 || row >= board.Size || col < 0 || col >= board.Size)
                return;

            // Check if already visited to prevent infinite recursion
            if (board.Grid[row, col].IsVisited)
                return;

            // Mark as visited
            board.Grid[row, col].IsVisited = true;

            // If the cell has 0 bomb neighbors, recursively clear neighbors
            if (board.Grid[row, col].NumberOfBombNeighbors == 0)
            {
                // Loop through neighbors
                for (int dr = -1; dr <= 1; dr++)
                {
                    for (int dc = -1; dc <= 1; dc++)
                    {
                        if (dr == 0 && dc == 0) continue; // Skip self
                        FloodFill(row + dr, col + dc, board);
                    }
                }
            }
        }
    }
}
