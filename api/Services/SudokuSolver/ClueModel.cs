namespace api.Services.SudokuSolver
{
    public class ClueModel
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int[] PossibleDigits { get; set; }
        public string Rationale { get; set; }
        public string RationaleComments { get; set; }
    }
}
