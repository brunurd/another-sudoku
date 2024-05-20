using Godot;
using AnothrSudokuLib.Data;

namespace AnothrSudokuLib
{
    public partial class GameManager : Node
    {
        private Level[] _readOnlyLevels = null;
        public static GameManager Instance = null;

        public override void _Ready()
        {
            Instance = GetNode<GameManager>(Constants.Resources.AutoLoad.GameManager);
        }

        public Level[] ReadOnlyLevels
        {
            get
            {
                if (_readOnlyLevels == null)
                {
                    throw new UnloadedException("ReadOnlyLevels");
                }
                return _readOnlyLevels;
            }
            set
            {
                _readOnlyLevels = value;
            }
        }
    }
}
