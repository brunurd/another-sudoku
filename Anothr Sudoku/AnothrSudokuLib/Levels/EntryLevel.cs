using Godot;
using AnothrSudokuLib.Components;

namespace AnothrSudokuLib.Levels
{
    public partial class EntryLevel : Node
    {
        private ListRequest _listRequest;

        public override void _Ready()
        {
            var listRequestResource = ResourceLoader.Load<PackedScene>(Constants.Resources.Components.ListRequest);
            var listRequest = listRequestResource.Instantiate() as ListRequest;
            AddChild(listRequest);
            _listRequest = listRequest;
        }
    }
}
