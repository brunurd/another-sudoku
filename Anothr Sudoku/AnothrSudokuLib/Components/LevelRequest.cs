using System.Collections.Generic;
using Godot;
using AnothrSudokuLib.Data;

namespace AnothrSudokuLib.Components
{
    public partial class LevelRequest : HttpRequest
    {
        private LevelPathData _levelPathData;
        private LevelAuthor _levelAuthor;
        private LevelData _levelData;
        private WebRequest _webRequest;

        public LevelAuthor LevelAuthor
        {
            get
            {
                return _levelAuthor;
            }
        }

        public LevelData LevelData
        {
            get
            {
                return _levelData;
            }
        }

        public void LoadLevelData(LevelPathData levelPathData)
        {
            _levelPathData = levelPathData;
            _webRequest = new WebRequest(this, _levelPathData.url);
            _webRequest.fallback += GetDataFromCache;
            _webRequest.onSuccess += SaveDataToCache;
            _webRequest.onSuccess += LoadLevelsFromRequest;
            _webRequest.Request();
        }

        private void ProcessDataDictionaryFromJson(Godot.Collections.Dictionary<string, Variant> dict)
        {
            var author = dict["author"].AsGodotDictionary<string, string>();
            var data = dict["data"].AsGodotDictionary<string, int[]>();
            LoadLevels(author, data);
        }

        private void GetDataFromCache()
        {
            if (FileAccess.FileExists(_levelPathData.cachePath))
            {
                Logger.Log($"Loading level \"{_levelPathData.levelName}\" data from cache.", new Logger.Detail("path", _levelPathData.cachePath));
                DirAccess.MakeDirRecursiveAbsolute(Constants.Persistent.LevelsDirCache);
                using var file = FileAccess.Open(_levelPathData.cachePath, FileAccess.ModeFlags.Read);
                var contents = file.GetAsText();
                var json = Json.ParseString(contents).AsGodotDictionary<string,Variant>();
                ProcessDataDictionaryFromJson(json);
                return;
            }
            if (ResourceLoader.Exists(_levelPathData.localDataPath))
            {
                Logger.Log($"Loading level \"{_levelPathData.levelName}\" data from local resource.", new Logger.Detail("path", _levelPathData.localDataPath));
                var levelResource = ResourceLoader.Load<Json>(_levelPathData.localDataPath);
                var json = Json.ParseString(
                                levelResource.Get("data").AsString())
                                .AsGodotDictionary<string, Variant>();
                ProcessDataDictionaryFromJson(json);
            }
        }

        private void SaveDataToCache(WebRequest.Response response)
        {
            Logger.Log(
                $"Saving level \"{_levelPathData.levelName}\" data to cache.",
                new Logger.Detail("path", _levelPathData.cachePath)
            );
            DirAccess.MakeDirRecursiveAbsolute(Constants.Persistent.LevelsDirCache);
            using var file = FileAccess.Open(_levelPathData.cachePath, FileAccess.ModeFlags.Write);
            file.StoreString(response.body);
        }

        private void LoadLevelsFromRequest(WebRequest.Response response)
        {
            var json = response.Json();
            ProcessDataDictionaryFromJson(json);
        }

        private void LoadLevels(Godot.Collections.Dictionary<string, string> author, Godot.Collections.Dictionary<string, int[]> data)
        {
            _levelAuthor = new LevelAuthor(author["name"], author["email"], author["website"]);
            var cells = new List<LevelCellData>();

            foreach (KeyValuePair<string, int[]> cell in data)
            {
                string[] keySplitted = cell.Key.Split(',');
                int[] values = cell.Value;

                cells.Add(new LevelCellData(
                    int.Parse(keySplitted[0]),
                    int.Parse(keySplitted[1]),
                    int.Parse(keySplitted[2]),
                    values[0],
                    values[1] == 1
                ));
            }

            _levelData = new LevelData(cells.ToArray());

            Logger.Log(
                $"Level \"{_levelPathData.levelName}\" loaded.",
                new Logger.Detail("author", _levelAuthor.ToString()),
                new Logger.Detail("levelData", _levelData.ToString())
            );
        }
    }
}
