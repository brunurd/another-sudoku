using System.Collections.Generic;
using AnotherSudokuLib.Data;
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
            _levelsRequests = new List<LevelRequest>();
            _webRequest.fallback += GetDataFromCache;
            _webRequest.onSuccess += SaveDataToCache;
            _webRequest.onSuccess += InstantiateLevelsFromRequest;
            _webRequest.Request();
        }

        private void InstantiateLevelsFromRequest(WebRequest.Response response)
        {
            ProcessDataDictionaryFromJson(response.Json());
        }

        private void ProcessDataDictionaryFromJson(Godot.Collections.Dictionary<string, Variant> dict) {
            _listData = dict["data"].AsStringArray();
            foreach (var url in _listData)
            {
                var levelPathData = new LevelPathData(url);
                InstantiateLevel(levelPathData);
            }
        }

        private void GetDataFromCache()
        {
            if (FileAccess.FileExists(Constants.Persistent.LevelListCache))
            {
                Logger.Log("Loading list data from cache.", new Logger.Detail("path", Constants.Persistent.LevelListCache));
                DirAccess.MakeDirRecursiveAbsolute(Constants.Persistent.DataCache);
                using var file = FileAccess.Open(Constants.Persistent.LevelListCache, FileAccess.ModeFlags.Read);
                var contents = file.GetAsText();
                var json = Json.ParseString(contents).AsGodotDictionary<string,Variant>();
                ProcessDataDictionaryFromJson(json);
                return;
            }
            if (ResourceLoader.Exists(Constants.Persistent.LevelList))
            {
                Logger.Log("Loading list data from local resource.", new Logger.Detail("path", Constants.Persistent.LevelList));
                var indexResource = ResourceLoader.Load<Json>(Constants.Persistent.LevelList);
                var json = Json.ParseString(
                                indexResource.Get("data").AsString())
                                .AsGodotDictionary<string,Variant>();
                ProcessDataDictionaryFromJson(json);
            }
        }

        private void SaveDataToCache(WebRequest.Response response)
        {
            Logger.Log(
                "Saving list data to cache.",
                new Logger.Detail("path", Constants.Persistent.LevelListCache)
            );
            DirAccess.MakeDirRecursiveAbsolute(Constants.Persistent.DataCache);
            using var file = FileAccess.Open(Constants.Persistent.LevelListCache, FileAccess.ModeFlags.Write);
            file.StoreString(response.body);
        }

        private void InstantiateLevel(LevelPathData levelPathData)
        {
            if (!ResourceLoader.Exists(Constants.Resources.Components.LevelRequest)) {
                return;
            }

            var levelRequestResource = ResourceLoader.Load<PackedScene>(Constants.Resources.Components.LevelRequest);
            var levelRequest = levelRequestResource.Instantiate() as LevelRequest;
            AddChild(levelRequest);
            levelRequest.LoadLevelData(levelPathData);
            _levelsRequests.Add(levelRequest);
        }
    }
}
