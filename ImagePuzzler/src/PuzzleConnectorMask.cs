using Svg;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using static ImagePuzzler.PuzzlePiece;

namespace ImagePuzzler
{
    public class PuzzleConnectorMask : IDisposable
    {
        private const string ResourcesPath = "resources";
        private const string PuzzleConnectorFileName = "puzzle-connector.png";

        private Dictionary<PuzzleSideType, Bitmap> _puzzleConnectorMasks = new Dictionary<PuzzleSideType, Bitmap>();
        private Dictionary<PuzzleSideType, BitmapData> _puzzleConnectorMasksData = new Dictionary<PuzzleSideType, BitmapData>();

        internal BitmapData TopData =>
            _puzzleConnectorMasksData.ContainsKey(PuzzleSideType.Top)
                ? _puzzleConnectorMasksData[PuzzleSideType.Top]
                : null;

        internal BitmapData RightData =>
            _puzzleConnectorMasksData.ContainsKey(PuzzleSideType.Right)
                ? _puzzleConnectorMasksData[PuzzleSideType.Right]
                : null;

        internal BitmapData BottomData =>
            _puzzleConnectorMasksData.ContainsKey(PuzzleSideType.Bottom)
                ? _puzzleConnectorMasksData[PuzzleSideType.Bottom]
                : null;

        internal BitmapData LeftData =>
            _puzzleConnectorMasksData.ContainsKey(PuzzleSideType.Left)
                ? _puzzleConnectorMasksData[PuzzleSideType.Left]
                : null;

        internal Rectangle Rect { get; }

        internal PuzzleConnectorMask(int width, int height)
        {
            var maskBitmapRaw = new Bitmap(Path.Combine(ResourcesPath, PuzzleConnectorFileName));
            var maskBitmapLeft = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            maskBitmapRaw.DrawTo(maskBitmapLeft, width: width, height: height);
            _puzzleConnectorMasks.Add(PuzzleSideType.Left, maskBitmapLeft);

            var maskBitmapTop = (Bitmap)maskBitmapLeft.Clone();
            maskBitmapTop.RotateFlip(RotateFlipType.Rotate90FlipNone);
            _puzzleConnectorMasks.Add(PuzzleSideType.Top, maskBitmapTop);

            var maskBitmapRight = (Bitmap)maskBitmapLeft.Clone();
            maskBitmapRight.RotateFlip(RotateFlipType.RotateNoneFlipX);
            _puzzleConnectorMasks.Add(PuzzleSideType.Right, maskBitmapRight);

            var maskBitmapBottom = (Bitmap)maskBitmapTop.Clone();
            maskBitmapBottom.RotateFlip(RotateFlipType.RotateNoneFlipY);
            _puzzleConnectorMasks.Add(PuzzleSideType.Bottom, maskBitmapBottom);

            Rect = new Rectangle(0, 0, width, height);

            LockBits();
        }

        public void LockBits()
        {
            foreach (var puzzleConnectorMask in _puzzleConnectorMasks)
            {
                _puzzleConnectorMasksData.Add(
                    puzzleConnectorMask.Key,
                    puzzleConnectorMask.Value.LockBits(Rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb));
            }
        }

        public void Dispose()
        {
            foreach (var puzzleConnectorMask in _puzzleConnectorMasks)
            {
                if (_puzzleConnectorMasksData.ContainsKey(puzzleConnectorMask.Key))
                {
                    var bits = _puzzleConnectorMasksData[puzzleConnectorMask.Key];
                    puzzleConnectorMask.Value.UnlockBits(bits);
                }

                puzzleConnectorMask.Value.Dispose();
            }

            _puzzleConnectorMasks.Clear();
            _puzzleConnectorMasksData.Clear();
        }
    }
}
