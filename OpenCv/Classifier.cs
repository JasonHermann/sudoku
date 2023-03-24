using Keras.Models;
using Numpy;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tensorflow.NumPy;
using Tesseract;
using static Tensorflow.Binding;

namespace OpenCv
{
    public static class Classifier
    {
        public static TesseractEngine Engine = new TesseractEngine(@"../D:\Assets\sudoku\tessdata\", "eng", EngineMode.Default)
        {
            DefaultPageSegMode = PageSegMode.SingleChar,
        };
        public static int Classify(Mat image)
        {
            var preprocess = SudokuVision.PreProcess(image);
            Cv2.ImShow("Image", preprocess);
            Cv2.ImWrite(@"D:\trash\test.bmp", preprocess);
            var bytes = preprocess.ToBytes();

            using (var img = Pix.LoadFromMemory(bytes))
            {
                using (var page = Engine.Process(img, PageSegMode.SingleChar))
                {
                    var text = page.GetText();
                    if (text != null && text != "")
                    {
                        if (text.Contains("i") || text.Contains("l") || text.Contains("I") || text.Contains("|") || text.Contains("'") || text.Contains("!")
                            || text.Contains("L") || text.Contains(")") || text.Contains("("))
                            return 1;
                        if (text.Contains("S") || text.Contains("s"))
                            return 5;
                        if (text.Contains("+") || text.Contains("?"))
                            return 7;
                        if (text.Contains("B") || text.Contains("%"))
                            return 8;
                        if (text.Contains("q"))
                            return 9;
                        if (text.Contains("H") || text.Contains("Y"))
                            return 4;
                        if (text.Contains("b"))
                            return 6;
                        if (int.TryParse(text, out int digit))
                            return digit;
                        return 0;
                    }
                }
            }
            return 0;
        }
        
        public static NDarray Convert(Mat m)
        {
            var bytes = m.ToBytes();
            var numpy_array = Numpy.np.array(bytes);
            return numpy_array;
        }
    }
}
