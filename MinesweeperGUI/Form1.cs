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
        
        private Image imgHidden;
        private Image imgQuestion;
        private Image imgFlag;
        private Image imgBomb;
        private Image[] imgNum = new Image[9];

        private bool cheatMode = false;

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
            board.StartTime = DateTime.Now;

            // Load Images
            string path = System.IO.Path.Combine(Application.StartupPath, "Images");
            imgHidden = Image.FromFile(System.IO.Path.Combine(path, "hidden.png"));
            imgQuestion = Image.FromFile(System.IO.Path.Combine(path, "question.png"));
            imgFlag = Image.FromFile(System.IO.Path.Combine(path, "flag.png"));
            imgBomb = Image.FromFile(System.IO.Path.Combine(path, "bomb.png"));
            for (int i = 1; i <= 8; i++)
            {
                imgNum[i] = Image.FromFile(System.IO.Path.Combine(path, $"{i}.png"));
            }

            // Setup UI
            int buttonSize = 35;
            this.ClientSize = new Size(board.Size * buttonSize, board.Size * buttonSize);
            
            buttons = new Button[board.Size, board.Size];
            panel1.Location = new Point(0, 0);
            panel1.Size = this.ClientSize;
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Resize += Panel1_Resize;

            for (int r = 0; r < board.Size; r++)
            {
                for (int c = 0; c < board.Size; c++)
                {
                    Button btn = new Button();
                    btn.Width = buttonSize;
                    btn.Height = buttonSize;
                    btn.Location = new Point(c * buttonSize, r * buttonSize);
                    
                    // We use an object array to store coordinates and a state int (0=none, 1=flag, 2=question)
                    btn.Tag = new object[] { r, c, 0 };
                    
                    btn.MouseDown += Btn_MouseDown;
                    panel1.Controls.Add(btn);
                    buttons[r, c] = btn;
                }
            }

            // Immediately render the unvisited states so hidden slots appear 
            UpdateBoard();

            // Enable form to capture key strokes before controls
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
            {
                cheatMode = !cheatMode;
                UpdateBoard();
            }
        }

        private void Panel1_Resize(object sender, EventArgs e)
        {
            if (buttons == null) return;
            int buttonWidth = panel1.Width / board.Size;
            int buttonHeight = panel1.Height / board.Size;
            for (int r = 0; r < board.Size; r++)
            {
                for (int c = 0; c < board.Size; c++)
                {
                    Button btn = buttons[r, c];
                    btn.Width = buttonWidth;
                    btn.Height = buttonHeight;
                    btn.Location = new Point(c * buttonWidth, r * buttonHeight);
                }
            }
        }

        private void Btn_MouseDown(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            object[] tagData = (object[])btn.Tag;
            int r = (int)tagData[0];
            int c = (int)tagData[1];
            int markingState = (int)tagData[2];

            if (e.Button == MouseButtons.Right)
            {
                // Right click to cycle: 0 -> 1 (Flag), 1 -> 2 (Question), 2 -> 0 (None)
                markingState = (markingState + 1) % 3;
                tagData[2] = markingState;

                // Sync the actual Board model if it's a flag, so win logic works correctly
                board.Grid[r, c].IsFlagged = (markingState == 1);
            }
            else if (e.Button == MouseButtons.Left)
            {
                // Left click to visit; deny if flagged or question 
                if (markingState == 1 || markingState == 2) return;

                // Hit a bomb?
                if (board.Grid[r, c].IsLive)
                {
                    board.Grid[r, c].IsVisited = true;
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
                else 
                {
                    board.Grid[r, c].IsVisited = true;
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

                    if (cell.IsVisited || cheatMode)
                    {
                        btn.BackColor = Color.LightGray;
                        btn.FlatStyle = FlatStyle.Flat;
                        btn.FlatAppearance.BorderColor = Color.DarkGray;
                        btn.BackgroundImageLayout = ImageLayout.Stretch;
                        
                        if (cell.IsLive)
                        {
                            btn.BackgroundImage = imgBomb;
                        }
                        else
                        {
                            if (cell.NumberOfBombNeighbors > 0)
                            {
                                btn.BackgroundImage = imgNum[cell.NumberOfBombNeighbors];
                            }
                            else
                            {
                                btn.BackgroundImage = null;
                            }
                        }
                    }
                    else
                    {
                        // Unvisited
                        btn.BackColor = SystemColors.ButtonFace;
                        btn.FlatStyle = FlatStyle.Standard;
                        btn.BackgroundImageLayout = ImageLayout.Stretch;
                        
                        object[] tagData = (object[])btn.Tag;
                        int markingState = (int)tagData[2];

                        if (markingState == 1)      // Flag
                        {
                            btn.BackgroundImage = imgFlag;
                        }
                        else if (markingState == 2) // Question
                        {
                            btn.BackgroundImage = imgQuestion;
                        }
                        else                        // Normal hidden
                        {
                            btn.BackgroundImage = imgHidden;
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
                 board.EndTime = DateTime.Now;
                 
                 // score calc
                 TimeSpan timeTaken = board.EndTime - board.StartTime;
                 int score = Math.Max(0, (board.Size * board.Size * 10) - (int)timeTaken.TotalSeconds);

                 Form3 form3 = new Form3(score);
                 if (form3.ShowDialog() == DialogResult.OK)
                 {
                     GameStat stat = new GameStat(
                         new Random().Next(1, 10000),
                         form3.WinnerName,
                         score,
                         timeTaken.ToString(@"hh\:mm\:ss"),
                         board.EndTime
                     );

                     Form4 form4 = new Form4(stat);
                     form4.ShowDialog();
                 }

                 this.Close();
             }
        }
    }
}
