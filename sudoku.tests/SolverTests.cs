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
    public class SolverTests
    {
        [TestMethod]
        public void IsValid_SolvedPuzzle()
        {
            // Arrange
            var parsed = Parser.GridFromString(Puzzles.Puzzle119_Solution, Puzzles.Puzzle119Attributes);
            var sudoku = new FastSudoku(parsed);

            // Act
            var isFinished = SudokuSolver.IsFinished(sudoku);
            var isValid = SudokuSolver.IsValid(sudoku);

            // Assert
            Assert.IsTrue(isFinished);
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void IsInValid_SolvedPuzzle()
        {
            // Arrange
            var parsed = Parser.GridFromString(Puzzles.Puzzle119_Solution, Puzzles.Puzzle119Attributes);
            var sudoku = new FastSudoku(parsed);

            // Act
            // Change One Value
            sudoku.SetByCell(1, Constants.Values[7]); // Normally this is a 9.

            var isFinished = SudokuSolver.IsFinished(sudoku);
            var isValid = SudokuSolver.IsValid(sudoku);

            // Assert
            Assert.IsTrue(isFinished);
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void IsValid_UnsolvedPuzzle_FindSolution()
        {
            // Arrange
            var parsed = Parser.GridFromString(Puzzles.Puzzle51_Unsolved, Puzzles.Puzzle51Attributes);
            var sudoku = new FastSudoku(parsed);

            // Act

            var solvedCells_Before = sudoku.SolvedCells();
            sudoku = SudokuSolver.SolveAllCells(sudoku);
            var solvedCells_After = sudoku.SolvedCells();
            var isFinished = SudokuSolver.IsFinished(sudoku);
            var isValid = SudokuSolver.IsValid(sudoku);

            // Assert
            Assert.IsTrue(isFinished);
            Assert.IsTrue(isValid);
            Assert.AreEqual(81, solvedCells_After);
        }

        [TestMethod]
        public void IsValid_UnsolvedPuzzle_FindSolution2()
        {
            // Arrange
            var parsed = Parser.GridFromString(Puzzles.Puzzle119_Unsolved, Puzzles.Puzzle119Attributes);
            var sudoku = new FastSudoku(parsed);

            // Act

            var solvedCells_Before = sudoku.SolvedCells();
            sudoku = SudokuSolver.SolveAllCells(sudoku);
            var solvedCells_After = sudoku.SolvedCells();
            var isFinished = SudokuSolver.IsFinished(sudoku);
            var isValid = SudokuSolver.IsValid(sudoku);

            // Assert
            Assert.IsTrue(isFinished);
            Assert.IsTrue(isValid);
            Assert.AreEqual(81, solvedCells_After);
        }

        [TestMethod]
        public void IsValid_UnsolvedPuzzle_FindSolution3()
        {
            // Arrange
            var parsed = Parser.GridFromString(Puzzles.Puzzle108_Unsolved, Puzzles.Puzzle108Attributes);
            var sudoku = new FastSudoku(parsed);

            // Act

            var solvedCells_Before = sudoku.SolvedCells();
            sudoku = SudokuSolver.SolveAllCells(sudoku);
            var solvedCells_After = sudoku.SolvedCells();
            var isFinished = SudokuSolver.IsFinished(sudoku);
            var isValid = SudokuSolver.IsValid(sudoku);

            // Assert
            Assert.IsTrue(isFinished);
            Assert.IsTrue(isValid);
            Assert.AreEqual(81, solvedCells_After);
        }
    }
}
