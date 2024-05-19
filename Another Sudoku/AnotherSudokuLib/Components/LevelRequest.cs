using System.Collections.Generic;
using Godot;
using Godot.Collections;
using AnotherSudokuLib.Data;
using System;

namespace AnotherSudokuLib.Components
{
    public partial class LevelRequest : HttpRequest
    {
        private string _url;
        private LevelAuthor _levelAuthor;
        private LevelData _levelData;
        private WebRequest _webRequest;

        public LevelAuthor LevelAuthor {
            get {
                return _levelAuthor;
            }
        }

        public LevelData LevelData {
            get {
                return _levelData;
            }
        }

        public void LoadLevelDataFromRequest(string url) {
            _url = url;
            _webRequest = new WebRequest(this, _url);
            _webRequest.fallback += GetDataFromCache;
            _webRequest.onSuccess += LoadLevels;
            _webRequest.Request();
        }

        private void GetDataFromCache()
        {
            Logger.Log("TODO: Get data from cache.");
            throw new NotImplementedException();
        }

        private void LoadLevels(WebRequest.Response response)
        {
            var json = response.Json();

            var author = json["author"].AsGodotDictionary<string,string>();
            _levelAuthor = new LevelAuthor(author["name"], author["email"], author["website"]);

            var data = json["data"].AsGodotDictionary<string,Array<int>>();
            var cells = new List<LevelCellData>();

            foreach (KeyValuePair<string,Array<int>> cell in data) {
                string[] keySplitted = cell.Key.Split(',');
                Array<int> values = cell.Value;

                cells.Add(new LevelCellData(
                    int.Parse(keySplitted[0]),
                    int.Parse(keySplitted[1]),
                    int.Parse(keySplitted[2]),
                    values[0],
                    values[1] == 1
                ));
            }

            _levelData = new LevelData(cells.ToArray());
        }
    }
}
