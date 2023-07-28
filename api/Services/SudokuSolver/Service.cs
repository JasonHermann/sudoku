using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Sudoku;

namespace api.Services.SudokuSolver
{
    public class Service
    {
        public static readonly ParserAttributes ExpectedAttributes = ParserAttributes.List | ParserAttributes.NumberNotDelimited | ParserAttributes.EmptyAsZero;

        public Service() { }

        public string Solve(string board)
        {
            if (board == null) return "";

            var initialDigits = Parser.GridFromString(board, ExpectedAttributes);
            var sudoku = new FastSudoku(initialDigits);

            var solvedSudoku = Sudoku.SudokuSolver.SolveAllCells(sudoku);
            var solvedDigits = solvedSudoku.Cells.Select(x => Constants.DigitLookup[x]).ToArray();
            var solution =  Parser.StringFromGrid(solvedDigits, ExpectedAttributes);
            return solution;
        }
    }
}
