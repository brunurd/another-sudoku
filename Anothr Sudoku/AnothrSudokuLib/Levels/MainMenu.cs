using System.Linq;
using Godot;

namespace AnothrSudokuLib.Levels
{
    public partial class MainMenu : Node
    {
        private Label _label;

        public override void _Ready()
        {
            var gameManager = GameManager.Instance;
            var levelsStr = gameManager.ReadOnlyLevels.Select(level => level.ToString()).ToArray().Join("\n");

            _label = new Label()
            {
                Name = "TestLabel",
                Text = levelsStr,
                GrowHorizontal = Control.GrowDirection.Both,
                GrowVertical = Control.GrowDirection.Both,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            AddChild(_label);
        }
    }
}
