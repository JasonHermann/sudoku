using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Solver
{
    public static class SudokuGenerator
    {
        public static FastSudoku MakePuzzle()
        {
            FastSudoku puzzle = new FastSudoku();
            bool keepGoing = true;
            int maxFailures = 10;

            while (keepGoing)
            {
                try
                {
                    puzzle = AddCell(puzzle, out keepGoing);
                }
                catch (Exception)
                {
                    // Sometimes we generate a bad puzzle, keep trying.
                    puzzle = new FastSudoku();
                    keepGoing = --maxFailures > 0;
                }
            }

            return puzzle;
        }

        static FastSudoku AddCell(FastSudoku puzzle, out bool keepGoing)
        {
            var random = new Random();
            var remaining = Constants.CellCount - puzzle.SolvedCells();
            var cell = random.Next(remaining);

            for (int i = 0; i < Constants.CellCount; i++)
            {
                var annotation = puzzle.Annotations_Cell[i];
                var possibleValues = SudokuSolver.PopCount[annotation];
                if (annotation != Constants.Solved)
                {
                    if (cell == 0)
                    {
                        if (possibleValues == 0)
                        {
                            keepGoing = false;
                            throw new NotSupportedException();
                        }
                        var digitsToSkip = random.Next(possibleValues);

                        var digit = 1;
                        while (digitsToSkip > 0 || (annotation & 1) == 0)
                        {
                            if ((annotation & 1) != 0)
                            {
                                digitsToSkip--;
                            }
                            digit++;
                            annotation >>= 1;
                        }
                        puzzle.SetByCell(i, Constants.Values[digit]);
                        break;
                    }
                    else
                    {
                        cell--;
                    }
                }
            }

            // Do any obvious deductions
            puzzle = SudokuSolver.SolveAllCells(puzzle);
            var solutions = SudokuSolver.FindAllSolutions(puzzle).Take(2).ToList();

            keepGoing = solutions.Count > 1;
            return keepGoing ? puzzle : solutions[0];
        }
    }
}
