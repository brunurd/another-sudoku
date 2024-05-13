using Godot;
using System;

namespace AnotherSudokuLib
{
    public partial class Init : Node2D
    {
        public override void _Ready()
        {
            GD.Print("Hello Another Sudoku!");
        }
    }
}