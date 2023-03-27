using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    public static class SudokuSolver
    {
        public static readonly int[] PopCount;

        static SudokuSolver()
        {
            PopCount = new int[513];
            for(uint i = 0; i < 513; i++)
            {
                PopCount[i] = System.Numerics.BitOperations.PopCount(i);
            }
        }

        public static bool IsValid(FastSudoku sudoku)
        {
            var rows = new int[9];
            var columns = new int[9];
            var segments = new int[9];

            for (int i = 0; i < Constants.CellCount; i++)
            {
                var v = sudoku.Cells[i];
                var r = i / Constants.RowSize;
                var c = i % Constants.ColumnSize;
                var s = (r / 3) * 3 + (c / 3);

                if ((rows[r] & v) != 0)
                    return false;
                if ((columns[c] & v) != 0)
                    return false;
                if ((segments[s] & v) != 0)
                    return false;

                rows[r]     |= v;
                columns[c]  |= v;
                segments[s] |= v;
            }
            return true;
        }

        public static int SolvedCells(FastSudoku sudoku)
        {
            var solved = 0;
            for (int i = 0; i < Constants.CellCount; i++)
            {
                if (sudoku.Cells[i] != 0) solved++;
            }
            return solved;
        }

        public static bool IsFinished(FastSudoku sudoku)
        {
            for (int i = 0; i < Constants.CellCount; i++)
            {
                if (sudoku.Cells[i] == 0)
                    return false;
            }
            return true;
        }

        public static bool IsSolved(FastSudoku sudoku)
        {
            return IsFinished(sudoku) && IsValid(sudoku);
        }

        public static IEnumerable<FastSudoku> FindAllSolutions(FastSudoku sudoku)
        {
            // Reduce as much as possible.
            sudoku = SolveAllCells(sudoku);
            if (SudokuSolver.IsSolved(sudoku))
            {
                yield return sudoku;
            }
            else
            {
                // for every unsolved cell, iterate through all possible moves
                var index_LowestCount = 0;
                var pop_LowestCount = int.MaxValue;
                for(int i = 0; i < Constants.CellCount; i++)
                {
                    var digit = sudoku.Cells[i];
                    if(digit == 0) // Unsolved cell
                    {
                        var value = sudoku.Annotations_Cell[i]; // Possible digits for this cell
                        var pop = PopCount[value]; // Number of possible digits 1 to 9.
                        if (value != Constants.Solved && pop < pop_LowestCount)
                        {
                            index_LowestCount = i;
                            pop_LowestCount = pop;

                            if (pop_LowestCount == 1) // We don't need to keep looking if we find the lowest value (an unsolved cell with only one value).
                                break;
                        }
                    }
                }

                var annotation = sudoku.Annotations_Cell[index_LowestCount];
                for(int digit = 1; digit <=9 ; digit++)
                {
                    var digitValue = Constants.Values[digit];
                    if((annotation & digitValue) != 0) // This digit is possible in this cell.
                    {
                        var puzzle = new FastSudoku(sudoku);
                        puzzle.SetByCell(index_LowestCount, digitValue);

                        foreach(var solution in FindAllSolutions(puzzle))
                        {
                            yield return solution;
                        }
                    }
                }
            }
            yield break;
        }

        public static IEnumerable<SudokuPuzzle> FindAllSolutions(SudokuPuzzle sudoku)
        {
            if (sudoku.IsSolved)
            {
                yield return sudoku;
            }
            else
            {
                // for every unsolved cell, iterate through all possible moves
                for (byte i = 0; i < Constants.RowCount; i++)
                {
                    for (byte j = 0; j < Constants.ColumnCount; j++)
                    {
                        var digit = sudoku.Get(i, j);
                        if (digit == 0) // Unsolved cell
                        {
                            for (byte d = 1; d <= 9; d++)
                            { 
                                if (!sudoku.Rows[i].Contains(d) &&
                                    !sudoku.Columns[j].Contains(d) &&
                                    !sudoku.Boxes[(i/3) * 3 + (j/3)].Contains(d)) // This digit is possible in this cell.
                                {
                                    sudoku.Set(i, j, d);
                                    foreach (var solution in FindAllSolutions(sudoku))
                                    {
                                        yield return solution;
                                    }
                                    sudoku.Set(i, j, 0);
                                }
                            }
                        }
                    }
                }
            }
            yield break;
        }

        public static FastSudoku SolveAllCells(FastSudoku sudoku)
        {
            bool solvedCell = true;
            bool isFinished = false;
            while (solvedCell == true && isFinished == false)
            {
                solvedCell = false;
                isFinished = true;

                solvedCell |= PlaceFindingMethod_ByRow(sudoku);
                solvedCell |= PlaceFindingMethod_ByColumn(sudoku);
                solvedCell |= PlaceFindingMethod_BySegment(sudoku);
                solvedCell |= PreemptiveSetsSearch_ByRow(sudoku);
                solvedCell |= PreemptiveSetsSearch_ByColumn(sudoku);
                solvedCell |= PreemptiveSetsSearch_BySegment(sudoku);
                for (int cell = 0; cell < Constants.CellCount; cell++)
                {
                    var v = sudoku.Cells[cell];
                    if (v == 0) // Unsolved
                    {
                        var a = sudoku.Annotations_Cell[cell];
                        if (PopCount[a] == 1 && a != Constants.Solved) // Candidate Checking Method
                        {
                            sudoku.SetByCell(cell, a);
                            solvedCell = true;
                        }
                        else
                        {
                            isFinished = false;
                        }
                    }
                }

            }

            return sudoku;
        }

        static bool PlaceFindingMethod_ByRow(FastSudoku sudoku)
        {
            bool modifiedCell = false;
            for (int row = 0; row < Constants.RowCount; row++)
            {
                for (int digit = 1; digit <= 9; digit++)
                {
                    var availablePlaces = 0;
                    var baseRowIndex = row * 9;
                    var digitValue = Constants.Values[digit];
                    var lastFoundCell = 0;
                    if ((sudoku.Annotations_Row[row] & digitValue) != 0)
                    {
                        for (int column = 0; column < Constants.ColumnCount; column++)
                        {
                            if ((sudoku.Annotations_Cell[baseRowIndex + column] & digitValue) != 0)
                            {
                                availablePlaces++;
                                lastFoundCell = baseRowIndex + column;
                            }
                        }
                        if(availablePlaces == 0)
                        {
                            // We have an error.

                        }
                        else if(availablePlaces == 1)
                        {
                            sudoku.SetByCell(lastFoundCell, digitValue);
                            modifiedCell = true;
                        }
                        else
                        {
                            // No conclusion
                        }
                    }
                    else
                    {
                        // This digit has already been solved for.
                    }
                }
            }
            return modifiedCell;
        }

        static bool PlaceFindingMethod_ByColumn(FastSudoku sudoku)
        {
            bool modifiedCell = false;
            for (int column = 0; column < Constants.ColumnCount; column++)
            {
                for (int digit = 1; digit <= 9; digit++)
                {
                    var availablePlaces = 0;
                    var digitValue = Constants.Values[digit];
                    var lastFoundCell = 0;
                    if ((sudoku.Annotations_Column[column] & digitValue) != 0)
                    {
                        for (int row = 0; row < Constants.RowCount; row++)
                        {
                            if ((sudoku.Annotations_Cell[row * 9 + column] & digitValue) != 0)
                            {
                                availablePlaces++;
                                lastFoundCell = row * 9 + column;
                            }
                        }
                        if (availablePlaces == 0)
                        {
                            // We have an error.

                        }
                        else if (availablePlaces == 1)
                        {
                            sudoku.SetByCell(lastFoundCell, digitValue);
                            modifiedCell = true;
                        }
                        else
                        {
                            // No conclusion
                        }
                    }
                    else
                    {
                        // This digit has already been solved for.
                    }
                }
            }
            return modifiedCell;
        }

        static bool PlaceFindingMethod_BySegment(FastSudoku sudoku)
        {
            bool modifiedCell = false;
            for (int segment = 0; segment < Constants.SegmentCount; segment++)
            {
                var baseRow = (segment / 3) * 3;
                var baseColumn = (segment % 3) * 3;
                for (int digit = 1; digit <= 9; digit++)
                {
                    var availablePlaces = 0;
                    var digitValue = Constants.Values[digit];
                    var lastFoundCell = 0;
                    if ((sudoku.Annotations_Segment[segment] & digitValue) != 0)
                    {
                        for (int item = 0; item < Constants.SegmentSize; item++)
                        {
                            var cell = (baseRow + (item / 3)) * 9 + baseColumn + (item % 3);
                            if ((sudoku.Annotations_Cell[cell] & digitValue) != 0)
                            {
                                availablePlaces++;
                                lastFoundCell = cell;
                            }
                        }
                        if (availablePlaces == 0)
                        {
                            // We have an error.
                            //throw new NotImplementedException();
                        }
                        else if (availablePlaces == 1)
                        {
                            sudoku.SetByCell(lastFoundCell, digitValue);
                            modifiedCell = true;
                        }
                        else
                        {
                            // No conclusion
                        }
                    }
                    else
                    {
                        // This digit has already been solved for.
                    }
                }
            }
            return modifiedCell;
        }


        static bool PreemptiveSetsSearch_ByRow(FastSudoku sudoku)
        {
            bool modifiedCell = false;
            for (int row = 0; row < Constants.RowCount; row++)
            {
                var baseRowIndex = row * 9;
                var annotationHash = new int[513];
                for (int column = 0; column < Constants.ColumnCount; column++)
                {
                    annotationHash[sudoku.Annotations_Cell[baseRowIndex + column]]++;
                }
                for (int annotationValue = 0; annotationValue < 513; annotationValue++)
                {
                    var frequencyOfAnnotation = annotationHash[annotationValue];
                    if (frequencyOfAnnotation != 0 && frequencyOfAnnotation == PopCount[annotationValue] && annotationValue != Constants.Solved)
                    {
                        // An annotation occurs exactly as many times as the number of digits it constrains
                        // Remove these values from all other values in this row.
                        for (int column = 0; column < Constants.ColumnCount; column++)
                        {
                            var currentCellAnnotationValue = sudoku.Annotations_Cell[baseRowIndex + column];
                            if (currentCellAnnotationValue != annotationValue && currentCellAnnotationValue != Constants.Solved)
                            {
                                sudoku.Annotations_Cell[baseRowIndex + column] &= ~annotationValue;
                                if (currentCellAnnotationValue != sudoku.Annotations_Cell[baseRowIndex + column]) // Only if the annotation of another cell changes.
                                    modifiedCell = true;
                            }
                        }
                    }
                }
            }

            return modifiedCell;
        }

        static bool PreemptiveSetsSearch_ByColumn(FastSudoku sudoku)
        {
            bool modifiedCell = false;
            for (int column = 0; column < Constants.ColumnCount; column++)
            {
                var annotationHash = new int[513];
                for (int row = 0; row < Constants.RowCount; row++)
                {
                    annotationHash[sudoku.Annotations_Cell[row * 9 + column]]++;
                }
                for (int annotationValue = 0; annotationValue < 513; annotationValue++)
                {
                    var frequencyOfAnnotation = annotationHash[annotationValue];
                    if (frequencyOfAnnotation != 0 && frequencyOfAnnotation == PopCount[annotationValue] && annotationValue != Constants.Solved)
                    {
                        // An annotation occurs exactly as many times as the number of digits it constrains
                        // Remove these values from all other values in this column.
                        for (int row = 0; row < Constants.RowCount; row++)
                        {
                            var currentCellAnnotationValue = sudoku.Annotations_Cell[row * 9 + column];
                            if (currentCellAnnotationValue != annotationValue && currentCellAnnotationValue != Constants.Solved)
                            {
                                sudoku.Annotations_Cell[row * 9 + column] &= ~annotationValue;
                                if (currentCellAnnotationValue != sudoku.Annotations_Cell[row * 9 + column]) // Only if the annotation of another cell changes.
                                    modifiedCell = true;
                            }
                        }
                    }
                }
            }

            return modifiedCell;
        }

        static bool PreemptiveSetsSearch_BySegment(FastSudoku sudoku)
        {
            bool modifiedCell = false;
            for (int segment = 0; segment < Constants.SegmentCount; segment++)
            {
                var baseRow = (segment / 3) * 3;
                var baseColumn = (segment % 3) * 3;
                var annotationHash = new int[513];
                for (int item = 0; item < Constants.SegmentSize; item++)
                {
                    var cell = (baseRow + (item / 3)) * 9 + baseColumn + (item % 3);
                    annotationHash[sudoku.Annotations_Cell[cell]]++;
                }
                for (int annotationValue = 0; annotationValue < 513; annotationValue++)
                {
                    var frequencyOfAnnotation = annotationHash[annotationValue];
                    if (frequencyOfAnnotation != 0 && frequencyOfAnnotation == PopCount[annotationValue] && annotationValue != Constants.Solved)
                    {
                        // An annotation occurs exactly as many times as the number of digits it constrains
                        // Remove these values from all other values in this segment.
                        for (int item = 0; item < Constants.SegmentSize; item++)
                        {
                            var cell = (baseRow + (item / 3)) * 9 + baseColumn + (item % 3);
                            var currentCellAnnotationValue = sudoku.Annotations_Cell[cell];
                            if (currentCellAnnotationValue != annotationValue && currentCellAnnotationValue != Constants.Solved)
                            {
                                sudoku.Annotations_Cell[cell] &= ~annotationValue;
                                if (currentCellAnnotationValue != sudoku.Annotations_Cell[cell]) // Only if the annotation of another cell changes.
                                    modifiedCell = true;
                            }
                        }
                    }
                }
            }

            return modifiedCell;
        }
    }
}
