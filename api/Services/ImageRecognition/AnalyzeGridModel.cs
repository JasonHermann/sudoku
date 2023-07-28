namespace api.Services.ImageRecognition
{
    public class AnalyzeGridModel
    {
        public bool WasSuccessful { get; set; }
        public int CountDigitsFound { get; set; }
        public string? SudokuBoard { get; set; }

        public CellModel[]? Cells { get; set; }
    }
}
