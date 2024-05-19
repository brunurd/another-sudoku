using System;
using System.Collections.Generic;
using Godot;

namespace AnotherSudokuLib.Components
{
    public partial class ListRequest : HttpRequest
    {
        private string[] _listData;
        private List<LevelRequest> _levelsRequests;
        private WebRequest _webRequest;

        public override void _Ready()
        {
            _webRequest = new WebRequest(httpRequest: this, url: Constants.Url.LevelList);
            _webRequest.fallback += GetDataFromCache;
            _webRequest.onSuccess += InstantiateLevels;
            _webRequest.Request();
        }

        private void InstantiateLevels(WebRequest.Response response) {
            _levelsRequests = new List<LevelRequest>();
            var json = response.Json();
            _listData = json["data"].AsStringArray();
            foreach (var url in _listData)
            {
                InstantiateLevelRequest(url);
            }
        }

        private void GetDataFromCache()
        {
            Logger.Log("TODO: Get data from cache.");
            throw new NotImplementedException();
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
