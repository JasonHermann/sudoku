using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Sudoku
{
    public static class Constants
    {
        static Constants()
        {
            AnnotationLookups = new UInt64[513];
            RowColumn = new int[RowCount, ColumnCount];
            RowPositions = new int[CellCount];
            ColumnPositions = new int[CellCount];
            SegmentPositions = new int[CellCount];
            for (int i = 0; i < CellCount; i++)
            {
                var row = i / Constants.RowSize;
                var column = i % Constants.RowSize;
                var segment = (row / 3) * 3 + (column / 3);

                RowColumn[row, column] = i;
                RowPositions[i] = row;
                ColumnPositions[i] = column;
                SegmentPositions[i] = segment;
            }

            for (UInt64 i = 0; i < 513; i++)
            {
                var value = ((i >> 0) & 1UL) << 0 |
                            ((i >> 1) & 1UL) << (4 * 1) |
                            ((i >> 2) & 1UL) << (4 * 2) |
                            ((i >> 3) & 1UL) << (4 * 3) |
                            ((i >> 4) & 1UL) << (4 * 4) |
                            ((i >> 5) & 1UL) << (4 * 5) |
                            ((i >> 6) & 1UL) << (4 * 6) |
                            ((i >> 7) & 1UL) << (4 * 7) |
                            ((i >> 8) & 1UL) << (4 * 8);
                AnnotationLookups[i] = value;
            }
        }

        public static readonly UInt64[] AnnotationLookups;
        public static readonly int[,] RowColumn;
        public static readonly int[] RowPositions;
        public static readonly int[] ColumnPositions;
        public static readonly int[] SegmentPositions;

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

        public static readonly int[] Values = new int[10] { 0, 1 << 0, 1 << 1, 1 << 2, 1 << 3, 1 << 4, 1 << 5, 1 << 6, 1 << 7, 1 << 8 };
        public static readonly UInt64[] ValuesExtended = new UInt64[10]
        {
            0,
            1UL << 0,
            1UL << (1*4),
            1UL << (2*4),
            1UL << (3*4),
            1UL << (4*4),
            1UL << (5*4),
            1UL << (6*4),
            1UL << (7*4),
            1UL << (8*4)
        };
        public static readonly UInt64[] ValueExtendedMask = new UInt64[10]
        {
            0,
            (15UL << 0) ,
            (15UL << (1*4)) ,
            (15UL << (2*4)) ,
            (15UL << (3*4)) ,
            (15UL << (4*4)) ,
            (15UL << (5*4)) ,
            (15UL << (6*4)) ,
            (15UL << (7*4)) ,
            (15UL << (8*4))
        };

        public static readonly int AllDigitsPossible = (1 << 9) - 1; // (2^9 - 1) --> 0 000 000 111 111 111 (last 9 digits all 1s)
        public static readonly UInt64 AllDigitsPossibleExtended = (1UL << 0) | (1UL << (1 * 4)) | (1UL << (2 * 4)) | (1UL << (3 * 4)) | (1UL << (4 * 4)) | (1UL << (5 * 4)) |
            (1UL << (6 * 4)) | (1UL << (7 * 4)) | (1UL << (8 * 4));

        public static readonly UInt64 AllDigitsPossibleExtendedSum = (9UL << 0) | (9UL << (1 * 4)) | (9UL << (2 * 4)) | (9UL << (3 * 4)) | (9UL << (4 * 4)) | (9UL << (5 * 4)) |
            (9UL << (6 * 4)) | (9UL << (7 * 4)) | (9UL << (8 * 4));

        public static readonly UInt64 SectionSolvedExtended = (9UL << (9 * 4));


        public static readonly int Solved = (1 << 9);
        public static readonly UInt64 SolvedExtended = (1UL << (9 * 4));

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
            Links = new int[Constants.CellCount, Constants.LinkCount];
            Buffer.BlockCopy(copy.Links, 0, Links, 0, copy.Links.Length * sizeof(int));

            Cells = new int[Constants.CellCount];
            Annotations_Cell = new int[Constants.RowSize * Constants.ColumnSize];
            Annotations_Row = new int[Constants.RowCount];
            Annotations_Column = new int[Constants.ColumnCount];
            Annotations_Segment = new int[Constants.SegmentCount];
            Buffer.BlockCopy(copy.Cells, 0, Cells, 0, copy.Cells.Length * sizeof(int));
            Buffer.BlockCopy(copy.Annotations_Cell, 0, Annotations_Cell, 0, copy.Annotations_Cell.Length * sizeof(int));
            Buffer.BlockCopy(copy.Annotations_Row, 0, Annotations_Row, 0, copy.Annotations_Row.Length * sizeof(int));
            Buffer.BlockCopy(copy.Annotations_Column, 0, Annotations_Column, 0, copy.Annotations_Column.Length * sizeof(int));
            Buffer.BlockCopy(copy.Annotations_Segment, 0, Annotations_Segment, 0, copy.Annotations_Segment.Length * sizeof(int));

            CellsExtended = new UInt64[Constants.CellCount];
            Annotations_CellExtended = new UInt64[Constants.RowSize * Constants.ColumnSize];
            Annotations_RowExtendedSum = new UInt64[Constants.RowCount];
            Annotations_ColumnExtendedSum = new UInt64[Constants.ColumnCount];
            Annotations_SegmentExtendedSum = new UInt64[Constants.SegmentCount];
            Buffer.BlockCopy(copy.CellsExtended, 0, CellsExtended, 0, copy.CellsExtended.Length * sizeof(UInt64));
            Buffer.BlockCopy(copy.Annotations_CellExtended, 0, Annotations_CellExtended, 0, copy.Annotations_CellExtended.Length * sizeof(UInt64));
            Buffer.BlockCopy(copy.Annotations_RowExtendedSum, 0, Annotations_RowExtendedSum, 0, copy.Annotations_RowExtendedSum.Length * sizeof(UInt64));
            Buffer.BlockCopy(copy.Annotations_ColumnExtendedSum, 0, Annotations_ColumnExtendedSum, 0, copy.Annotations_ColumnExtendedSum.Length * sizeof(UInt64));
            Buffer.BlockCopy(copy.Annotations_SegmentExtendedSum, 0, Annotations_SegmentExtendedSum, 0, copy.Annotations_SegmentExtendedSum.Length * sizeof(UInt64));
        }

        public FastSudoku(int[] digits)
        {
            Links = new int[Constants.CellCount, Constants.LinkCount];

            Cells = new int[Constants.CellCount];
            Annotations_Cell = new int[Constants.RowSize * Constants.ColumnSize];
            Annotations_Row = new int[Constants.RowCount];
            Annotations_Column = new int[Constants.ColumnCount];
            Annotations_Segment = new int[Constants.SegmentCount];
            Array.Fill(Annotations_Cell, Constants.AllDigitsPossible);
            Array.Fill(Annotations_Row, Constants.AllDigitsPossible);
            Array.Fill(Annotations_Column, Constants.AllDigitsPossible);
            Array.Fill(Annotations_Segment, Constants.AllDigitsPossible);

            CellsExtended = new UInt64[Constants.CellCount];
            Annotations_CellExtended = new UInt64[Constants.RowSize * Constants.ColumnSize];
            Annotations_RowExtendedSum = new UInt64[Constants.RowCount];
            Annotations_ColumnExtendedSum = new UInt64[Constants.ColumnCount];
            Annotations_SegmentExtendedSum = new UInt64[Constants.SegmentCount];
            Array.Fill(Annotations_CellExtended, Constants.AllDigitsPossibleExtended);
            Array.Fill(Annotations_RowExtendedSum, Constants.AllDigitsPossibleExtendedSum);
            Array.Fill(Annotations_ColumnExtendedSum, Constants.AllDigitsPossibleExtendedSum);
            Array.Fill(Annotations_SegmentExtendedSum, Constants.AllDigitsPossibleExtendedSum);
            SetLinks();
            SetFromDigits(digits);
        }
        public FastSudoku()
        {
            Links = new int[Constants.CellCount, Constants.LinkCount];
            Cells = new int[Constants.CellCount];
            Annotations_Cell = new int[Constants.RowSize * Constants.ColumnSize];
            Annotations_Row = new int[Constants.RowCount];
            Annotations_Column = new int[Constants.ColumnCount];
            Annotations_Segment = new int[Constants.SegmentCount];
            Array.Fill(Annotations_Cell, Constants.AllDigitsPossible);
            Array.Fill(Annotations_Row, Constants.AllDigitsPossible);
            Array.Fill(Annotations_Column, Constants.AllDigitsPossible);
            Array.Fill(Annotations_Segment, Constants.AllDigitsPossible);

            CellsExtended = new UInt64[Constants.CellCount];
            Annotations_CellExtended = new UInt64[Constants.RowSize * Constants.ColumnSize];
            Annotations_RowExtendedSum = new UInt64[Constants.RowCount];
            Annotations_ColumnExtendedSum = new UInt64[Constants.ColumnCount];
            Annotations_SegmentExtendedSum = new UInt64[Constants.SegmentCount];
            Array.Fill(Annotations_CellExtended, Constants.AllDigitsPossibleExtended);
            Array.Fill(Annotations_RowExtendedSum, Constants.AllDigitsPossibleExtendedSum);
            Array.Fill(Annotations_ColumnExtendedSum, Constants.AllDigitsPossibleExtendedSum);
            Array.Fill(Annotations_SegmentExtendedSum, Constants.AllDigitsPossibleExtendedSum);
            SetLinks();
        }

        public int[] Cells;
        public int[,] Links;
        public int[] Annotations_Cell;
        public int[] Annotations_Row;
        public int[] Annotations_Column;
        public int[] Annotations_Segment;
        public UInt64[] CellsExtended;
        public UInt64[] Annotations_CellExtended;
        public UInt64[] Annotations_RowExtendedSum;
        public UInt64[] Annotations_ColumnExtendedSum;
        public UInt64[] Annotations_SegmentExtendedSum;

        public int SolvedCells()
        {
            var solved = 0;
            for (int i = 0; i < Constants.CellCount; i++)
            {
                if (Cells[i] != 0) solved++;
            }
            return solved;
        }

        public void SetByRowColumn(int row, int column, int value, UInt64 extended)
        {
            SetByCell(Constants.RowColumn[row, column], value, extended);
        }

        public void EliminateAnnotation(int cell, int value)
        {
            Annotations_Cell[cell] &= ~value;
            var previous = Annotations_CellExtended[cell];
            var lookup = Constants.AnnotationLookups[value];
            var proposed = (previous) & (~lookup);
            if(proposed != previous)
            {
                Annotations_CellExtended[cell] = proposed;
                var row = Constants.RowPositions[cell];
                var column = Constants.ColumnPositions[cell];
                var segment = Constants.SegmentPositions[cell];
                //var sum = proposed - previous;
                Annotations_RowExtendedSum[row] -= previous;
                Annotations_ColumnExtendedSum[column] -= previous;
                Annotations_SegmentExtendedSum[segment] -= previous;
                Annotations_RowExtendedSum[row] += proposed;
                Annotations_ColumnExtendedSum[column] += proposed;
                Annotations_SegmentExtendedSum[segment] += proposed;
            }

        }

        public void SetByCell(int cell, int value, UInt64 extended)
        {
            Cells[cell] = value;
            CellsExtended[cell] = extended;

            var row = Constants.RowPositions[cell];
            var column = Constants.ColumnPositions[cell];
            var segment = Constants.SegmentPositions[cell];

            if (value == 0)
            {
                throw new NotImplementedException();
            }
            else
            {
                var ov = ~value;
                var ove = ~extended;
                Annotations_Cell[cell] = Constants.Solved;
                Annotations_Row[row] &= ov;
                Annotations_Column[column] &= ov;
                Annotations_Segment[segment] &= ov;
                var previous = Annotations_CellExtended[cell];
                var sum = (Constants.SolvedExtended - previous);
                Annotations_CellExtended[cell] = Constants.SolvedExtended;
                Annotations_RowExtendedSum[row] += sum;
                Annotations_ColumnExtendedSum[column] += sum;
                Annotations_SegmentExtendedSum[segment] += sum;
                // Commented out these 3 lines, becuase the linked cell below should do this.
                //Annotations_RowExtendedSum[row]         &= ove;
                //Annotations_ColumnExtendedSum[column]   &= ove;
                //Annotations_SegmentExtendedSum[segment] &= ove;

                for (int i = 0; i < Constants.LinkCount; i++)
                {
                    var l = Links[cell, i];
                    if (Annotations_Cell[l] != Constants.Solved)
                    {
                        Annotations_Cell[l] &= ov;
                        previous = Annotations_CellExtended[l];
                        // If this linked cell was marked as possibly having this value, remove it
                        // and also remove it from the linked cells container sums.
                        if ((previous & extended) != 0)
                        {
                            Annotations_CellExtended[l] &= ove;
                            var r = Constants.RowPositions[l];
                            Annotations_RowExtendedSum[r] -= extended;
                            var c = Constants.ColumnPositions[l];
                            Annotations_ColumnExtendedSum[c] -= extended;
                            var s = Constants.SegmentPositions[l];
                            Annotations_SegmentExtendedSum[s] -= extended;
                        }
                    }
                }
            }

        }

        void SetLinks()
        {
            for (int cell = 0; cell < Cells.Length; cell++)
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
                }
                for (int c = (row + 1) % 9; c != row; c = (c + 1) % 9)
                {
                    Links[cell, index++] = c * 9 + column;
                }
                for (int s = 0; s < 9; s++)
                {
                    if (s / 3 == segmentRow) continue;
                    if (s % 3 == segmentColumn) continue;

                    Links[cell, index++] = (segmentBaseRow + (s / 3)) * 9 + segmentBaseColumn + s % 3;
                }
            }
        }

        void SetFromDigits(int[] digits)
        {
            for (int cell = 0; cell < digits.Length; cell++)
            {
                var d = digits[cell];
                var value = Constants.Values[d];
                var extended = Constants.ValuesExtended[d];
                if (value != 0)
                    SetByCell(cell, value, extended);
            }
        }
    }
}
