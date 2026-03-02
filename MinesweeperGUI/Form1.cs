/*
 * Christopher Zion
 * CST-250
 * Milestone 4
 */
using MineSweeperClassLib.BLL;
using MineSweeperClassLib.Models;
using System;
using System.Drawing;
using System.IO;
using System.Media;
using Newtonsoft.Json;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

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

        private SoundPlayer sndClick;
        private SoundPlayer sndBomb;
        private SoundPlayer sndWin;
        private SoundPlayer sndLost;
        
        private DateTime lastActionTime;

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
            // Setup backend only if brand new game (Start Time wouldn't be populated if so)
            if (board.StartTime == default(DateTime))
            {
                bll.SetupBombs(board);
                bll.CountBombsNearby(board);
                board.StartTime = DateTime.Now;
            }
            lastActionTime = DateTime.Now;

            // Load Images based on Theme
            string imgPath = System.IO.Path.Combine(Application.StartupPath, "Assets", "Themes", "Classic");
            
            if (System.IO.Directory.Exists(System.IO.Path.Combine(Application.StartupPath, "Assets", "Themes", board.ThemeName)))
            {
                 imgPath = System.IO.Path.Combine(Application.StartupPath, "Assets", "Themes", board.ThemeName);
            }
            else if (board.ThemeName != "Classic")
            {
                 MessageBox.Show("Custom theme folder missing. Expecting: " + System.IO.Path.Combine(Application.StartupPath, "Assets", "Themes", board.ThemeName) + "\nFalling back to Classic.");
            }

            imgHidden = Image.FromFile(System.IO.Path.Combine(imgPath, "hidden.png"));
            imgQuestion = Image.FromFile(System.IO.Path.Combine(imgPath, "question.png"));
            imgFlag = Image.FromFile(System.IO.Path.Combine(imgPath, "flag.png"));
            imgBomb = Image.FromFile(System.IO.Path.Combine(imgPath, "bomb.png"));
            for (int i = 1; i <= 8; i++)
            {
                imgNum[i] = Image.FromFile(System.IO.Path.Combine(imgPath, $"{i}.png"));
            }

            // Load Sounds safely
            if (board.HasSound)
            {
                 try
                 {
                     string sndPath = System.IO.Path.Combine(Application.StartupPath, "Assets", "Sounds");
                     sndClick = new SoundPlayer(System.IO.Path.Combine(sndPath, "click.wav"));
                     sndBomb = new SoundPlayer(System.IO.Path.Combine(sndPath, "bomb.wav"));
                     sndWin = new SoundPlayer(System.IO.Path.Combine(sndPath, "win.wav"));
                     sndLost = new SoundPlayer(System.IO.Path.Combine(sndPath, "lost.wav"));
                 }
                 catch { /* Graceful fail if missing audio */ }
            }

            // Setup UI
            int buttonSize = 100;
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
            if (e.KeyCode == Keys.S)
            {
                try
                {
                    string json = JsonConvert.SerializeObject(board);
                    File.WriteAllText("savedgame.json", json);
                    MessageBox.Show("Game successfully saved! (You may safely close the window)");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Save failed: " + ex.Message);
                }
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
            TimeSpan actionDelta = DateTime.Now - lastActionTime;
            board.TotalActionTime += actionDelta.TotalSeconds;
            board.TotalActions++;
            lastActionTime = DateTime.Now;

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
                
                if (board.HasSound) sndClick?.Play();
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
                    Application.DoEvents(); // Force UI to draw bomb before freezing thread
                    
                    if (board.HasSound) 
                    {
                        sndBomb?.PlaySync(); // Let explosion rock safely
                        sndLost?.Play();     // Trigger failure theme track
                    }
                    
                    MessageBox.Show("Game Over! You hit a bomb.");
           

                        Form4 form4 = new Form4();
                        form4.ShowDialog();
                    this.Close();
                    
                    return;
                }
                
                // Safe click
                if (board.HasSound) sndClick?.Play();
                
                if (board.Grid[r, c].HasSpecialReward)
                {
                    board.Grid[r, c].HasSpecialReward = false;
                    MessageBox.Show("Secret hint found! A bomb has been revealed.");
                    
                    var unflaggedBombs = new System.Collections.Generic.List<CellModel>();
                    for (int i = 0; i < board.Size; i++)
                    {
                        for (int j = 0; j < board.Size; j++)
                        {
                            if (board.Grid[i, j].IsLive && !board.Grid[i, j].IsFlagged)
                                unflaggedBombs.Add(board.Grid[i, j]);
                        }
                    }

                    if (unflaggedBombs.Count > 0)
                    {
                        CellModel target = unflaggedBombs[new Random().Next(unflaggedBombs.Count)];
                        target.IsFlagged = true;
                        
                        object[] tgtData = (object[])buttons[target.Row, target.Column].Tag;
                        tgtData[2] = 1; // Sync flag state to visual marking index
                    }
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
                 
                 if (board.HasSound) sndWin?.Play();
                 
                 // score calc
                 TimeSpan timeTaken = board.EndTime - board.StartTime;
                 int score = Math.Max(0, (board.Size * board.Size * 10) - (int)timeTaken.TotalSeconds);

                 double avgActionTime = board.TotalActions > 0 ? board.TotalActionTime / board.TotalActions : 0;

                 Form3 form3 = new Form3(score);
                 if (form3.ShowDialog() == DialogResult.OK)
                 {
                     GameStat stat = new GameStat(
                         new Random().Next(1, 10000),
                         form3.WinnerName,
                         score,
                         timeTaken.ToString(@"hh\:mm\:ss"),
                         board.EndTime,
                         avgActionTime
                     );

                     Form4 form4 = new Form4(stat);
                     form4.ShowDialog();
                 }

                 this.Close();
             }
        }
    }
}
