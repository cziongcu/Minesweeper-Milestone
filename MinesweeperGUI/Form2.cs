/*
 * Christopher Zion
 * CST-250
 * Milestone 4
 */
using System;
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

            // Hide this form and show game board
            Form1 gameForm = new Form1(board);
            gameForm.FormClosed += (s, args) => this.Close(); 
            this.Hide();
            gameForm.Show();
        }
    }
}
