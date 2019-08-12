using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace ImagePuzzler
{
    public static class PuzzleMaker
    {
        private static Random Random = new Random();

        private static int DefaultConnectorScale = 8;

        public static void SetRandom(Random random)
        {
            Random = random;
        }

        public static void SetDefaultConnectorScale(int scale)
        {
            DefaultConnectorScale = scale;
        }

        public static Puzzle MakePuzzle(this Bitmap inputImageRaw, int xSplitCount, int ySplitCount)
        {
            var result = new Puzzle
            {
                PuzzlePieces = new PuzzlePiece[xSplitCount][]
            };

            for (var i = 0; i < result.PuzzlePieces.Length; i++)
            {
                result.PuzzlePieces[i] = new PuzzlePiece[ySplitCount];
            }

            var puzzlePieceWidth = (int)Math.Floor(inputImageRaw.Width / (decimal)xSplitCount);
            var puzzlePieceHeight = (int)Math.Floor(inputImageRaw.Height / (decimal)ySplitCount);
            result.XSplitCount = xSplitCount;
            result.YSplitCount = ySplitCount;

            var puzzlePieceRect = new Rectangle(0, 0, puzzlePieceWidth, puzzlePieceHeight);

            var largerSideSize = puzzlePieceWidth > puzzlePieceHeight ? puzzlePieceWidth : puzzlePieceHeight;
            using (var puzzleConnectorMask =
                new PuzzleConnectorMask(
                    largerSideSize / DefaultConnectorScale,
                    largerSideSize / DefaultConnectorScale))
            {

                var inputImageRect = new Rectangle(0, 0, xSplitCount * puzzlePieceRect.Width, ySplitCount * puzzlePieceRect.Height);
                var inputImage = inputImageRaw.Clone(inputImageRect, PixelFormat.Format32bppArgb);
                result.Image = inputImage;

                for (var i = 0; i < xSplitCount; i++)
                {
                    for (var j = 0; j < ySplitCount; j++)
                    {
                        var slicePuzzlePieceRect =
                            new Rectangle(i * puzzlePieceRect.Width, j * puzzlePieceRect.Height, puzzlePieceRect.Width, puzzlePieceRect.Height);

                        var slicePuzzlePieceBitmap = inputImage.Clone(slicePuzzlePieceRect, inputImage.PixelFormat);

                        var puzzlePieceBitmap =
                            new Bitmap(
                                puzzlePieceRect.Width + puzzleConnectorMask.Rect.Width * 2,
                                puzzlePieceRect.Height + puzzleConnectorMask.Rect.Height * 2,
                                PixelFormat.Format32bppArgb);

                        slicePuzzlePieceBitmap.DrawTo(
                            puzzlePieceBitmap,
                            new Point(
                                puzzleConnectorMask.Rect.Width,
                                puzzleConnectorMask.Rect.Height),
                            slicePuzzlePieceRect.Width,
                            slicePuzzlePieceRect.Height);

                        var puzzlePiece = new PuzzlePiece(puzzlePieceBitmap, puzzleConnectorMask);
                        result.PuzzlePieces[i][j] = puzzlePiece;
                    }
                }

                Parallel.For(0, xSplitCount, i =>
                {
                    for (var j = 0; j < ySplitCount; j++)
                    {
                        var puzzlePiece = result.PuzzlePieces[i][j];
                        if (i != 0)
                        {
                            var puzzleConnectorType = Random.Next(2) > 0
                                ? PuzzleSide.PuzzleConnectorType.Inbound
                                : PuzzleSide.PuzzleConnectorType.Outbound;

                            var leftPuzzlePiece = result.PuzzlePieces[i - 1][j];
                            puzzlePiece.AddUpdatePuzzleSide(
                                PuzzlePiece.PuzzleSideType.Left,
                                new PuzzleSide(
                                    leftPuzzlePiece,
                                    (i - 1, j),
                                    puzzleConnectorType));

                            leftPuzzlePiece.AddUpdatePuzzleSide(
                                PuzzlePiece.PuzzleSideType.Right,
                                new PuzzleSide(
                                    puzzlePiece,
                                    (i, j),
                                    puzzleConnectorType == PuzzleSide.PuzzleConnectorType.Inbound
                                        ? PuzzleSide.PuzzleConnectorType.Outbound
                                        : PuzzleSide.PuzzleConnectorType.Inbound));
                        }

                        if (j != 0)
                        {
                            var puzzleConnectorType = Random.Next(2) > 0
                                ? PuzzleSide.PuzzleConnectorType.Inbound
                                : PuzzleSide.PuzzleConnectorType.Outbound;

                            var topPuzzlePiece = result.PuzzlePieces[i][j - 1];
                            puzzlePiece.AddUpdatePuzzleSide(
                                PuzzlePiece.PuzzleSideType.Top,
                                new PuzzleSide(
                                    topPuzzlePiece,
                                    (i, j - 1),
                                    puzzleConnectorType));

                            topPuzzlePiece.AddUpdatePuzzleSide(
                                PuzzlePiece.PuzzleSideType.Bottom,
                                new PuzzleSide(
                                    puzzlePiece,
                                    (i, j),
                                    puzzleConnectorType == PuzzleSide.PuzzleConnectorType.Inbound
                                        ? PuzzleSide.PuzzleConnectorType.Outbound
                                        : PuzzleSide.PuzzleConnectorType.Inbound));
                        }
                    }
                });
            }

            return result;
        }
    }
}
