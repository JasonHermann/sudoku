using OpenCvSharp;
using Sudoku;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCv
{
    public static class SudokuVision
    {
        public static Mat? ReturnBoardFromImage(string pathToFile, out Mat? perspectiveMatrix)
        {
            Mat originalImage = new Mat(pathToFile);
            Mat gray = MakeGray(originalImage);
            Mat blurred = GaussianBlur(gray);
            Mat blackWhite = MakeAdaptiveThreshold(blurred);
            var contour = FindSudokuGrid(blackWhite);
            if (contour == null)
            {
                perspectiveMatrix = null;
                return null;
            }
            //Mat extracted = Extract(originalImage, contour);
            Mat? perspective = PerspectiveDistortion(originalImage, contour, out perspectiveMatrix);
            return perspective;
        }
        public static Mat PreProcess(Mat image)
        {
            Mat gray = MakeGray(image);
            Mat blackWhite = MakeAdaptiveThreshold(gray);
            return blackWhite;
        }

        static Mat MakeGray(Mat image)
        {
            if (image.Channels() == 1)
                return image;
            return image.CvtColor(ColorConversionCodes.BGR2GRAY);
        }

        static Mat MakeAdaptiveThreshold(Mat image)
        {
            if (image.Channels() != 1)
                image = MakeGray(image);
            return image.AdaptiveThreshold(255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, 21, 8);
        }

        static Mat GaussianBlur(Mat image)
        {
            return image.GaussianBlur(new Size(5, 5), 0, 0);
        }



        static Mat? FindSudokuGrid(Mat image)
        {
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

                // It has to look like a square.
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

        static Mat? PerspectiveDistortion(Mat originalImage, Mat imageToExtract, out Mat perspectiveMatrix)
        {
            var mc = imageToExtract.MinAreaRect();
            var polygon = imageToExtract.ApproxPolyDP(0.01 * imageToExtract.ArcLength(true), true);
            var mc2 = polygon.MinAreaRect();

            var points = polygon.ConvexHullPoints();

            var targetSize = new Size(mc2.Size.Width, mc2.Size.Height);
            var source = new Point2f[4]
            {
                new Point2f(points[0].X, points[0].Y),
                new Point2f(points[1].X, points[1].Y),
                new Point2f(points[2].X, points[2].Y),
                new Point2f(points[3].X, points[3].Y),
            };
            var destination = new Point2f[4]
            {
                new Point2f(0, 0),
                new Point2f(targetSize.Width, 0),
                new Point2f(targetSize.Width, targetSize.Height),
                new Point2f(0, targetSize.Height),
            };

            
            perspectiveMatrix = Cv2.GetPerspectiveTransform(source, destination);
            var output = originalImage.WarpPerspective(perspectiveMatrix, targetSize);
            return output;
        }


        public static Mat ReturnGridFromImage(Mat extractedImage, Mat perspectiveMatrix)
        {
            var cells = FindGridCells(extractedImage);
            if (cells == null)
                return extractedImage;

            var contours = FindDigitContours(extractedImage, cells);
            var digits = new int[81];

            for(int i =0; i < 81; i++)
            {
                var gridCell = cells[i];
                var contour = contours[i];

                if(contour != null)
                {
                    var boundary = contour.BoundingRect();
                    var center = new Point((boundary.Left + boundary.Right) / 2, (boundary.Top + boundary.Bottom) / 2);
                    var slightlyBigger = new Rect(
                        Math.Max(0,Math.Min(boundary.Left, center.X - gridCell.Width / 3)),
                        Math.Max(0, Math.Min(boundary.Top, center.Y - gridCell.Height / 3)),
                        Math.Max(boundary.Width,  (int) (gridCell.Width / 1.5)),
                        Math.Max(boundary.Height, (int) (gridCell.Height / 1.5)));
                    var sub = extractedImage.SubMat(slightlyBigger);
                    digits[i] = Classifier.Classify(sub);
                    if(digits[i] > 0)
                    {
                        Cv2.Rectangle(extractedImage, slightlyBigger, new Scalar(0, 128, 0), 2);
                    }
                    else
                    {
                        Cv2.Rectangle(extractedImage, slightlyBigger, new Scalar(0, 0, 255), 2);
                    }
                }
            }

            var puzzle = new FastSudoku(digits);
            var beforeSolvedCells = puzzle.SolvedCells();
            puzzle = SudokuSolver.SolveAllCells(puzzle);
            var afterSolvedCells = puzzle.SolvedCells();
            
            if(afterSolvedCells > beforeSolvedCells)
            {
                for(int i = 0; i< 81; i++)
                {
                    var answer = Sudoku.Constants.DigitLookup[puzzle.Cells[i]];
                    if(answer != 0 && digits[i] == 0) // We have a value, but we didn't have a starting digit
                    {
                        // Draw the answer!
                        var r = cells[i];
                        Cv2.PutText(extractedImage, answer.ToString(), 
                            new Point(r.Left + r.Width * 0.25, r.Bottom - r.Height * 0.25), 
                            HersheyFonts.HersheyPlain, 
                            3,
                            new Scalar(128, 128, 0), 1);
                        Cv2.Rectangle(extractedImage, r, new Scalar(128, 128, 0), 3);
                    }
                    else if(answer != 0)
                    {
                        //var r = cells[i];
                        //Cv2.PutText(extractedImage, answer.ToString(),
                        //    new Point(r.Left + r.Width * 0.25, r.Bottom - r.Height * 0.25),
                        //    HersheyFonts.HersheyPlain,
                        //    3,
                        //    new Scalar(67, 64, 10), 1);
                    }
                }
            }

            return extractedImage;
        }
        static Rect[]? FindGridCells(Mat modifiedGridOnlyImage)
        {
            // 81 of 83 for sudoku, 2 of 83 for edges
            // This is a totally made up idea.
            var startingX = 0;
            var startingY = 0;
            var width = modifiedGridOnlyImage.Width / 9;
            var height = modifiedGridOnlyImage.Height / 9;
            var output = new Rect[81];
            for (int i = 0; i < 81; i++)
            {
                var rect = new Rect(startingX + width * (i % 9), startingY + height * (i / 9), width, height);
                output[i] = rect;
            }
            return output;
        }

        static Mat[] FindDigitContours(Mat extractedImage, Rect[] cells)
        {
            var output = new Mat[81];
            int[] scores = new int[81];

            var processed = PreProcess(extractedImage);

            Mat[] contours;
            Mat placeHolder = new Mat();
            OutputArray hierarchy = OutputArray.Create(placeHolder);
            processed.FindContours(out contours, hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);

            var contoursIn8 = 0;
            foreach(var contour in contours)
            {
                var r = contour.BoundingRect();
                var a = contour.ContourArea();

                if(cells[32].Contains(r))
                {
                    // This is an 8!
                    contoursIn8++;
                }
                for(int i =0; i < cells.Length; i++)
                {
                    var c = cells[i];
                    var score = ContourScore(c, r, a);
                    if(score > scores[i] || (score == scores[i] && output[i] != null && contour.ArcLength(true) > output[i].ArcLength(true)))
                    {
                        scores[i] = score;
                        output[i] = contour;
                        break; // if a contour has a best score, then it at least had a positive score, which means it was inside this cell
                               // and if it is inside this cell, then it can't be inside any other cell.
                    }
                }
            }
            return output;
        }

        static int ContourScore(Rect gridCell, Rect boundingRectangle, double contourArea)
        {
            // Contour should be inside the grid cell;
            // Contour should be centered relative to the cell
            // Contour area shouldn't be too small or too large (i.e. the perimeter of the grid or a small scribble)
            var r = boundingRectangle;
            if (gridCell.Contains(r))
            {
                var cellArea = gridCell.Width * gridCell.Height;
                var packing = Math.Abs(contourArea) / cellArea;

                var midPointR = new Point((r.Left + r.Right) / 2, (r.Top + r.Bottom) / 2);
                var midPointG = new Point((gridCell.Left + gridCell.Right) / 2, (gridCell.Top + gridCell.Bottom) / 2);
                var relativeXDeviationFromCenter = Math.Abs(midPointR.X - midPointG.X) / (gridCell.Width / 2.00);
                var relativeYDeviationFromCenter = Math.Abs(midPointR.Y - midPointG.Y) / (gridCell.Width / 2.00);

                // If the center of the contour is extremely close to the edge of the cell, then it probably is not a number
                // but an artifact of a grid line.
                if (relativeXDeviationFromCenter > 0.7 || relativeYDeviationFromCenter > 0.7)
                    return 0;
                if (packing < 0.01)
                    return 0;
                var score = 0;
                score += (packing < 0.05 || packing > 0.95) ? 0 : 1;
                score += (packing < 0.10 || packing > 0.90) ? 0 : 1;
                score += (packing < 0.15 || packing > 0.85) ? 0 : 1;
                score += (packing < 0.20 || packing > 0.80) ? 0 : 1;
                score += (relativeXDeviationFromCenter > 0.5) ? 0 : 1;
                score += (relativeXDeviationFromCenter > 0.25) ? 0 : 1;
                score += (relativeYDeviationFromCenter > 0.5) ? 0 : 1;
                score += (relativeYDeviationFromCenter > 0.25) ? 0 : 1;
                return score;
            }
            return 0;
        }
    }
}
