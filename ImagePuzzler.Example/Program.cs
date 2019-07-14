using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace ImagePuzzler.Example
{
    class Program
    {
        private const string ResultsPath = "puzzles";

        static void Main(string[] args)
        {
            Console.WriteLine("Hi! Enter, please, path to image from which you want to make a puzzle");

            string imagePath;
            Bitmap inputImageRaw = null;
            while (!File.Exists(imagePath = Console.ReadLine()) || !TryOpenImage(imagePath, out inputImageRaw))
            {
                Console.WriteLine("That's not valid path to image, try again");
            }

            Console.WriteLine("Enter, X-split count and Y-split count");

            int xSplitCount;
            while (!int.TryParse(Console.ReadLine(), out xSplitCount))
            {
                Console.WriteLine("That's not a valid integer, try again");
            }

            int ySplitCount;
            while (!int.TryParse(Console.ReadLine(), out ySplitCount))
            {
                Console.WriteLine("That's not a valid integer, try again");
            }

            var stopWatch = Stopwatch.StartNew();
            var puzzlePieces = inputImageRaw.MakePuzzle(xSplitCount, ySplitCount);
            Console.WriteLine($"Puzzle algorithm duration: {stopWatch.Elapsed}");

            if (!Directory.Exists(ResultsPath))
            {
                Directory.CreateDirectory(ResultsPath);
            }

            var currentResultsPath = Path.Combine(ResultsPath, Path.GetFileNameWithoutExtension(imagePath));
            if (!Directory.Exists(currentResultsPath))
            {
                Directory.CreateDirectory(currentResultsPath);
            }

            var oldFiles = Directory.GetFiles(currentResultsPath);
            Parallel.For(0, oldFiles.Length, i =>
            {
                File.Delete(oldFiles[i]);
            });

            Console.WriteLine("Saving results...");
            Parallel.For(0, puzzlePieces.Length, i =>
            {
                var column = puzzlePieces[i];
                for (var j = 0; j < column.Length; j++)
                {
                    var puzzlePiece = puzzlePieces[i][j];
                    puzzlePiece.Image.Save(
                        Path.Combine(currentResultsPath, $"{j * puzzlePieces.Length + i + 1}.png"),
                        ImageFormat.Png);
                }
            });

            Console.WriteLine($"Puzzle successful. Total duration: {stopWatch.Elapsed}");
        }

        private static bool TryOpenImage(string path, out Bitmap image)
        {
            image = null;

            try
            {
                image = new Bitmap(path);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
