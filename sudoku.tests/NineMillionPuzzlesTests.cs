using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sudoku;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sudoku.tests
{
    [TestClass]
    public class NineMillionPuzzlesTests
    {
        /// <summary>
        /// This file is expected to be a multiline, two-column CSV file.
        /// Header row is puzzle,solution
        /// Rows are 81 digits + comma + 81 digits.
        /// </summary>
        /// <remarks>
        /// You can find a huge file of 9,000,000 puzzles and solutions here: 
        /// https://www.kaggle.com/datasets/rohanrao/sudoku
        /// </remarks>
        public string FilePath = Path.Combine(@"D:\assets\open\sudoku\", "sudoku.csv");

        /// <summary>
        /// https://codegolf.stackexchange.com/questions/190727/the-fastest-sudoku-solver
        /// </summary>
        public string FilePath_Stackoverflow = Path.Combine(@"D:\assets\open\sudoku\", "sudoku.txt");

        [TestInitialize]
        public void Initialize()
        {
            // Assert file exists
            var exists = File.Exists(FilePath);

            if (!exists)
                Assert.Inconclusive("File does not exist.");
        }


        [TestMethod]
        public void RunTests()
        {
            var count = 0;
            var failCount = 0;
            // Arrange
            using (var stream = new StreamReader(FilePath, new FileStreamOptions() { Access = FileAccess.Read, Mode = FileMode.Open }))
            {
                // First line of CSV
                if (!stream.EndOfStream)
                {
                    var header = stream.ReadLine();
                    Assert.AreEqual("puzzle,solution", header, "Header is not as expected.");
                }
                else
                {
                    Assert.Fail("File contains no header");
                }

                // Act on each row
                while (!stream.EndOfStream)
                {
                    var line = stream.ReadLine();
                    count++;
                    Assert.IsNotNull(line);

                    var values = line.Split(",");
                    var puzzle = values[0];
                    var solution = values[1];

                    var digits = puzzle.ToCharArray().Select(x => (int)x - '0').ToArray();
                    var sudoku = new FastSudoku(digits);
                    var answers = SudokuSolver.FindAllSolutions(sudoku).Take(1);

                    if (answers != null && answers.Any())
                    {
                        var answer = answers.FirstOrDefault();
                        var fail = !SudokuSolver.IsValid(answer) || !SudokuSolver.IsFinished(answer) ||
                                     81 != SudokuSolver.SolvedCells(answer);
                        if (fail)
                            failCount++;

                        var solutionString = new String(answer.Cells.Select(x => (char)('0' + Constants.DigitLookup[x])).ToArray());
                        Assert.AreEqual(solution, solutionString);
                    }
                    else
                        failCount++;
                }
            }

            Assert.AreEqual(0, failCount);
            Assert.AreEqual(9000000, count);
        }

        [TestMethod]
        public void TestRecursiveSearch()
        {
            // Arrange
            var puzzle   = "000002534000010280200034000020000740906000300140203000708000001300009600460070803";
            var solution = "671892534534617289289534176823961745956748312147253968798326451315489627462175893";

            // Act
            var digits = puzzle.ToCharArray().Select(x => (int)x - '0').ToArray();
            var sudoku = new FastSudoku(digits);
            var answers = SudokuSolver.FindAllSolutions(sudoku).Take(1);

            Assert.IsNotNull(answers);
            Assert.IsTrue(answers.Any());
            var answer = answers.FirstOrDefault();
            var fail = !SudokuSolver.IsValid(answer) || !SudokuSolver.IsFinished(answer) ||
                         81 != SudokuSolver.SolvedCells(answer);

            // Assert
            Assert.IsFalse(fail);
        }


        [TestMethod]
        public void FindASolution_EmptyGrid()
        {
            // Arrange
            var puzzle   = "000000000000000000000000000000000000000000000000000000000000000000000000000000000";

            // Act
            var digits = puzzle.ToCharArray().Select(x => (int)x - '0').ToArray();
            var sudoku = new FastSudoku(digits);

            var watch = Stopwatch.StartNew();
            var answers = SudokuSolver.FindAllSolutions(sudoku).Take(1000);

            Assert.IsNotNull(answers);
            Assert.IsTrue(answers.Any());
            var answer = answers.Skip(999).FirstOrDefault();
            var fail = !SudokuSolver.IsValid(answer) || !SudokuSolver.IsFinished(answer) ||
                         81 != SudokuSolver.SolvedCells(answer);
            watch.Stop();

            // Assert
            Assert.IsFalse(fail);
        }

        [TestMethod]
        public void RunTests_StackOverflow()
        {
            var count = 0;
            var failCount = 0;
            var times = new List<long>();

            // Arrange
            using (var stream = new StreamReader(FilePath_Stackoverflow, new FileStreamOptions() { Access = FileAccess.Read, Mode = FileMode.Open }))
            {
                // First line of CSV
                if (!stream.EndOfStream)
                {
                    var header = stream.ReadLine(); // Number of puzzles.
                }
                else
                {
                    Assert.Fail("File contains no header");
                }

                var watch = new Stopwatch();

                // Act on each row
                while (!stream.EndOfStream)
                {
                    count++;
                    var line = stream.ReadLine();
                    var digits = line.ToCharArray().Select(x => (int)x - '0').ToArray();
                    watch.Start();
                    var sudoku = new FastSudoku(digits);
                    var answers = SudokuSolver.FindAllSolutions(sudoku).Take(1).ToList();
                    watch.Stop();
                    times.Add(watch.ElapsedMilliseconds);
                    if (answers != null && answers.Any())
                    {
                        var answer = answers.FirstOrDefault();
                        var fail = !SudokuSolver.IsValid(answer) || !SudokuSolver.IsFinished(answer) ||
                                     81 != SudokuSolver.SolvedCells(answer);

                        if (fail)
                            failCount++;
                    }
                    else
                        failCount++;
                }
            }

            var maxTime = times.Max();
            Debug.Print(maxTime.ToString());
            Assert.AreEqual(0, failCount);
        }



        /// <summary>
        /// This is incredibly slow.
        /// </summary>
        //[TestMethod]
        //public void TestRecursiveSearch_SimplerImplementation()
        //{
        //    // Arrange
        //    var puzzle = "000002534000010280200034000020000740906000300140203000708000001300009600460070803";
        //    var solution = "671892534534617289289534176823961745956748312147253968798326451315489627462175893";

        //    // Act
        //    var digits = puzzle.ToCharArray().Select(x => (byte)((byte)x - '0')).ToArray();
        //    var sudoku = new SudokuPuzzle(digits);
        //    var answers = SudokuSolver.FindAllSolutions(sudoku).Take(1);

        //    Assert.IsNotNull(answers);
        //    Assert.IsTrue(answers.Any());
        //    var answer = answers.FirstOrDefault();
        //    Assert.IsNotNull(answer);
        //    var fail = !answer.IsSolved;

        //    // Assert
        //    Assert.IsFalse(fail);
        //}


    }
}
