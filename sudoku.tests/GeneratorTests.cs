using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sudoku;
using Sudoku.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sudoku.tests
{
    [TestClass]
    public class GeneratorTests
    {
        [TestMethod]
        public void MakeAPuzzle()
        {
            // Arrange
            var puzzle = SudokuGenerator.MakePuzzle();

            // Act
            Assert.IsTrue(SudokuSolver.IsValid(puzzle));
            Assert.IsTrue(SudokuSolver.IsFinished(puzzle));
        }
    }
}
