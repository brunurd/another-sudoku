namespace AnotherSudokuLib
{
    public static class Constants
    {
        public static class Resources
        {
            public static class Components
            {
                public const string LevelRequest = "res://Scenes/Components/LevelRequest.tscn";
                public const string ListRequest = "res://Scenes/Components/ListRequest.tscn";
            }
        }

        public static class Url
        {
            public const string LevelList = "https://lavaleak.github.io/another-sudoku/index.json";
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
