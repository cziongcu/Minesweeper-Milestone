/*
 * Christopher Zion
 * CST-250
 * Milestone 4
 */
using System;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Forms;
using MineSweeperClassLib.Models;

namespace MinesweeperGUI
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            // Default values
            int difficulty = 10; // Easy
            int size = 10; // Small

            // Check difficulty
            if (rbMedium.Checked) difficulty = 20;
            else if (rbHard.Checked) difficulty = 30;

            // Check size
            if (rbSizeMedium.Checked) size = 15;
            else if (rbSizeLarge.Checked) size = 20;

            // Create board
            BoardModel board = new BoardModel(size);
            board.Difficulty = difficulty;
            board.HasSound = chkSound.Checked;
            board.ThemeName = cmbTheme.SelectedItem.ToString();

            // Hide this form and show game board
            Form1 gameForm = new Form1(board);
            gameForm.FormClosed += (s, args) => this.Close(); 
            this.Hide();
            gameForm.Show();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            string path = "savedgame.json";
            if (!File.Exists(path))
            {
                MessageBox.Show("No saved game found!");
                return;
            }

            try
            {
                string json = File.ReadAllText(path);
                BoardModel board = JsonConvert.DeserializeObject<BoardModel>(json);
                
                // Inherit current toggle preferences over what the save file had, just in case user changed themes/sound mid re-load.
                board.HasSound = chkSound.Checked;
                board.ThemeName = cmbTheme.SelectedItem.ToString();
                
                Form1 gameForm = new Form1(board);
                gameForm.FormClosed += (s, args) => this.Close(); 
                this.Hide();
                gameForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading game: " + ex.Message);
            }
        }
    }
}
