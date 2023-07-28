using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace api.Services.SudokuCvSolver
{
    [Route(@"api/[controller]/[action]")]

    public class SudokuCvSolverController : Controller
    {
        [HttpGet]
        public FileContentResult FromImage(IFormFile originalImage)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public FileStreamResult FromVideo()
        {
            throw new NotImplementedException();
        }
    }
}
