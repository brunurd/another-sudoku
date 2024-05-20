using System;

namespace AnothrSudokuLib
{
    public class UnloadedException : Exception
    {
        public UnloadedException(string subject) : base($"The {subject} was called before been loaded.")
        {
        }
    }
}