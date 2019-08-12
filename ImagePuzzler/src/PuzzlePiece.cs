using System.Drawing;
using System.Drawing.Imaging;

namespace ImagePuzzler
{
    public class PuzzlePiece
    {
        private readonly PuzzleConnectorMask _puzzleConnectorMask;

        public Bitmap Image { get; }

        public Rectangle ImageRect { get; }

        public PuzzleSide[] PuzzleSides { get; } = new PuzzleSide[4];

        internal PuzzlePiece(Bitmap image, PuzzleConnectorMask puzzleConnectorMask)
        {
            Image = image;
            ImageRect = new Rectangle(0, 0, Image.Width, Image.Height);

            _puzzleConnectorMask = puzzleConnectorMask;
        }

        internal void AddUpdatePuzzleSide(PuzzleSideType puzzleSideType, PuzzleSide puzzleSide)
        {
            PuzzleSides[(int) puzzleSideType] = puzzleSide;

            var targetExtract = puzzleSide.ConnectorType == PuzzleSide.PuzzleConnectorType.Inbound
                ? Image
                : puzzleSide.PuzzlePiece.Image;

            var targetInsert = puzzleSide.ConnectorType == PuzzleSide.PuzzleConnectorType.Inbound
                ? puzzleSide.PuzzlePiece.Image
                : Image;

            BitmapData connectorMaskData = null;
            var maskX = 0;
            var maskY = 0;
            switch (puzzleSideType)
            {
                case PuzzleSideType.Right:
                    if (puzzleSide.ConnectorType == PuzzleSide.PuzzleConnectorType.Inbound)
                    {
                        connectorMaskData = _puzzleConnectorMask.LeftData;
                        maskX = ImageRect.Width - _puzzleConnectorMask.Rect.Width * 2;
                        maskY = ImageRect.Height / 2 - _puzzleConnectorMask.Rect.Height / 2;
                    }
                    else
                    {
                        connectorMaskData = _puzzleConnectorMask.RightData;
                        maskX = _puzzleConnectorMask.Rect.Width;
                        maskY = ImageRect.Height / 2 - _puzzleConnectorMask.Rect.Height / 2;
                    }

                    break;
                case PuzzleSideType.Bottom:
                    if (puzzleSide.ConnectorType == PuzzleSide.PuzzleConnectorType.Inbound)
                    {
                        connectorMaskData = _puzzleConnectorMask.TopData;
                        maskX = ImageRect.Width / 2 - _puzzleConnectorMask.Rect.Width / 2;
                        maskY = ImageRect.Height - _puzzleConnectorMask.Rect.Height * 2;
                    }
                    else
                    {
                        connectorMaskData = _puzzleConnectorMask.BottomData;
                        maskX = ImageRect.Width / 2 - _puzzleConnectorMask.Rect.Width / 2;
                        maskY = _puzzleConnectorMask.Rect.Height;
                    }

                    break;
            }

            if (connectorMaskData != null)
            {
                var insertMask =
                    targetExtract.ExtractMask(ImageRect, connectorMaskData, _puzzleConnectorMask.Rect, maskX, maskY);

                insertMask.DrawTo(
                    targetInsert,
                    new Point(
                        puzzleSideType == PuzzleSideType.Right
                            ? ImageRect.Width - maskX -
                                (puzzleSide.ConnectorType == PuzzleSide.PuzzleConnectorType.Inbound
                                    ? _puzzleConnectorMask.Rect.Width * 2
                                    : 0)
                            : maskX,
                        puzzleSideType == PuzzleSideType.Bottom
                            ? ImageRect.Height - maskY -
                                (puzzleSide.ConnectorType == PuzzleSide.PuzzleConnectorType.Inbound
                                    ? _puzzleConnectorMask.Rect.Height * 2
                                    : 0)
                            : maskY));
            }
        }

        public enum PuzzleSideType
        {
            Top,
            Right,
            Bottom,
            Left
        }
    }
}
