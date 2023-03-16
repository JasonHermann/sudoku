using OpenCv;
using OpenCvSharp;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.Serialization;

namespace OpenCV
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = new List<string>()
            {
                "sudoku1.jpg",
                "sudoku2.jpg",
                "sudoku3.jpg",
                "sudoku4.jpg",
                "sudoku5.jpg",
                "sudoku6.jpg",
                "sudoku7.jpg",
                "sudoku8.jpg",
                "sudoku9.jpg",
                "sudoku10.jpg",
                "sudoku11.jpg",
                "sudoku12.jpg",
                "sudoku13.jpg",
                "sudoku14.jpg",
                "sudoku15.jpg",
            };

            // Read the image file
            var index = 0;
            Mat color = new Mat(Path.Combine(@"D:\assets\sudoku\", files[index]));
            Mat? image = color;
            Mat? contour = null;
            Mat? perspectiveMatrix = null;
            RotatedRect rect= default;
            int key = 1;
            Cv2.NamedWindow("Image", WindowFlags.Normal);

            while (key != 'q')
            {
                Cv2.ImShow("Image", image);
                key = Cv2.WaitKey(0);
                switch(key)
                {
                    case 'a': index = (index - 1 + files.Count) % files.Count; color = new Mat(Path.Combine(@"D:\assets\sudoku\", files[index])); image = color; contour = default; break; // Left
                    case 'd': index = (index + 1) % files.Count; color = new Mat(Path.Combine(@"D:\assets\sudoku\", files[index])); image = color; contour = default; break; // Right
                    case 's': break; // Down
                    case 'w': break; // Up
                }
                if (key == 'c')
                { 
                    image = color;
                }
                else if (key == '9')
                {
                    image = SudokuVision.ReturnBoardFromImage(Path.Combine(@"D:\assets\sudoku\", files[index]), out perspectiveMatrix);
                }
                else if (key == 'b')
                    image = MakeGray(image);
                else if (key == 't')
                    image = MakeThreshold(image);
                else if (key == '1')
                {
                    var digit = Classifier.Classify(image);
                    Cv2.PutText(image, digit.ToString(), new Point(0, image.Height), HersheyFonts.HersheyPlain, 1, new Scalar(255, 255, 0), 2);
                }
                else if (key == 'p')
                    image = MakeAdaptiveThreshold(image);
                else if (key == 'g')
                    image = GaussianBlur(image);
                else if (key == 'h')
                    image = Canny(image);
                else if (key == '8')
                {
                    image = SudokuVision.ReturnGridFromImage(image, perspectiveMatrix);
                }
                else if (key == 'm')
                    image = Morphological(image);
                else if (key == '=' || key == '+')
                {
                    image = Rotate(image);
                }
                else if (key == 'r')
                {
                    var g = FindSudokuGrid(image);
                    if (g != null)
                    {
                        var contours = new Mat[1] { g };
                        color.DrawContours(contours, -1, new Scalar(0, 0, 255), 1, LineTypes.Link8);
                        image = color;
                        contour = g;
                    }
                }
                else if (key == '2')
                {
                    var g = FindAllContours(image);
                    color.DrawContours(g, -1, new Scalar(0, 0, 255), 1, LineTypes.Link8);
                    image = color;
                }
                else if (key == 'e')
                {
                    if (contour != null)
                    {
                        image = Extract(color, contour);
                    }
                }
            }
            // Wait for a key press and then exit
            Cv2.DestroyAllWindows();
        }



        private static Mat Rotate(Mat image)
        {
            var output = new Mat();
            Cv2.Rotate(image, output, RotateFlags.Rotate90Clockwise);
            return output;
        }

        static Mat MakeGray(Mat image)
        {
            if (image.Channels() == 1)
                return image;
            return image.CvtColor(ColorConversionCodes.BGR2GRAY);
        }

        static Mat MakeThreshold(Mat image)
        {
            if (image.Channels() != 1)
                image = MakeGray(image);
            return image.Threshold(50, 255, ThresholdTypes.Binary);
        }

        static Mat MakeAdaptiveThreshold(Mat image)
        {
            if (image.Channels() != 1)
                image = MakeGray(image);
            return image.AdaptiveThreshold(255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, 21,8);
        }

        static Mat GaussianBlur(Mat image)
        {
            return image.GaussianBlur(new Size(5, 5), 0, 0);
        }

        static Mat Canny(Mat image)
        {
            return image.Canny(100, 200);
        }

        static Mat Extract(Mat originalImage, Mat imageToExtract)
        {
            var boundingRectacngle = imageToExtract.BoundingRect();
            Mat interior = originalImage.SubMat(boundingRectacngle);

            var minimumRetangle = Cv2.MinAreaRect(imageToExtract);
            Mat rotationMatrix = Cv2.GetRotationMatrix2D(new Point2f(interior.Width / 2, interior.Height / 2), minimumRetangle.Angle, 1);
            var rotated = interior.WarpAffine(rotationMatrix, interior.Size());
            Size imageSize = new Size(minimumRetangle.Size.Width, minimumRetangle.Size.Height);
            Rect boundingRect = new Rect(new Point(0, 0), imageSize);
            var middle = new Point2f(rotated.Size().Width / 2, rotated.Size().Height / 2);
            var center = rotated.GetRectSubPix(imageSize, middle);
            return center;
       }

        static Mat Morphological(Mat image)
        {
            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3));
            Mat closedImage = new Mat();
            Cv2.MorphologyEx(image, closedImage, MorphTypes.Close, kernel);
            return closedImage;
        }

        static Mat? FindSudokuGrid(Mat image)
        {
            if (image.Channels() != 1)
                image = MakeGray(image);

            Mat[] contours;
            Mat placeHolder = new Mat();
            OutputArray hierarchy = OutputArray.Create(placeHolder);
            image.FindContours(out contours, hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);

            double maxArea = 0;
            RotatedRect maxRect = new RotatedRect();
            Mat maxContour = null;
            foreach (Mat contour in contours)
            {
                // approximately a square
                var perimeter = Cv2.ArcLength(contour, true);
                var area = Cv2.ContourArea(contour);
                double epsilon = 0.01 * perimeter;
                var approx = contour.ApproxPolyDP(epsilon, true);
                var error = (Math.Sqrt(area) - perimeter / 4.00) / area;
                //var error = 0.00;

                // It has to look like a square.c
                if (approx.Rows * approx.Cols == 4 && error <= 0.05)
                {
                    if (area > maxArea)
                    {
                        RotatedRect rect = Cv2.MinAreaRect(contour);
                        maxArea = area;
                        maxRect = rect;
                        maxContour = contour;
                    }
                }
                else
                {
                    // Does not look like a square.
                    maxArea = maxArea;
                }


            }
            return maxContour;
        }

        private static Mat[] FindAllContours(Mat image)
        {
            Assembly.GetExecutingAssembly();


            image = SudokuVision.PreProcess(image);

            Mat[] contours;
            Mat placeHolder = new Mat();
            OutputArray hierarchy = OutputArray.Create(placeHolder);
            image.FindContours(out contours, hierarchy, RetrievalModes.CComp, ContourApproximationModes.ApproxSimple);

            //double maxArea = 0;
            //RotatedRect maxRect = new RotatedRect();
            //Mat maxContour = null;
            //foreach (Mat contour in contours)
            //{
            //    // approximately a square
            //    var perimeter = Cv2.ArcLength(contour, true);
            //    var area = Cv2.ContourArea(contour);
            //    double epsilon = 0.01 * perimeter;
            //    var approx = contour.ApproxPolyDP(epsilon, true);
            //    var error = (Math.Sqrt(area) - perimeter / 4.00) / area;
            //    //var error = 0.00;

            //    // It has to look like a square.c
            //    if (approx.Rows * approx.Cols == 4 && error <= 0.05)
            //    {
            //        if (area > maxArea)
            //        {
            //            RotatedRect rect = Cv2.MinAreaRect(contour);
            //            maxArea = area;
            //            maxRect = rect;
            //            maxContour = contour;
            //        }
            //    }
            //    else
            //    {
            //        // Does not look like a square.
            //        maxArea = maxArea;
            //    }


            //}
            return contours;
        }



    }

}
