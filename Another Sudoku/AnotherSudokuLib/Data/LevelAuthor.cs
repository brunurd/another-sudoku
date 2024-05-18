namespace AnotherSudokuLib.Data
{
    public struct LevelAuthor
    {
        public readonly string name;
        public readonly string email;
        public readonly string website;

        public LevelAuthor(string name, string email, string website) {
            this.name = name;
            this.email = email;
            this.website = website;
        }

        public override string ToString()
        {
            return $"{{ \"name\": \"{name}\", \"email\": \"{email}\", \"website\": \"{website}\" }}";
        }
    }
}