using api.Services.ImageRecognition;
using api.Services.SudokuSolver;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Services.ImageManipulation
{
    [Route(@"api/[controller]/[action]")]
    public class ImageManipulationController : Controller
    {
        /// <summary>
        /// Given an image and a corresponding grid model, it extracts the grid from the image
        /// and returns a perspective transformed image.
        /// </summary>
        [HttpGet]
        public FileContentResult ExtractGrid(IFormFile originalImage, RecognizeGridModel gridModel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Given a transformed image and the result of analysing that image for digits, borders, markings
        /// it draws the provided board (solved or partial solved) on top the image.
        /// </summary>
        [HttpGet]
        public FileContentResult AugumentSolutionToGridImage(IFormFile gridImage, AnalyzeGridModel analyzeGridModel, BoardModel board )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Given a augmented board (ideally solved, but possibly modified in any way), it replaces the original board in the
        /// source image with this augmented board.
        /// </summary>
        [HttpGet]
        public FileContentResult ApplySolutionToOriginalImage(IFormFile originalImage, IFormFile imageToApply, RecognizeGridModel gridModel)
        {
            throw new NotImplementedException();
        }
    }
}
