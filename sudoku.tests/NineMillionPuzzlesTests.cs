using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sudoku;
using System;
using System.Collections.Generic;
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
                    var answer = SudokuSolver.SolveAllCells(sudoku);


                    var fail = !SudokuSolver.IsValid(answer) || !SudokuSolver.IsFinished(answer) ||
                                 81 != SudokuSolver.SolvedCells(answer);
                    if (fail) failCount++;
                }
            }

            Assert.AreEqual(0, failCount);
            Assert.AreEqual(9000000, count);
        }
    }
}
