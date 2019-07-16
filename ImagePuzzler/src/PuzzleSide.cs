namespace ImagePuzzler
{
    public class PuzzleSide
    {
        public PuzzlePiece PuzzlePiece { get; }

        public int XIndex { get; }

        public int YIndex { get; }

        public PuzzleConnectorType ConnectorType { get; }

        public PuzzleSide(PuzzlePiece puzzlePiece, (int x, int y) pieceIndeces,  PuzzleConnectorType connectorType)
        {
            PuzzlePiece = puzzlePiece;
            XIndex = pieceIndeces.x;
            YIndex = pieceIndeces.y;
            ConnectorType = connectorType;
        }

        public enum PuzzleConnectorType
        {
            Inbound,
            Outbound
        }
    }
}
