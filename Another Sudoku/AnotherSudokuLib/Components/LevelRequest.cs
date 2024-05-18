using System.Collections.Generic;
using Godot;
using Godot.Collections;
using AnotherSudokuLib.Data;

namespace AnotherSudokuLib.Components
{
    public partial class LevelRequest : HttpRequest
    {
        private string _url;
        private LevelAuthor _levelAuthor;
        private LevelData _levelData;

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
            RequestCompleted += LoadContents;
            Request(url);
        }

        private void LoadContents(long result, long responseCode, string[] headers, byte[] body)
        {
            var json = Json.ParseString(body.GetStringFromUtf8());

            var author = json.AsGodotDictionary<string, Variant>()["author"].AsGodotDictionary<string,string>();
            _levelAuthor = new LevelAuthor(author["name"], author["email"], author["website"]);

            var data = json.AsGodotDictionary<string, Variant>()["data"].AsGodotDictionary<string,Array<int>>();
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
