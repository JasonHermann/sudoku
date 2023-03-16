using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    [Flags]
    public enum ParserAttributes
    {
        List = 1 << 0,
        Rows = 1 << 1,
        GridShownUsingPipes = 1 << 2,
        EmptyAsSpace = 1 << 3,
        EmptyAsZero = 1 << 4,
        EmptyAsDash = 1 << 5,
        NumberDelimitedBySpace = 1 << 6,
        NumberDelimitedByComma = 1 << 7,
        NumberNotDelimited = 1 << 8,
    }

    public enum Tokens
    {
        Digit = 0,
        DigitDelimiter = 1,
        GridDelimiter = 2,
    }

    public static class Parser
    {
        public static string Print(FastSudoku puzzle)
        {
            var digits = puzzle.Cells.Select(x => Constants.DigitLookup[x]).ToArray();
            return StringFromGrid(digits, ParserAttributes.Rows | ParserAttributes.EmptyAsDash | ParserAttributes.NumberDelimitedBySpace);
        }


        /// <summary>
        /// Given a string and grid parameters, return the values.
        /// </summary>
        /// <returns>
        /// 81 integers
        /// </returns>
        public static int[] GridFromString(string sudoku, ParserAttributes parserAttributes)
        {
            if((parserAttributes & ParserAttributes.List) != 0)
            {
                return ParseList(sudoku, parserAttributes);
            }
            else if((parserAttributes & ParserAttributes.Rows) != 0)
            {
                return ParseRows(sudoku, parserAttributes);
            }
            throw new NotSupportedException("At least one of List or Rows must be turned on.");
        }

        public static string StringFromGrid(int[] sudokuGrid, ParserAttributes parserAttributes)
        {
            if ((parserAttributes & ParserAttributes.List) != 0)
            {
                return ParseGridList(sudokuGrid, parserAttributes);
            }
            else if ((parserAttributes & ParserAttributes.Rows) != 0)
            {
                return ParseGridRows(sudokuGrid, parserAttributes);
            }
            throw new NotSupportedException("At least one of List or Rows must be turned on.");
        }

        static char GetDelimiter(ParserAttributes attributes)
        {
            if ((attributes & ParserAttributes.NumberDelimitedBySpace) != 0)
                return ' ';
            if ((attributes & ParserAttributes.NumberDelimitedByComma) != 0)
                return ',';
            if ((attributes & ParserAttributes.NumberNotDelimited) != 0)
                return '\0';
            throw new NotSupportedException("At least one of Space/Comma delimiter must be turned on.");
        }


        static char GetEmptyDigit(ParserAttributes attributes)
        {
            if ((attributes & ParserAttributes.EmptyAsDash) != 0)
                return '-';
            if ((attributes & ParserAttributes.EmptyAsZero) != 0)
                return '0';
            if ((attributes & ParserAttributes.EmptyAsSpace) != 0)
                return ' ';
            throw new NotSupportedException("At least one of Dash/Zero/Space delimiter must be turned on.");
        }

        static string GetRowDelimiter(ParserAttributes parserAttributes)
        {
            return Environment.NewLine;
        }

        static int[] ParseList(string sudoku, ParserAttributes parserAttributes)
        {
            var output = new int[81];
            char delimiter = GetDelimiter(parserAttributes);
            char emptyDigit = GetEmptyDigit(parserAttributes);
            int cell = 0;
            int stringIndex = 0;
            var expectedToken = Tokens.Digit;
            while (cell < 81 && stringIndex < sudoku.Length)
            {
                var c = sudoku[stringIndex];
                if (expectedToken == Tokens.Digit)
                {
                    if (c == emptyDigit)
                        output[cell] = 0;
                    else
                        output[cell] = c - '0';

                    if (delimiter != Char.MinValue)
                        expectedToken = Tokens.DigitDelimiter;

                    cell++;
                }
                else if (expectedToken == Tokens.DigitDelimiter)
                {
                    // No Op
                    expectedToken = Tokens.Digit;
                }

                stringIndex++;
            }
            return output;
        }

        static int[] ParseRows(string sudoku, ParserAttributes parserAttributes)
        {
            throw new NotImplementedException("I haven't written this code yet.");
        }

        static string ParseGridRows(int[] sudokuGrid, ParserAttributes parserAttributes)
        {
            var output = new StringBuilder();
            char delimiter = GetDelimiter(parserAttributes);
            char emptyDigit = GetEmptyDigit(parserAttributes);
            string rowDelimiter = GetRowDelimiter(parserAttributes);

            for (int i = 0; i < sudokuGrid.Length; i++)
            {
                char digit = sudokuGrid[i] == 0 ? emptyDigit : (char)('0' + sudokuGrid[i]);
                output.Append(digit);
                if (emptyDigit != Char.MinValue)
                    output.Append(delimiter);
                if ((i+1) % 9 == 0)
                    output.Append(rowDelimiter);
            }
            return output.ToString();
        }



        static string ParseGridList(int[] sudokuGrid, ParserAttributes parserAttributes)
        {
            var output = new StringBuilder();
            char delimiter = GetDelimiter(parserAttributes);
            char emptyDigit = GetEmptyDigit(parserAttributes);

            for(int i = 0; i < sudokuGrid.Length; i++)
            {
                char digit = sudokuGrid[i] == 0 ? emptyDigit : (char)('0' + sudokuGrid[i]);
                output.Append(digit);
                if(emptyDigit != Char.MinValue)
                    output.Append(delimiter);
            }
            return output.ToString();
        }
    }
}
