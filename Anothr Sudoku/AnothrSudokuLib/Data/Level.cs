using System.Text;

namespace AnothrSudokuLib.Data
{
    public struct Level
    {
        public readonly string levelName;
        public readonly LevelAuthor author;
        public readonly LevelData data;

        public Level(string levelName, LevelAuthor author, LevelData data)
        {
            this.levelName = levelName;
            this.author = author;
            this.data = data;
        }

        public override string ToString()
        {
            var str = new StringBuilder($"Level: {levelName}\n");
            str.Append($"{author}\n");
            str.Append($"{data}\n");
            return str.ToString();
        }
    }
}