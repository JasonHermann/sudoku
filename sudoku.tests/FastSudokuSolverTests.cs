using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sudoku;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sudoku.tests
{
    [TestClass]
    public class FastSudokuSolverTests
    {
        [TestMethod]
        public void Initialize_Works()
        {
            // Arrange
            var sudoku = new FastSudoku();

            // Act
            sudoku.SetByRowColumn(0, 6, Constants.Values[2]);
            sudoku.SetByRowColumn(4, 5, Constants.Values[8]);
            sudoku.SetByRowColumn(6, 1, Constants.Values[7]);
            sudoku.SetByRowColumn(8, 8, Constants.Values[3]);
            // --------   --------   --------
            //          |          | 2        |
            //          |          |          |
            //          |          |          |
            // --------   --------   --------
            //          |          |          |
            //          |        8 |          |
            //          |          |          |
            // --------   --------   --------
            //     7    |          |          |
            //          |          |          |
            //          |          |        3 |
            // --------   --------   --------

            // Assert
            Assert.AreEqual(2, sudoku.Cells[0 * 9 + 6]);
            Assert.AreEqual(128, sudoku.Cells[4 * 9 + 5]);
            Assert.AreEqual(64, sudoku.Cells[6 * 9 + 1]);
            Assert.AreEqual(4, sudoku.Cells[8 * 9 + 8]);

        }
    
    
        [TestMethod]
        public void MakeCopies_IsFast()
        {
            // Arrange
            var parsed = Parser.GridFromString(Puzzles.Puzzle119_Unsolved, Puzzles.Puzzle119Attributes);
            var sudoku = new FastSudoku(parsed);

            // Act
            var copy = new FastSudoku(sudoku);
            copy.SetByCell(0, 1);
            sudoku.SetByCell(0, 2);
            for(int i = 0; i < 1000; i++)
            {
                var test = new FastSudoku(sudoku);
            }

            // Asset
            Assert.AreNotSame(copy, sudoku);
            Assert.AreEqual(1, copy.Cells[0]);
            Assert.AreEqual(2, sudoku.Cells[0]);
        }

    }
}
