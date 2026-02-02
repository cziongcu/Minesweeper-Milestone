/*
 * Christopher Zion
 * CST - 250
 * 01/30/2026
 * Milestone 3
 * Activity 3
 */
using System;
using MineSweeperClassLib.Models;
using MineSweeperClassLib.BLL;

namespace MinesweeperConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            BoardBLL bll = new BoardBLL();
            Console.WriteLine("Welcome to Minesweeper!");
            Console.Write("Enter board size (e.g., 10): ");
            int size = int.Parse(Console.ReadLine() ?? "10");
            
            BoardModel board = new BoardModel(size);
            board.Difficulty = 15; // default 15%
            bll.SetupBombs(board);
            bll.CountBombsNearby(board);

            bool gameOver = false;
            while (!gameOver)
            {
                PrintBoard(board);
                Console.WriteLine("\nOptions: 1. Visit  2. Flag  3. Use Reward  4. Show Answers (Cheat)");
                Console.Write("Choose action and Row/Col (e.g., 1 5 5): ");
                string[] input = (Console.ReadLine() ?? "").Split(' ');
                
                if (input.Length < 3 && input[0] != "4") continue;

                int action = int.Parse(input[0]);
                if (action == 4) { PrintAnswers(board); continue; }

                int r = int.Parse(input[1]);
                int c = int.Parse(input[2]);

                if (r < 0 || r >= size || c < 0 || c >= size) continue;

                if (action == 1)
                {
                    bll.FloodFill(r, c, board);
                    if (board.Grid[r, c].HasSpecialReward)
                    {
                        Console.WriteLine("REWARD FOUND! You can now peek at any cell.");
                        board.RewardsRemaining++;
                    }
                }
                else if (action == 2)
                {
                    board.Grid[r, c].IsFlagged = !board.Grid[r, c].IsFlagged;
                }
                else if (action == 3 && board.RewardsRemaining > 0)
                {
                    Console.Write("Enter Row/Col to peek at (e.g., 2 3): ");
                    string[] peek = (Console.ReadLine() ?? "").Split(' ');
                    int pr = int.Parse(peek[0]);
                    int pc = int.Parse(peek[1]);
                    Console.WriteLine(board.Grid[pr, pc].IsLive ? "BOMB!" : "Safe.");
                    board.RewardsRemaining--;
                }

                GameState state = bll.DetermineGameState(board);
                if (state == GameState.Lost)
                {
                    PrintAnswers(board);
                    Console.WriteLine("BOOM! Game Over.");
                    gameOver = true;
                }
                else if (state == GameState.Won)
                {
                    PrintAnswers(board);
                    Console.WriteLine("Victory! You cleared the field.");
                    gameOver = true;
                }
            }
        }

        static void PrintBoard(BoardModel board)
        {
            Console.Write("    ");
            for (int c = 0; c < board.Size; c++) Console.Write($"{c:D2} ");
            Console.WriteLine("\n   " + new string('-', board.Size * 3 + 1));

            for (int r = 0; r < board.Size; r++)
            {
                Console.Write($"{r:D2} |");
                for (int c = 0; c < board.Size; c++)
                {
                    CellModel cell = board.Grid[r, c];
                    if (cell.IsVisited)
                    {
                        if (cell.IsLive) { Console.ForegroundColor = ConsoleColor.Red; Console.Write(" B "); }
                        else if (cell.NumberOfBombNeighbors > 0) { Console.ForegroundColor = ConsoleColor.Blue; Console.Write($" {cell.NumberOfBombNeighbors} "); }
                        else { Console.ForegroundColor = ConsoleColor.Gray; Console.Write(" . "); }
                    }
                    else if (cell.IsFlagged)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write(" F ");
                    }
                    else if (cell.HasSpecialReward && cell.IsVisited) // logic should match PrintBoard requirement
                    {
                         Console.Write(" r ");
                    }
                    else
                    {
                        Console.Write(" ? ");
                    }
                    Console.ResetColor();
                }
                Console.WriteLine("|");
            }
        }

        static void PrintAnswers(BoardModel board)
        {
            Console.WriteLine("\n--- ANSWER KEY ---");
            for (int r = 0; r < board.Size; r++)
            {
                for (int c = 0; c < board.Size; c++)
                {
                    CellModel cell = board.Grid[r, c];
                    if (cell.IsLive) Console.Write(" B ");
                    else Console.Write($" {cell.NumberOfBombNeighbors} ");
                }
                Console.WriteLine();
            }
        }
    }
}
