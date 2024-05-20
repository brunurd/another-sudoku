namespace AnothrSudokuLib
{
    public static class Constants
    {
        public static class Resources
        {

            public static class AutoLoad
            {
                public const string GameManager = "/root/GameManager";
            }

            public static class Levels
            {
                public const string EntryLevel = "res://Scenes/Levels/EntryLevel.tscn";
                public const string MainMenu = "res://Scenes/Levels/MainMenu.tscn";
            }

            public static class Components
            {
                public const string LevelRequest = "res://Scenes/Components/LevelRequest.tscn";
                public const string ListRequest = "res://Scenes/Components/ListRequest.tscn";
            }
        }

        public static class Url
        {
            public const string LevelList = "https://lavaleak.github.io/anothr-sudoku/index.json";
        }

        public static class Persistent
        {
            public const string LevelList = "res://Data/index.json";
            public const string LevelsDir = "res://Data/levels";
            public const string DataCache = "user://Data";
            public const string LevelListCache = "user://Data/index.json";
            public const string LevelsDirCache = "user://Data/levels";
        }
    }
}
