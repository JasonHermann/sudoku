using Sudoku;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sudoku.tests
{
    /// <summary>
    /// Sourced From Sudoku Puzzle Book
    /// </summary>
    public static class Puzzles
    {
        public static readonly string Template_Unsolved = "- - - - - - - - - " +
                                                          "- - - - - - - - - " +
                                                          "- - - - - - - - - " +
                                                          "- - - - - - - - - " +
                                                          "- - - - - - - - - " +
                                                          "- - - - - - - - - " +
                                                          "- - - - - - - - - " +
                                                          "- - - - - - - - - " +
                                                          "- - - - - - - - -";
        public static readonly string Template_Solution  = "- - - - - - - - - " +
                                                           "- - - - - - - - - " +
                                                           "- - - - - - - - - " +
                                                           "- - - - - - - - - " +
                                                           "- - - - - - - - - " +
                                                           "- - - - - - - - - " +
                                                           "- - - - - - - - - " +
                                                           "- - - - - - - - - " +
                                                           "- - - - - - - - -";

        public static readonly ParserAttributes Puzzle51Attributes = ParserAttributes.List |
                                                                    ParserAttributes.NumberDelimitedBySpace |
                                                                    ParserAttributes.EmptyAsDash;


        public static readonly string Puzzle51_Unsolved = "- 3 - 7 9 1 4 - - " +
                                                          "- - - 2 - - - - 7 " +
                                                          "7 - 4 6 - - 2 1 - " +
                                                          "- - 9 - - 8 6 2 5 " +
                                                          "4 - - - - - - - 1 " +
                                                          "6 5 8 1 - - 7 - - " +
                                                          "- 6 1 - - 7 8 - 4 " +
                                                          "8 - - - - 2 - - - " +
                                                          "- - 7 8 6 5 - 9 -";
        public static readonly string Puzzle51_Solution = "5 3 2 7 9 1 4 6 8 " +
                                                          "9 1 6 2 8 4 5 3 7 " +
                                                          "7 8 4 6 5 3 2 1 9 " +
                                                          "1 7 9 3 4 8 6 2 5 " +
                                                          "4 2 3 5 7 6 9 8 1 " +
                                                          "6 5 8 1 2 9 7 4 3 " +
                                                          "2 6 1 9 3 7 8 5 4 " +
                                                          "8 9 5 4 1 2 3 7 6 " +
                                                          "3 4 7 8 6 5 1 9 2";

        public static readonly ParserAttributes Puzzle51Attributes_Line = ParserAttributes.List |
                                                            ParserAttributes.NumberNotDelimited |
                                                            ParserAttributes.EmptyAsZero;

        public static readonly string Puzzle51_Unsolved_Line ="030791400"+"000200007"+"704600210"+"009008625"+"400000001"+"658100700"+"061007804"+"800002000"+"007865090";
        public static readonly string Puzzle51_Solution_Line ="532791468"+"916284537"+"784653219"+"179348625"+"423576981"+"658129743"+"261937854"+"895412376"+"347865192";


        public static readonly ParserAttributes Puzzle119Attributes = ParserAttributes.List |
                                                                    ParserAttributes.NumberDelimitedBySpace |
                                                                    ParserAttributes.EmptyAsSpace;

        public static readonly string Puzzle119_Unsolved = "  9   2     5 3   " +
                                                           "  4   9       8 6 " +
                                                           "      1       4 9 " +
                                                           "        7   4 6   " +
                                                           "  8   3   5   9   " +
                                                           "  7 2   6         " +
                                                           "7 6       1       " +
                                                           "9 5       3   1   " +
                                                           "  2 1     8   7  ";
        public static readonly string Puzzle119_Solution = "1 9 6 2 8 4 5 3 7 " +
                                                           "2 4 5 9 3 7 1 8 6 " +
                                                           "8 3 7 1 5 6 2 4 9 " +
                                                           "5 1 9 8 7 2 4 6 3 " +
                                                           "6 8 4 3 1 5 7 9 2 " +
                                                           "3 7 2 4 6 9 8 5 1 " +
                                                           "7 6 3 5 4 1 9 2 8 " +
                                                           "9 5 8 7 2 3 6 1 4 " +
                                                           "4 2 1 6 9 8 3 7 5";


        public static readonly ParserAttributes Puzzle108Attributes = ParserAttributes.List |
                                                                    ParserAttributes.NumberDelimitedBySpace |
                                                                    ParserAttributes.EmptyAsZero;
        public static readonly string Puzzle108_Unsolved = "1 0 5 2 0 6 0 0 0 " +
                                                           "7 0 0 0 0 0 0 2 0 " +
                                                           "0 0 6 4 7 0 0 0 0 " +
                                                           "0 2 8 0 0 0 0 0 9 " +
                                                           "6 1 0 0 0 0 0 3 5 " +
                                                           "3 0 0 0 0 0 1 4 0 " +
                                                           "0 0 0 0 4 8 6 0 0 " +
                                                           "0 5 0 0 0 0 0 0 4 " +
                                                           "0 0 0 9 0 7 3 0 1";
        public static readonly string Puzzle108_Solution = "1 8 5 2 3 6 4 9 7 " +
                                                           "7 4 3 8 9 1 5 2 6 " +
                                                           "2 9 6 4 7 5 8 1 3 " +
                                                           "5 2 8 3 1 4 7 6 9 " +
                                                           "6 1 4 7 8 9 2 3 5 " +
                                                           "3 7 9 5 6 2 1 4 8 " +
                                                           "9 3 7 1 4 8 6 5 2 " +
                                                           "8 5 1 6 2 3 9 7 4 " +
                                                           "4 6 2 9 5 7 3 8 1";
    }
}
