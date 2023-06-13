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

        public static int GuessCounter = 0; 
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
                var r = Constants.RowPositions[i];
                var c = Constants.ColumnPositions[i];
                var s = Constants.SegmentPositions[i];

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
                int[] popCount = new int[81];
                var index_HighestNeighborCount = 0;
                var highestNeighborCount = 0;
                var pop_LowestCount = int.MaxValue;
                for (int i = 0; i < Constants.CellCount; i++)
                {
                    var digit = sudoku.Cells[i];
                    if(digit == 0) // Unsolved cell
                    {
                        var value = sudoku.Annotations_Cell[i]; // Possible digits for this cell
                        var pop = PopCount[value]; // Number of possible digits 1 to 9.
                        popCount[i] = pop;

                        if (pop < pop_LowestCount)
                        {
                            pop_LowestCount = pop;
                        }
                    }
                }
                for(int i =0; i< Constants.CellCount; i++)
                {
                    if (popCount[i] == pop_LowestCount)
                    {
                        var nc = 0;
                        for (int n = 0; n < Constants.LinkCount; n++)
                        {
                            nc += popCount[sudoku.Links[i, n]];
                        }
                        if(nc > highestNeighborCount)
                        {
                            highestNeighborCount = nc;
                            index_HighestNeighborCount = i;
                        }
                    }
                }

                var annotation = sudoku.Annotations_Cell[index_HighestNeighborCount];
                for(int digit = 1; digit <=9 ; digit++)
                {
                    var digitValue = Constants.Values[digit];
                    var digitValue_Extended = Constants.ValuesExtended[digit];
                    if((annotation & digitValue) != 0) // This digit is possible in this cell.
                    {
                        GuessCounter++;
                        var puzzle = new FastSudoku(sudoku);
                        puzzle.SetByCell(index_HighestNeighborCount, digitValue, digitValue_Extended);

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
                while (solvedCell == true && isFinished == false)
                {
                    solvedCell = false;
                    isFinished = true;

                    for (int cell = 0; cell < Constants.CellCount; cell++)
                    {
                        var v = sudoku.Cells[cell];
                        if (v == 0) // Unsolved
                        {
                            var a = sudoku.Annotations_Cell[cell];
                            var a_ext = sudoku.Annotations_CellExtended[cell];
                            if (PopCount[a] == 1 && a != Constants.Solved) // Candidate Checking Method
                            {
                                sudoku.SetByCell(cell, a, a_ext);
                                solvedCell = true;
                            }
                            else
                            {
                                // We found at least one unsolved cell.
                                isFinished = false;
                            }
                        }
                    }
                    if (solvedCell) continue;
                    solvedCell |= PlaceFindingMethod_ByRow(sudoku);
                    if (solvedCell) continue;
                    solvedCell |= PlaceFindingMethod_ByColumn(sudoku);
                    if (solvedCell) continue;
                    solvedCell |= PlaceFindingMethod_BySegment(sudoku);
                }
                // These are more expensive, so do them less often.
                solvedCell |= PreemptiveSetsSearch_ByRow(sudoku);
                if (solvedCell) continue;
                solvedCell |= PreemptiveSetsSearch_ByColumn(sudoku);
                if (solvedCell) continue;
                solvedCell |= PreemptiveSetsSearch_BySegment(sudoku);
            }
            return sudoku;
        }

        static bool PlaceFindingMethod_ByRow(FastSudoku sudoku)
        {
            bool modifiedCell = false;
            for (int row = 0; row < Constants.RowCount; row++)
            {
                if (sudoku.Annotations_RowExtendedSum[row] == Constants.SectionSolvedExtended)
                    continue;

                var e = sudoku.Annotations_RowExtendedSum[row];
                for(int digit = 1; digit <=9; digit++)
                {
                    var mask = Constants.ValueExtendedMask[digit];
                    var digitValue_Extended = Constants.ValuesExtended[digit];
                    var digitValue = Constants.Values[digit];

                    if((e & mask) == digitValue_Extended)
                    {
                        for (int column = 0; column < Constants.ColumnCount; column++)
                        {
                            var cell = Constants.RowColumn[row, column];
                            if ((sudoku.Annotations_Cell[cell] & digitValue) != 0)
                            {
                                sudoku.SetByCell(cell, digitValue,digitValue_Extended);
                                modifiedCell = true;
                                e = sudoku.Annotations_RowExtendedSum[row];
                                break;
                            }
                        }
                    }
                }
            }
            return modifiedCell;
        }

        static bool PlaceFindingMethod_ByColumn(FastSudoku sudoku)
        {
            bool modifiedCell = false;
            for (int col = 0; col < Constants.ColumnCount; col++)
            {
                if (sudoku.Annotations_ColumnExtendedSum[col] == Constants.SectionSolvedExtended)
                    continue;

                var e = sudoku.Annotations_ColumnExtendedSum[col];
                for (int digit = 1; digit <= 9; digit++)
                {
                    var mask = Constants.ValueExtendedMask[digit];
                    var digitValue_Extended = Constants.ValuesExtended[digit];
                    var digitValue = Constants.Values[digit];

                    if ((e & mask) == digitValue_Extended)
                    {
                        for (int row = 0; row < Constants.RowCount; row++)
                        {
                            var cell = Constants.RowColumn[row, col];
                            if ((sudoku.Annotations_Cell[cell] & digitValue) != 0)
                            {
                                sudoku.SetByCell(cell, digitValue, digitValue_Extended);
                                modifiedCell = true;
                                e = sudoku.Annotations_ColumnExtendedSum[col];
                                break;
                            }
                        }
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
                if (sudoku.Annotations_SegmentExtendedSum[segment] == Constants.SectionSolvedExtended)
                    continue;

                var baseRow = (segment / 3) * 3;
                var baseColumn = (segment % 3) * 3;
                var e = sudoku.Annotations_SegmentExtendedSum[segment];
                for (int digit = 1; digit <= 9; digit++)
                {
                    var mask = Constants.ValueExtendedMask[digit];
                    var digitValue_Extended = Constants.ValuesExtended[digit];
                    var digitValue = Constants.Values[digit];

                    if((e & mask) == digitValue_Extended)
                    {
                        for (int item = 0; item < Constants.SegmentSize; item++)
                        {
                            var cell = (baseRow + (item / 3)) * 9 + baseColumn + (item % 3);
                            if ((sudoku.Annotations_Cell[cell] & digitValue) != 0)
                            {
                                sudoku.SetByCell(cell, digitValue, digitValue_Extended);
                                e = sudoku.Annotations_SegmentExtendedSum[segment];
                                modifiedCell = true;
                                break;
                            }
                        }
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
                if (sudoku.Annotations_RowExtendedSum[row] == Constants.SectionSolvedExtended)
                    continue;
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
                                var cell = Constants.RowColumn[row, column];
                                sudoku.EliminateAnnotation(cell, annotationValue);
                                if (currentCellAnnotationValue != sudoku.Annotations_Cell[cell]) // Only if the annotation of another cell changes.
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
                if (sudoku.Annotations_ColumnExtendedSum[column] == Constants.SectionSolvedExtended)
                    continue;

                var annotationHash = new int[513];
                for (int row = 0; row < Constants.RowCount; row++)
                {
                    annotationHash[sudoku.Annotations_Cell[Constants.RowColumn[row, column]]]++;
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
                            var c = Constants.RowColumn[row, column];
                            var currentCellAnnotationValue = sudoku.Annotations_Cell[c];
                            if (currentCellAnnotationValue != annotationValue && currentCellAnnotationValue != Constants.Solved)
                            {
                                sudoku.EliminateAnnotation(c, annotationValue);
                                if (currentCellAnnotationValue != sudoku.Annotations_Cell[c]) // Only if the annotation of another cell changes.
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
                if (sudoku.Annotations_SegmentExtendedSum[segment] == Constants.SectionSolvedExtended)
                    continue;

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
                                sudoku.EliminateAnnotation(cell, annotationValue);
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
