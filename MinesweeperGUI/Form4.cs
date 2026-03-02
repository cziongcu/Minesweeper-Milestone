using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MineSweeperClassLib.Models;
using ScottPlot;
using ScottPlot.WinForms;

namespace MinesweeperGUI
{
    public class Form4 : Form
    {
        private DataGridView dgvScores;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileMenuItem;
        private ToolStripMenuItem saveMenuItem;
        private ToolStripMenuItem loadMenuItem;
        private ToolStripMenuItem exitMenuItem;

        private ToolStripMenuItem sortMenuItem;
        private ToolStripMenuItem byNameMenuItem;
        private ToolStripMenuItem byScoreMenuItem;
        private ToolStripMenuItem byDateMenuItem;
        
        private System.Windows.Forms.Label lblAverageScore;
        private System.Windows.Forms.Label lblAverageTime;
        private System.Windows.Forms.Label lblAverageActionTime;
        private FormsPlot formsPlot1;
        
        // Use BindingList to update UI automatically
        private BindingList<GameStat> statsList;
        private string filePath = "highscores.txt";


        public Form4()
        {
            this.Text = "High Scores";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            statsList = new BindingList<GameStat>();

            try
            {
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (var line in lines)
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length >= 5)
                        {
                            double avgTime = 0;
                            if (parts.Length >= 6) double.TryParse(parts[5], out avgTime);

                            statsList.Add(new GameStat(
                                int.Parse(parts[0]),
                                parts[1],
                                int.Parse(parts[2]),
                                parts[3],
                                DateTime.TryParse(parts[4], out var parsedDate) ? parsedDate : DateTime.Now,
                                avgTime
                            ));
                        }
                    }
                }
            }
            catch
            {
                // Fallback silently if bad disk reading
            }


            InitializeComponent();
        
        }

        public Form4(GameStat newStat)
        {
            this.Text = "High Scores";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            statsList = new BindingList<GameStat>();

            try
            {
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (var line in lines)
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length >= 5)
                        {
                            double avgTime = 0;
                            if (parts.Length >= 6) double.TryParse(parts[5], out avgTime);

                            statsList.Add(new GameStat(
                                int.Parse(parts[0]),
                                parts[1],
                                int.Parse(parts[2]),
                                parts[3],
                                DateTime.TryParse(parts[4], out var parsedDate) ? parsedDate : DateTime.Now,
                                avgTime
                            ));
                        }
                    }
                }
            }
            catch 
            {
                // Fallback silently if bad disk reading
            }
            
            if (newStat != null)
            {
                statsList.Add(newStat);
            }

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            dgvScores = new DataGridView();
            dgvScores.Dock = DockStyle.Fill;
            dgvScores.AutoGenerateColumns = true;
            dgvScores.AllowUserToAddRows = false;
            dgvScores.ReadOnly = true;
            dgvScores.DataSource = statsList;
            
            menuStrip1 = new MenuStrip();
            
            // File Menu
            fileMenuItem = new ToolStripMenuItem("File");
            saveMenuItem = new ToolStripMenuItem("Save");
            saveMenuItem.Click += SaveMenuItem_Click;
            loadMenuItem = new ToolStripMenuItem("Load");
            loadMenuItem.Click += LoadMenuItem_Click;
            exitMenuItem = new ToolStripMenuItem("Exit");
            exitMenuItem.Click += ExitMenuItem_Click;
            
            fileMenuItem.DropDownItems.Add(saveMenuItem);
            fileMenuItem.DropDownItems.Add(loadMenuItem);
            fileMenuItem.DropDownItems.Add(new ToolStripSeparator());
            fileMenuItem.DropDownItems.Add(exitMenuItem);

            // Sort Menu
            sortMenuItem = new ToolStripMenuItem("Sort");
            byNameMenuItem = new ToolStripMenuItem("By Name");
            byNameMenuItem.Click += ByNameMenuItem_Click;
            byScoreMenuItem = new ToolStripMenuItem("By Score");
            byScoreMenuItem.Click += ByScoreMenuItem_Click;
            byDateMenuItem = new ToolStripMenuItem("By Date");
            byDateMenuItem.Click += ByDateMenuItem_Click;

            sortMenuItem.DropDownItems.Add(byNameMenuItem);
            sortMenuItem.DropDownItems.Add(byScoreMenuItem);
            sortMenuItem.DropDownItems.Add(byDateMenuItem);

            menuStrip1.Items.Add(fileMenuItem);
            menuStrip1.Items.Add(sortMenuItem);

            TableLayoutPanel tlp = new TableLayoutPanel();
            tlp.Dock = DockStyle.Fill;
            tlp.RowCount = 2;
            tlp.ColumnCount = 2;
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            
            tlp.Controls.Add(dgvScores, 0, 0);
            tlp.SetColumnSpan(dgvScores, 2);

            Panel pnlStats = new Panel();
            pnlStats.Dock = DockStyle.Fill;

            lblAverageScore = new System.Windows.Forms.Label();
            lblAverageScore.AutoSize = true;
            lblAverageScore.Location = new Point(10, 10);
            lblAverageScore.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            lblAverageScore.Text = "Average Score: --";

            lblAverageTime = new System.Windows.Forms.Label();
            lblAverageTime.AutoSize = true;
            lblAverageTime.Location = new Point(10, 35);
            lblAverageTime.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            lblAverageTime.Text = "Average Time: --";

            lblAverageActionTime = new System.Windows.Forms.Label();
            lblAverageActionTime.AutoSize = true;
            lblAverageActionTime.Location = new Point(10, 60);
            lblAverageActionTime.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            lblAverageActionTime.Text = "Avg Action Time: --";

            pnlStats.Controls.Add(lblAverageScore);
            pnlStats.Controls.Add(lblAverageTime);
            pnlStats.Controls.Add(lblAverageActionTime);
            
            tlp.Controls.Add(pnlStats, 0, 1);

            formsPlot1 = new FormsPlot();
            formsPlot1.Dock = DockStyle.Fill;
            tlp.Controls.Add(formsPlot1, 1, 1);

            this.Controls.Add(tlp);
            this.Controls.Add(menuStrip1);
            this.MainMenuStrip = menuStrip1;

            CalculateAverages();
            UpdateGraph();
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    foreach (var stat in statsList)
                    {
                        sw.WriteLine($"{stat.Id},{stat.Name},{stat.Score},{stat.GameTime},{stat.Date},{stat.AverageActionTime}");
                    }
                }
                MessageBox.Show("Scores saved successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving scores: {ex.Message}");
            }
        }

        private void LoadMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    statsList.Clear();
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (var line in lines)
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length >= 5)
                        {
                            double avgTime = 0;
                            if (parts.Length >= 6) double.TryParse(parts[5], out avgTime);

                            statsList.Add(new GameStat(
                                int.Parse(parts[0]),
                                parts[1],
                                int.Parse(parts[2]),
                                parts[3],
                                DateTime.TryParse(parts[4], out var parsedDate) ? parsedDate : DateTime.Now,
                                avgTime
                            ));
                        }
                    }
                    CalculateAverages();
                    UpdateGraph();
                }
                else
                {
                    MessageBox.Show("No saved scores found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading scores: {ex.Message}");
            }
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ByNameMenuItem_Click(object sender, EventArgs e)
        {
            var sorted = statsList.OrderBy(s => s.Name).ToList();
            statsList.Clear();
            foreach (var s in sorted) statsList.Add(s);
            CalculateAverages();
            UpdateGraph();
        }

        private void ByScoreMenuItem_Click(object sender, EventArgs e)
        {
            var sorted = statsList.OrderByDescending(s => s.Score).ToList();
            statsList.Clear();
            foreach (var s in sorted) statsList.Add(s);
            CalculateAverages();
            UpdateGraph();
        }

        private void ByDateMenuItem_Click(object sender, EventArgs e)
        {
            var sorted = statsList.OrderByDescending(s => s.Date).ToList();
            statsList.Clear();
            foreach (var s in sorted) statsList.Add(s);
            CalculateAverages();
            UpdateGraph();
        }

        private void CalculateAverages()
        {
            if (statsList.Count > 0)
            {
                double avgScore = statsList.Average(s => s.Score);
                double avgSeconds = statsList.Average(s => TimeSpan.Parse(s.GameTime).TotalSeconds);
                double avgActTime = statsList.Average(s => s.AverageActionTime);
                TimeSpan avgSpan = TimeSpan.FromSeconds(avgSeconds);

                lblAverageScore.Text = $"Average Score: {Math.Round(avgScore, 2)}";
                lblAverageTime.Text = $"Average Time: {avgSpan.ToString(@"hh\:mm\:ss")}";
                lblAverageActionTime.Text = $"Avg Action Time: {Math.Round(avgActTime, 2)}s";
            }
            else
            {
                lblAverageScore.Text = "Average Score: N/A";
                lblAverageTime.Text = "Average Time: N/A";
                lblAverageActionTime.Text = "Avg Action Time: N/A";
            }
        }

        private void UpdateGraph()
        {
            formsPlot1.Plot.Clear();
            if (statsList.Count < 2)
            {
                formsPlot1.Refresh();
                return;
            }

            var sorted = statsList.OrderBy(s => s.Date).ToList();
            double[] xs = new double[sorted.Count];
            double[] ys = new double[sorted.Count];

            for (int i = 0; i < sorted.Count; i++)
            {
                xs[i] = i + 1; // 1-based match index
                ys[i] = TimeSpan.Parse(sorted[i].GameTime).TotalSeconds;
            }

            var scatter = formsPlot1.Plot.Add.Scatter(xs, ys);
            scatter.LineWidth = 3;
            scatter.MarkerSize = 8;
            scatter.Color = ScottPlot.Color.FromHex("#007ACC");

            formsPlot1.Plot.Title("Win History");
            formsPlot1.Plot.Axes.Left.Label.Text = "Seconds";
            formsPlot1.Plot.Axes.Bottom.Label.Text = "Matches";
            
            formsPlot1.Refresh();
        }
    }
}
