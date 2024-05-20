using Godot;
using AnothrSudokuLib.Components;
using AnothrSudokuLib.Data;

namespace AnothrSudokuLib.Levels
{
    public partial class EntryLevel : Node
    {
        private ListRequest _listRequest;

        public override void _Ready()
        {
            var listRequestResource = ResourceLoader.Load<PackedScene>(Constants.Resources.Components.ListRequest);
            var listRequest = listRequestResource.Instantiate() as ListRequest;
            listRequest.onLevelsLoaded += PersistReadOnlyLevelsData;
            AddChild(listRequest);
            _listRequest = listRequest;
        }

        private void PersistReadOnlyLevelsData(Level[] levels)
        {
            var gameManager = GetNode<GameManager>(Constants.Resources.AutoLoad.GameManager);
            gameManager.ReadOnlyLevels = levels;

            // Load next scene only after set the levels on gameManager.
            if (gameManager.ReadOnlyLevels != null)
            {
                var mainMenu = ResourceLoader.Load<PackedScene>(Constants.Resources.Levels.MainMenu);
                GetTree().ChangeSceneToPacked(mainMenu);
            }
        }
    }
}
