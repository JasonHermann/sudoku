using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Services.ImageRecognition
{
    [Route(@"api/[controller]/[action]")]

    public class ImageRecognitionController : Controller
    {
        [HttpGet]
        public RecognizeDigitModel RecognizeDigit(IFormFile imageFile)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public RecognizeGridModel RecognizeGrid(IFormFile imageFile)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public AnalyzeGridModel AnalyzeGrid(IFormFile imageFile)
        {
            throw new NotImplementedException();
        }
    }
}
