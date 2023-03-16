﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sudoku;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sudoku.tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void SimpleListOfNumbers()
        {
            // Arrange
            var puzzleSolution = Puzzles.Puzzle51_Solution;
            var puzzleUnsolved = Puzzles.Puzzle51_Unsolved;
            var attributes = Puzzles.Puzzle51Attributes;

            // Act
            int[] solutionGrid = Parser.GridFromString(puzzleSolution, attributes);
            int[] unsolvedGrid = Parser.GridFromString(puzzleUnsolved, attributes);

            // Assert
            Assert.AreEqual(81, solutionGrid.Length);
            Assert.AreEqual(81, unsolvedGrid.Length);
            Assert.AreEqual(2, solutionGrid[12]); // Row 2; Column 4
            Assert.AreEqual(2, unsolvedGrid[12]); // Row 2; Column 4
        }

        [TestMethod]
        public void PrintStringFromGrid()
        {
            // Arrange
            var puzzleSolution = Puzzles.Puzzle51_Solution;
            var puzzleUnsolved = Puzzles.Puzzle51_Unsolved;
            var attributes = Puzzles.Puzzle51Attributes;

            // Act
            int[] unsolvedGrid = Parser.GridFromString(puzzleUnsolved, attributes);
            string parsedString = Parser.StringFromGrid(unsolvedGrid, attributes).Trim();

            // Assert
            Assert.AreEqual(puzzleUnsolved, parsedString);
        }

    }
}
