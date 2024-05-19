using System;
using Godot;

namespace AnotherSudokuLib
{
    public class WebRequest
    {
        public struct Response
        {
            public readonly long result;
            public readonly long responseCode;
            public readonly string[] headers;
            public readonly byte[] body;

            public Response(long result, long responseCode, string[] headers, byte[] body)
            {
                this.result = result;
                this.responseCode = responseCode;
                this.headers = headers;
                this.body = body;
            }

            public Godot.Collections.Dictionary<string,Variant> Json() {
                var json = Godot.Json.ParseString(body.GetStringFromUtf8());
                return json.AsGodotDictionary<string, Variant>();
            }
        }

        private readonly HttpRequest _httpRequest;
        private readonly string _url;
        public short maxAttemps;
        private short _requestCallAttemp = 1;
        public Action fallback;
        public Action<Response> onSuccess;
        public Action<Response> onFail;

        public WebRequest(HttpRequest httpRequest, string url, short maxAttemps = 3) {
            _httpRequest = httpRequest;
            _url = url;
            this.maxAttemps = maxAttemps;
            _httpRequest.RequestCompleted += OnRequestComplete;
        }

        public void Request()
        {
            if (_requestCallAttemp > maxAttemps)
            {
                fallback?.Invoke();
                return;
            }

            try
            {
                Logger.Log(
                    $"Calling: {_url}. attemp: {_requestCallAttemp}/{maxAttemps}.",
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
                Request();
                return;
            }

            try
            {
                onSuccess?.Invoke(response);
                LogResponse("SUCCESSFUL REQUEST.", response);
            }
            catch (Exception err)
            {
                LogResponse($"REQUEST FAILED. {err.Message}", response, Logger.LogLevel.Error);
                _requestCallAttemp += 1;
                onFail?.Invoke(response);
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