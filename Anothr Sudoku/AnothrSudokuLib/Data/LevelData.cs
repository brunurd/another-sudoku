namespace AnothrSudokuLib.Data
{
    public struct LevelData
    {
        public readonly LevelCellData[] cells;

        public LevelData(LevelCellData[] cells)
        {
            this.cells = cells;
        }

        public override string ToString()
        {
            return string.Join(",", cells);
        }
    }
}