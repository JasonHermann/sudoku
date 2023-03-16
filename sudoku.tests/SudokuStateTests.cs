using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sudoku;

namespace sudoku.tests
{
    [TestClass]
    public class SudokuStateTests
    {
        [TestMethod]
        public void SetupBoardStateWorks()
        {
            // Arrange

            // Act
            var board = new SudokuState();

            // Assert
            Assert.IsNotNull(board);
            Assert.AreEqual(true, board.IsValid);
            Assert.AreEqual(false, board.IsFinished);
        }

        [TestMethod]
        public void SetupBoard_AddNumbers_StateWorks()
        {
            // Arrange
            var board = new SudokuState();

            // Act
            board.Set(0, 0, 1);
            board.Set(0, 1, 2);
            board.Set(0, 2, 3);
            board.Set(0, 3, 4);
            board.Set(0, 4, 5);
            board.Set(0, 5, 6);
            board.Set(0, 6, 7);
            board.Set(0, 7, 8);
            board.Set(0, 8, 9);

            board.Set(3, 0, 4);
            board.Set(4, 0, 5);
            board.Set(5, 0, 6);
            board.Set(6, 0, 7);
            board.Set(7, 0, 8);
            board.Set(8, 0, 9);

            // Assert
            Assert.IsNotNull(board);
            Assert.AreEqual(true, board.IsValid);
            Assert.AreEqual(false, board.IsFinished);
        }

        [TestMethod]
        public void SetupBoard_AddNumbers_Invalid_StateWorks()
        {
            // Arrange
            var board = new SudokuState();

            // Act
            board.Set(0, 0, 1);
            board.Set(1, 0, 1);

            // Assert
            Assert.IsNotNull(board);
            Assert.AreEqual(false, board.IsValid);
        }

        [TestMethod]
        public void SetupBoard_SolvedPuzzle_IsValid()
        {
            // Arrange
            // Act
            // 6 7 4 1 3 5 9 8 2
            // 2 5 1 9 8 7 6 4 3
            // 8 9 3 4 2 6 1 7 5
            // 5 4 6 2 7 8 3 1 9
            // 1 3 7 5 4 9 2 6 8
            // 9 2 8 3 6 1 4 5 7
            // 7 6 9 8 1 3 5 2 4
            // 3 8 2 6 5 4 7 9 1
            // 4 1 5 7 9 2 8 3 6
            var input = new string[]
            {
                "6 7 4 1 3 5 9 8 2",
                "2 5 1 9 8 7 6 4 3",
                "8 9 3 4 2 6 1 7 5",
                "5 4 6 2 7 8 3 1 9",
                "1 3 7 5 4 9 2 6 8",
                "9 2 8 3 6 1 4 5 7",
                "7 6 9 8 1 3 5 2 4",
                "3 8 2 6 5 4 7 9 1",
                "4 1 5 7 9 2 8 3 6",
            };
            var board = new SudokuState(input);
            Assert.IsNotNull(board);
            Assert.AreEqual(true, board.IsValid);
            Assert.AreEqual(true, board.IsFinished);
            Assert.AreEqual(true, board.IsSolved);


            // Act
            // 2 9 1 3 6 8 4 5 7
            // 3 8 4 7 5 9 6 2 1
            // 5 6 7 4 2 1 9 8 3 
            // 6 3 8 5 1 7 2 9 4
            // 4 7 9 2 3 6 5 1 8
            // 1 2 5 8 9 4 3 7 6
            // 7 1 3 9 4 2 8 6 5 
            // 9 4 6 1 8 5 7 3 2
            // 8 5 2 6 7 3 1 4 9
            input = new string[]
            {
                "2 9 1 3 6 8 4 5 7",
                "3 8 4 7 5 9 6 2 1",
                "5 6 7 4 2 1 9 8 3",
                "6 3 8 5 1 7 2 9 4",
                "4 7 9 2 3 6 5 1 8",
                "1 2 5 8 9 4 3 7 6",
                "7 1 3 9 4 2 8 6 5",
                "9 4 6 1 8 5 7 3 2",
                "8 5 2 6 7 3 1 4 9",
            };
            board = new SudokuState(input);
            Assert.IsNotNull(board);
            Assert.AreEqual(true, board.IsValid);
            Assert.AreEqual(true, board.IsFinished);
            Assert.AreEqual(true, board.IsSolved);
        }

    }
}