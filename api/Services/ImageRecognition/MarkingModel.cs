using System.Drawing;

namespace api.Services.ImageRecognition
{
    public enum MarkingType
    {
        Nothing,
        Annotation,
        Solution,
    }

    [Flags]
    public enum Flourish
    {
        Color = 1 << 0,
        Strikethrough = 1 << 1,
        Annotation = 1 << 2,
        Irregular = 1 << 3,
        Erasure = 1 << 4,
        GridLine = 1 << 5,
        Handwritten = 1 << 6,
        Printed = 1 << 7,
    }

    public class MarkingModel
    {
        public int Color { get; set; }
        public Flourish Flourish { get; set; }
        public MarkingType Type { get; set; }
        public Point TopLeft { get; set; }
        public Point BottomRight { get; set; }
        public string? Content { get; set; }
    }
}
