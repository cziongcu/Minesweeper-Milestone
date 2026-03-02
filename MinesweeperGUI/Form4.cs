using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MineSweeperClassLib.Models;

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
        
        // Use BindingList to update UI automatically
        private BindingList<GameStat> statsList;
        private string filePath = "highscores.txt";

        public Form4(GameStat newStat)
        {
            this.Text = "High Scores";
            this.Size = new Size(600, 400);
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
                        if (parts.Length == 5)
                        {
                            statsList.Add(new GameStat(
                                int.Parse(parts[0]),
                                parts[1],
                                int.Parse(parts[2]),
                                parts[3],
                                DateTime.Parse(parts[4])
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

            this.Controls.Add(dgvScores);
            this.Controls.Add(menuStrip1);
            this.MainMenuStrip = menuStrip1;
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    foreach (var stat in statsList)
                    {
                        sw.WriteLine($"{stat.Id},{stat.Name},{stat.Score},{stat.GameTime},{stat.Date}");
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
                        if (parts.Length == 5)
                        {
                            statsList.Add(new GameStat(
                                int.Parse(parts[0]),
                                parts[1],
                                int.Parse(parts[2]),
                                parts[3],
                                DateTime.Parse(parts[4])
                            ));
                        }
                    }
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
        }

        private void ByScoreMenuItem_Click(object sender, EventArgs e)
        {
            var sorted = statsList.OrderByDescending(s => s.Score).ToList();
            statsList.Clear();
            foreach (var s in sorted) statsList.Add(s);
        }

        private void ByDateMenuItem_Click(object sender, EventArgs e)
        {
            var sorted = statsList.OrderByDescending(s => s.Date).ToList();
            statsList.Clear();
            foreach (var s in sorted) statsList.Add(s);
        }
    }
}
