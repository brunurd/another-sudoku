using Godot;
using System;
using System.Collections.Generic;

namespace AnotherSudokuLib
{
    public partial class Init : Node2D
    {
        private HttpRequest _httpRequest;

        public override void _Ready()
        {
            _httpRequest = GetNode<HttpRequest>("HTTPRequest");
            _httpRequest.RequestCompleted += (long result, long responseCode, string[] headers, byte[] body) =>
            {
                var json = Json.ParseString(body.GetStringFromUtf8());
                GD.Print(json);
            };
        }
    }
}

// get list
// show buttons list