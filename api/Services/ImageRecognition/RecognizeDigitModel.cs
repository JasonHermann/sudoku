using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace api.Services.ImageRecognition
{
    public class RecognizeDigitModel
    {
        public bool WasDigitFound { get; set; }
        public string? Digit { get; set; }
        public Point TopLeft { get; set; }
        public Point TopRight { get; set; }
        public Point BottomLeft { get; set; }
        public Point BottomRight { get; set; }
    }
}
