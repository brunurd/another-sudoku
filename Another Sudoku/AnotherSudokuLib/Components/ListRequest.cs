using System.Collections.Generic;
using Godot;

namespace AnotherSudokuLib.Components
{
    public partial class ListRequest : HttpRequest
    {
        private Godot.Collections.Dictionary<string, Variant> _temporaryRequestResponse;
        private string[] _listData;
        private List<LevelRequest> _levelsRequests;

        public override void _Ready()
        {
            _levelsRequests = new List<LevelRequest>();
            RequestCompleted += LoadContents;
            Request(Constants.Url.LevelList);
        }

        private void LoadContents(long result, long responseCode, string[] headers, byte[] body)
        {
            var json = Json.ParseString(body.GetStringFromUtf8());
            _listData = json.AsGodotDictionary<string, Variant>()["data"].AsStringArray();

            foreach (var url in _listData)
            {
                InstantiateLevelRequest(url);
            }
        }

        private void InstantiateLevelRequest(string url)
        {
            var levelRequestResource = ResourceLoader.Load<PackedScene>(Constants.Resources.Components.LevelRequest);
            var levelRequest = levelRequestResource.Instantiate() as LevelRequest;
            AddChild(levelRequest);
            levelRequest.LoadLevelDataFromRequest(url);
            _levelsRequests.Add(levelRequest);
        }
    }
}
