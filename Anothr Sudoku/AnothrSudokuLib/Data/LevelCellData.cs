
namespace AnothrSudokuLib.Data
{
    public struct LevelCellData
    {
        public readonly int square;
        public readonly int x;
        public readonly int y;
        public readonly int number;
        public readonly bool marked;

        public LevelCellData(int square, int x, int y, int number, bool marked)
        {
            this.square = square;
            this.x = x;
            this.y = y;
            this.number = number;
            this.marked = marked;
        }

        public override string ToString()
        {
            return $"{{ \"{square},{x},{y}\": [{number},{marked}] }}";
        }
    }
}