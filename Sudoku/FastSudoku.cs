using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Sudoku
{
    public static class Constants
    {
        public static readonly Dictionary<int, int> DigitLookup = new Dictionary<int, int>()
        {
            {0, 0},
            {1 << 0, 1},
            {1 << 1, 2},
            {1 << 2, 3},
            {1 << 3, 4},
            {1 << 4, 5},
            {1 << 5, 6},
            {1 << 6, 7},
            {1 << 7, 8},
            {1 << 8, 9},
        };

        public static readonly int[] Values = new int[10] {0,  1 << 0, 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 5, 1 << 6, 1 << 7, 1 << 8 };

        public static readonly int AllDigitsPossible = (1 << 9) - 1; // (2^9 - 1) --> 0 000 000 111 111 111 (last 9 digits all 1s)
        public static readonly int Solved = (1 << 9);

        // --------   --------   --------
        // r1 r1 r1 | r1 r1 r1 | r1 r1 r1 |
        // r2 r2 r2 | r2 r2 r2 | r2 r2 r2 |
        // r3 r3 r3 | r3 r3 r3 | r3 r3 r3 |
        // --------   --------   --------
        // r4 r4 r4 | r4 r4 r4 | r4 r4 r4 |
        // r5 r5 r5 | r5 r5 r5 | r5 r5 r5 |
        // r6 r6 r6 | r6 r6 r6 | r6 r6 r6 |
        // --------   --------   --------
        // r7 r7 r7 | r7 r7 r7 | r7 r7 r7 |
        // r8 r8 r8 | r8 r8 r8 | r8 r8 r8 |
        // r9 r9 r9 | r9 r9 r9 | r9 r9 r9 |
        // --------   --------   --------

        // --------   --------   --------
        // c1 c2 c3 | c4 c5 c6 | c7 c8 c9 |
        // c1 c2 c3 | c4 c5 c6 | c7 c8 c9 |
        // c1 c2 c3 | c4 c5 c6 | c7 c8 c9 |
        // --------   --------   --------
        // c1 c2 c3 | c4 c5 c6 | c7 c8 c9 |
        // c1 c2 c3 | c4 c5 c6 | c7 c8 c9 |
        // c1 c2 c3 | c4 c5 c6 | c7 c8 c9 |
        // --------   --------   --------
        // c1 c2 c3 | c4 c5 c6 | c7 c8 c9 |
        // c1 c2 c3 | c4 c5 c6 | c7 c8 c9 |
        // c1 c2 c3 | c4 c5 c6 | c7 c8 c9 |
        // --------   --------   --------

        // --------   --------   --------
        // s1 s1 s1 | s2 s2 s2 | s3 s3 s3 |
        // s1 s1 s1 | s2 s2 s2 | s3 s3 s3 |
        // s1 s1 s1 | s2 s2 s2 | s3 s3 s3 |
        // --------   --------   --------
        // s4 s4 s4 | s5 s5 s5 | s6 s6 s6 |
        // s4 s4 s4 | s5 s5 s5 | s6 s6 s6 |
        // s4 s4 s4 | s5 s5 s5 | s6 s6 s6 |
        // --------   --------   --------
        // s7 s7 s7 | s8 s8 s8 | s9 s9 s9 |
        // s7 s7 s7 | s8 s8 s8 | s9 s9 s9 |
        // s7 s7 s7 | s8 s8 s8 | s9 s9 s9 |
        // --------   --------   --------


        /// <summary>
        /// Number of Rows
        /// </summary>
        public static readonly int RowCount = 9;
        /// <summary>
        /// How big is each Row
        /// </summary>
        public static readonly int RowSize = 9;

        /// <summary>
        /// Number of Columns
        /// </summary>
        public static readonly int ColumnCount = 9;
        /// <summary>
        /// How big is each Column
        /// </summary>
        public static readonly int ColumnSize = 9;
        /// <summary>
        /// Number of Segments
        /// </summary>
        public static readonly int SegmentCount = 9;
        /// <summary>
        /// How Big is Each Segment
        /// </summary>
        public static readonly int SegmentSize = 9;

        public static readonly int CellCount = 81;

        public static readonly int LinkCount = 20; // Every cell has 8+8+4 links (row, column, segment).
                                                   // --------   --------   --------
                                                   //          |    L9    |          |
                                                   //          |    10    |          |
                                                   //          |    11    |          |
                                                   // --------   --------   --------
                                                   //          | 17 12 18 |          |
                                                   // L1 L2 L3 | L4 XX L5 | L6 L7 L8 |
                                                   //          | 19 13 20 |          |
                                                   // --------   --------   --------
                                                   //          |    14    |          |
                                                   //          |    15    |          |
                                                   //          |    16    |          |
                                                   // --------   --------   --------
    }

    public struct FastSudoku
    {
        public FastSudoku(FastSudoku copy)
        {
            Cells = new int[Constants.CellCount];
            Links = new int[Constants.CellCount, Constants.LinkCount];
            Annotations_Cell = new int[Constants.RowSize * Constants.ColumnSize];
            Annotations_Row = new int[Constants.RowCount];
            Annotations_Column = new int[Constants.ColumnCount];
            Annotations_Segment = new int[Constants.SegmentCount];

            Buffer.BlockCopy(copy.Cells, 0, Cells, 0, copy.Cells.Length * sizeof(int));
            Buffer.BlockCopy(copy.Links, 0, Links, 0, copy.Links.Length * sizeof(int));
            Buffer.BlockCopy(copy.Annotations_Cell, 0, Annotations_Cell, 0, copy.Annotations_Cell.Length * sizeof(int));
            Buffer.BlockCopy(copy.Annotations_Row, 0, Annotations_Row, 0, copy.Annotations_Row.Length * sizeof(int));
            Buffer.BlockCopy(copy.Annotations_Column, 0, Annotations_Column, 0, copy.Annotations_Column.Length * sizeof(int));
            Buffer.BlockCopy(copy.Annotations_Segment, 0, Annotations_Segment, 0, copy.Annotations_Segment.Length * sizeof(int));
        }

        public FastSudoku(int[] digits)
        {
            Cells = new int[Constants.CellCount];
            Links = new int[Constants.CellCount, Constants.LinkCount];
            Annotations_Cell = new int[Constants.RowSize * Constants.ColumnSize];
            Annotations_Row = new int[Constants.RowCount];
            Annotations_Column = new int[Constants.ColumnCount];
            Annotations_Segment = new int[Constants.SegmentCount];
            Array.Fill(Annotations_Cell, Constants.AllDigitsPossible);
            Array.Fill(Annotations_Row, Constants.AllDigitsPossible);
            Array.Fill(Annotations_Column, Constants.AllDigitsPossible);
            Array.Fill(Annotations_Segment, Constants.AllDigitsPossible);
            SetLinks();
            SetFromDigits(digits);
        }
        public FastSudoku()
        {
            Cells = new int[Constants.CellCount];
            Links = new int[Constants.CellCount, Constants.LinkCount];
            Annotations_Cell = new int[Constants.RowSize * Constants.ColumnSize];
            Annotations_Row = new int[Constants.RowCount];
            Annotations_Column = new int[Constants.ColumnCount];
            Annotations_Segment = new int[Constants.SegmentCount];
            Array.Fill(Annotations_Cell, Constants.AllDigitsPossible);
            Array.Fill(Annotations_Row, Constants.AllDigitsPossible);
            Array.Fill(Annotations_Column, Constants.AllDigitsPossible);
            Array.Fill(Annotations_Segment, Constants.AllDigitsPossible);
            SetLinks();
        }

        public int[] Cells;
        public int[,] Links;
        public int[] Annotations_Cell;
        public int[] Annotations_Row;
        public int[] Annotations_Column;
        public int[] Annotations_Segment;

        public int SolvedCells()
        {
            var solved = 0;
            for(int i = 0; i < Constants.CellCount; i++)
            {
                if (Cells[i] != 0) solved++;
            }
            return solved;
        }

        public void SetByRowColumn(int row, int column, int value)
        {
            SetByCell(row * Constants.RowSize + column, value);
        }

        public void SetByCell(int cell, int value)
        {
            var previousValue = Cells[cell];
            Cells[cell] = value;

            var row = cell / Constants.RowSize;
            var column = cell % Constants.RowSize;
            var segment = (row / 3) * 3 + (column / 3);

            if (value == 0)
            {
                throw new NotImplementedException();
            }
            else
            {
                Annotations_Cell[cell] = Constants.Solved;
                Annotations_Row[row]         &= ~value;
                Annotations_Column[column]   &= ~value;
                Annotations_Segment[segment] &= ~value;

                for (int i = 0; i < Constants.LinkCount; i++)
                {
                    Annotations_Cell[Links[cell, i]] &= ~value;
                }
            }

        }

        void SetLinks()
        {
            for(int cell = 0; cell < Cells.Length; cell++)
            {
                var row = cell / Constants.RowSize;
                var column = cell % Constants.RowSize;
                var segmentRow = row % 3;
                var segmentColumn = column % 3;
                var segmentBaseRow = (row / 3) * 3;
                var segmentBaseColumn = (column / 3) * 3;

                var index = 0;
                for (int r = (column + 1) % 9; r != column; r = (r + 1) % 9)
                {
                    Links[cell, index++] = row * 9 + r;
                    ;
                }
                for (int c = (row + 1) % 9; c != row; c = (c + 1) % 9)
                {
                    Links[cell, index++] = c * 9 + column;
                }
                for(int s = 0; s < 9; s++)
                {
                    if (s / 3 == segmentRow) continue;
                    if (s % 3 == segmentColumn) continue;

                    Links[cell, index++] = (segmentBaseRow + (s/3)) * 9 + segmentBaseColumn + s % 3;
                }
            }
        }

        void SetFromDigits(int[] digits)
        {
            for(int cell = 0; cell < digits.Length; cell++)
            {
                var value = Constants.Values[digits[cell]];
                if (value != 0)
                    SetByCell(cell, value);
            }
        }
    }
}
