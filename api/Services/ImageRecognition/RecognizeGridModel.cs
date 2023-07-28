using System.Drawing;

namespace api.Services.ImageRecognition
{


    public class RecognizeGridModel
    {
        public bool WasGridFound { get; set; }
        public Point TopLeft { get; set; }
        public Point TopRight { get; set; }
        public Point BottomLeft { get; set; }
        public Point BottomRight { get; set; }

        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }


    }
}
