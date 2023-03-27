namespace Sudoku
{
    public class SudokuPuzzle
    {
        public const byte RowsCount = 9;
        public const byte ColumnsCount = 9;
        public const byte SegmentsCount = 9;

        public HashSet<int>[] Rows { get; set; }
        public HashSet<int>[] Columns { get; set; }
        public HashSet<int>[] Boxes { get; set; }

        public SudokuPuzzle()
        {
            Grid = new byte[RowsCount * ColumnsCount];
            Rows    = new HashSet<int>[9];
            Columns = new HashSet<int>[9];
            Boxes   = new HashSet<int>[9];
            for(int i = 0; i < 9; i++)
            {
                Rows[i] = new HashSet<int>();
                Columns[i] = new HashSet<int>();
                Boxes[i] = new HashSet<int>();
            }
        }


        public SudokuPuzzle(byte[] initial) : this()
        {
            InitialSetup(initial);
        }

        public SudokuPuzzle(string[] initial) : this()
        {
            var digits = initial.SelectMany(r => r.Split(" ", StringSplitOptions.None)).Select(d => byte.Parse(d)).ToArray();
            InitialSetup(digits);
        }

        public SudokuPuzzle(char[] initial) : this()
        {
            var digits = initial.Select(d => (byte) (d - '0')).ToArray();
            InitialSetup(digits);
        }

        private void InitialSetup(byte[] initial)
        {
            byte rowNumber = 0;
            byte columnNumber = 0;
            foreach (var digit in initial)
            {
                Set(rowNumber, columnNumber, digit);
                columnNumber++;
                if (columnNumber == 9)
                {
                    columnNumber = 0;
                    rowNumber++;

                }
            }
        }

        byte[] Grid { get; set; }

        public void Set(byte row, byte column, byte value)
        {
            var pos = row * ColumnsCount + column;
            var current = Grid[pos];

            if(current != 0)
            {
                Rows[row].Remove(current);
                Columns[column].Remove(current);
                Boxes[(row / 3) * 3 + (column / 3)].Remove(current);
            }
            Grid[pos] = value;
            if (value != 0)
            {
                var valid = Rows[row].Add(value) && Columns[column].Add(value) && Boxes[(row / 3) * 3 + (column / 3)].Add(value);
                if (!valid)
                    throw new NotSupportedException("You set an incorrect digit.");
            }
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
                return SudokuPuzzle.IsGridValid(Grid);
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