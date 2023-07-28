namespace api.Services.ImageRecognition
{
    public class CellModel
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public int InternalWidth { get; set; }
        public int InternalHeight { get; set; }
        public MarkingModel[] Markings { get; set; }

    }
}
