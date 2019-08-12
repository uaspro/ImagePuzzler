using System.Drawing;

namespace ImagePuzzler
{
    public class Puzzle
    {
        public Bitmap Image { get; set; }

        public int XSplitCount { get; set; }

        public int YSplitCount { get; set; }

        public PuzzlePiece[][] PuzzlePieces { get; set; }
    }
}
