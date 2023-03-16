﻿namespace Sudoku
{
    public class SudokuState
    {
        public const byte RowsCount = 9;
        public const byte ColumnsCount = 9;
        public const byte SegmentsCount = 9;

        public SudokuState()
        {
            Grid = new byte[RowsCount * ColumnsCount];
        }

        public SudokuState(string[] initial) : this()
        {
            byte rowNumber = 0;
            foreach (var row in initial)
            {
                var digits = row.Split(" ", StringSplitOptions.None);
                byte columnNumber = 0;
                foreach (var digit in digits)
                {
                    Set(rowNumber, columnNumber, byte.Parse(digit));
                    columnNumber++;
                }
                rowNumber++;
            }
        }

        byte[] Grid { get; set; }

        public void Set(byte row, byte column, byte value)
        {
            Grid[row * ColumnsCount + column] = value;
        }

        public byte Get(byte row, byte column)
        {
            return Grid[row * ColumnsCount + column];
        }
        public bool IsFinished
        {
            get
            {
                foreach (var row in Grid)
                {
                    if (row == 0)
                        return false;
                }
                return true;
            }
        }

        public bool IsValid
        {
            get
            {
                return SudokuState.IsGridValid(Grid);
            }
        }

        public bool IsSolved
        {
            get
            {
                return IsFinished && IsValid;
            }
        }

        public static IEnumerable<byte[]> Segments(byte[] grid)
        {
            for (int rowBase = 0; rowBase < RowsCount; rowBase = rowBase + 3)
            {
                for (int colBase = 0; colBase < ColumnsCount; colBase = colBase + 3)
                {
                    var current = new byte[9];
                    for (int r = 0; r < 3; r++)
                    {
                        for (int c = 0; c < 3; c++)
                        {
                            current[r * 3 + c] = grid[(rowBase + r) * ColumnsCount + (colBase + c)];
                        }
                    }
                    yield return current;
                }
            }
            yield break;
        }

        public static bool IsGridValid(byte[] grid)
        {

            // Check Rows
            for (int row = 0; row < RowsCount; row++)
            {
                byte rowValue = 0;
                for (int i = 0; i < ColumnsCount; i++)
                {
                    var n = grid[row * ColumnsCount + i];
                    if (n == 0)
                        continue;
                    var v = (byte)(1 << n - 1);
                    if ((rowValue & v) != 0)
                        return false;
                    rowValue |= v;
                }
            }

            // Check Columns
            for (int col = 0; col < ColumnsCount; col++)
            {
                byte colValue = 0;
                for (int i = 0; i < RowsCount; i++)
                {
                    var n = grid[i * ColumnsCount + col];
                    if (n == 0)
                        continue;
                    var v = (byte)(1 << n - 1);
                    if ((colValue & v) != 0)
                        return false;
                    colValue |= v;
                }
            }

            // Check Segments
            foreach (var segment in Segments(grid))
            {
                byte segmentValue = 0;
                foreach (var c in segment)
                {
                    if (c == 0)
                        continue;
                    var v = (byte)(1 << c - 1);
                    if ((segmentValue & v) != 0)
                        return false;
                    segmentValue |= v;
                }
            }

            return true;
        }
    }
}