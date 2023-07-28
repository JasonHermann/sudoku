using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Services.SudokuSolver
{
    [Route(@"api/[controller]/[action]")]

    public class SudokuSolverController : Controller
    {
        private Service _service;
        public SudokuSolverController() 
        {
            _service = new Service();
        }

        [HttpGet]
        public string CreateBoard(string difficulty, string mask)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public string SolveBoard(string board)
        {
            return _service.Solve(board);
        }

        [HttpGet]
        public ClueModel[] GiveClues(string board, int clues)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public ClueModel GiveClue(string board)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public ClueModel GiveClueForCell(string board, int row, int column)
        {
            throw new NotImplementedException();
        }
    }
}
