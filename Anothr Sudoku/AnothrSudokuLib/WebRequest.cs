using System;
using System.Text;
using Godot;

namespace AnothrSudokuLib
{
    public class WebRequest
    {
        public struct Response
        {
            public readonly long result;
            public readonly long responseCode;
            public readonly string[] headers;
            public readonly byte[] rawBody;
            public readonly string body;

            public Response(long result, long responseCode, string[] headers, byte[] body)
            {
                this.result = result;
                this.responseCode = responseCode;
                this.headers = headers;
                this.rawBody = body;
                this.body = Encoding.UTF8.GetString(body, 0, body.Length);
            }

            public Godot.Collections.Dictionary<string,Variant> Json() {
                var emptyDict = new Godot.Collections.Dictionary<string,Variant>();
                
                if (string.IsNullOrEmpty(body)) {
                    return emptyDict;
                }

                var jsonContentType = Array.Find(headers, header => header.Contains("application/json"));

                if (jsonContentType == null) {
                    return emptyDict;
                }

                try {
                    var json = Godot.Json.ParseString(body);
                    return json.AsGodotDictionary<string, Variant>();
                } catch (Exception err) {
                    Logger.Log(
                        err.Message,
                        Logger.LogLevel.Error,
                        new Logger.Detail("body", body)
                    );
                    return emptyDict;
                }
            }
        }

        public enum State
        {
            NotStarted = 0,
            Loading = 1,
            ReTrying = 2,
            Failed = 3,
            Complete = 4,
        }

        private readonly HttpRequest _httpRequest;
        private readonly string _url;
        public bool verbose;
        public short maxAttemps;
        private short _requestCallAttemp = 1;
        private State _state;

        public Action fallback;
        public Action<Response> onSuccess;
        public Action<Response> onFail;

        public State CurrentState
        {
            get { return _state; }
        }

        public WebRequest(HttpRequest httpRequest, string url, short maxAttemps = 3, bool verbose = true) {
            _httpRequest = httpRequest;
            _url = url;
            this.maxAttemps = maxAttemps;
            this.verbose = verbose;
            _state = State.NotStarted;
            _httpRequest.RequestCompleted += OnRequestComplete;
        }

        public void Request()
        {
            if (_state == State.NotStarted)
            {
                _state = State.Loading;
            }

            if (_requestCallAttemp > maxAttemps)
            {
                fallback?.Invoke();
                _state = State.Failed;
                return;
            }

            try
            {
                Logger.Log(
                    $"Calling: {_url}. attemp: {_requestCallAttemp}/{maxAttemps}.",
                    verbose ? Logger.LogLevel.Info : Logger.LogLevel.Debug,
                    new Logger.Detail("url", _url),
                    new Logger.Detail("attemp", _requestCallAttemp),
                    new Logger.Detail("maxAttemps", maxAttemps)
                );
                _httpRequest.Request(_url);
            }
            catch (Exception err)
            {
                Logger.Log(err.Message, Logger.LogLevel.Error, new Logger.Detail("stackTrace", err.StackTrace));
                _requestCallAttemp += 1;
                _state = State.ReTrying;
                Request();
            }
        }

        private void OnRequestComplete(long result, long responseCode, string[] headers, byte[] body)
        {
            var response = new Response(result, responseCode, headers, body);

            if (responseCode >= 400)
            {
                LogResponse("REQUEST FAILED.", response, Logger.LogLevel.Error);
                _requestCallAttemp += 1;
                onFail?.Invoke(response);
                _state = State.ReTrying;
                Request();
                return;
            }

            try
            {
                onSuccess?.Invoke(response);
                LogResponse("SUCCESSFUL REQUEST.", response, verbose ? Logger.LogLevel.Info : Logger.LogLevel.Debug);
                _state = State.Complete;
            }
            catch (Exception err)
            {
                LogResponse($"REQUEST FAILED. {err.Message}", response, Logger.LogLevel.Error);
                _requestCallAttemp += 1;
                onFail?.Invoke(response);
                _state = State.ReTrying;
                Request();
            }
        }

        private void LogResponse(string message, Response response, Logger.LogLevel logLevel = Logger.LogLevel.Info) {
            Logger.Log(
                message,
                logLevel,
                new Logger.Detail("result", response.result),
                new Logger.Detail("responseCode", response.responseCode),
                new Logger.Detail("headers", response.headers),
                new Logger.Detail("body", response.Json())
            );
        }
    }
}
