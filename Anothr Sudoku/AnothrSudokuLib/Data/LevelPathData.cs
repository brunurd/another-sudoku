using System.Text.RegularExpressions;
using System.Web;

namespace AnothrSudokuLib.Data
{
    public struct LevelPathData
    {
        public readonly string url;
        public readonly string levelName;
        public readonly string cachePath;
        public readonly string localDataPath;

        public LevelPathData(string url)
        {
            this.url = url;
            this.levelName = "";
            this.cachePath = "";
            this.localDataPath = "";

            var pattern = @".*\/(.*).json$";
            foreach (Match match in Regex.Matches(url, pattern))
            {
                if (match.Groups.Count < 2)
                {
                    continue;
                }
                var encodedLevelName = match.Groups[1].Value;
                levelName = HttpUtility.UrlDecode(encodedLevelName);
                cachePath = $"{Constants.Persistent.LevelsDirCache}/{encodedLevelName}.json";
                localDataPath = $"{Constants.Persistent.LevelsDir}/{encodedLevelName}.json";
            }
        }
    }
}