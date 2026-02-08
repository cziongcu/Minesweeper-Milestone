/*
 * Christopher Zion
 * CST-250
 * Milestone 4
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using MineSweeperClassLib.BLL;
using MineSweeperClassLib.Models;

namespace MinesweeperGUI
{
    public partial class Form1 : Form
    {
        private BoardModel board;
        private BoardBLL bll = new BoardBLL();
        private Button[,] buttons;

        public Form1(BoardModel boardModel)
        {
            InitializeComponent();
            this.board = boardModel;
            InitializeGame();
        }

        public Form1()
        {
            InitializeComponent();
             // Default for designer or empty init
             board = new BoardModel(10); 
             InitializeGame();
        }

        private void InitializeGame()
        {
            // Setup backend
            bll.SetupBombs(board);
            bll.CountBombsNearby(board);

            // Setup UI
            int buttonSize = 35;
            this.Width = board.Size * buttonSize + 40;
            this.Height = board.Size * buttonSize + 60;
            
            buttons = new Button[board.Size, board.Size];
            panel1.Width = board.Size * buttonSize;
            panel1.Height = board.Size * buttonSize;

            for (int r = 0; r < board.Size; r++)
            {
                for (int c = 0; c < board.Size; c++)
                {
                    Button btn = new Button();
                    btn.Width = buttonSize;
                    btn.Height = buttonSize;
                    btn.Location = new Point(c * buttonSize, r * buttonSize);
                    btn.Tag = new Point(r, c);
                    btn.MouseDown += Btn_MouseDown;
                    panel1.Controls.Add(btn);
                    buttons[r, c] = btn;
                }
            }
        }

        private void Btn_MouseDown(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            Point p = (Point)btn.Tag;
            int r = p.X;
            int c = p.Y;

            if (e.Button == MouseButtons.Right)
            {
                // Right click to flag
                board.Grid[r, c].IsFlagged = !board.Grid[r, c].IsFlagged;
            }
            else if (e.Button == MouseButtons.Left)
            {
                // Left click to visit
                if (board.Grid[r, c].IsFlagged) return;

                board.Grid[r, c].IsVisited = true;

                // Hit a bomb?
                if (board.Grid[r, c].IsLive)
                {
                    UpdateBoard();
                    MessageBox.Show("Game Over! You hit a bomb.");
                    this.Close();
                    return;
                }
                
                // Flood fill
                if (board.Grid[r, c].NumberOfBombNeighbors == 0)
                {
                    bll.FloodFill(r, c, board);
                }
            }

            UpdateBoard();
            CheckGameState();
        }

        private void UpdateBoard()
        {
            for (int r = 0; r < board.Size; r++)
            {
                for (int c = 0; c < board.Size; c++)
                {
                    Button btn = buttons[r, c];
                    CellModel cell = board.Grid[r, c];

                    if (cell.IsVisited)
                    {
                        btn.BackColor = Color.White;
                        if (cell.IsLive)
                        {
                            btn.Text = "B";
                            btn.ForeColor = Color.Red;
                        }
                        else
                        {
                            if (cell.NumberOfBombNeighbors > 0)
                            {
                                btn.Text = cell.NumberOfBombNeighbors.ToString();
                                
                                // Colors like the guide
                                switch(cell.NumberOfBombNeighbors)
                                {
                                    case 1: btn.ForeColor = Color.Blue; break;
                                    case 2: btn.ForeColor = Color.Green; break;
                                    case 3: btn.ForeColor = Color.Red; break;
                                    default: btn.ForeColor = Color.DarkRed; break;
                                } 
                            }
                            else
                            {
                                btn.Text = "";
                            }
                        }
                    }
                    else
                    {
                        // Unvisited
                        btn.BackColor = SystemColors.ButtonFace;
                        if (cell.IsFlagged)
                        {
                            btn.Text = "F";
                            btn.ForeColor = Color.OrangeRed;
                        }
                        else
                        {
                            btn.Text = "";
                        }
                    }
                }
            }
        }

        private void CheckGameState()
        {
             if (bll.DetermineGameState(board) == GameState.Won)
             {
                 UpdateBoard();
                 MessageBox.Show("Congratulations! You Won!");
                 this.Close();
             }
        }
    }
}
