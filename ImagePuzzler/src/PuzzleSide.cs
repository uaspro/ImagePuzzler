namespace ImagePuzzler
{
    public class PuzzleSide
    {
        public PuzzlePiece PuzzlePiece { get; }

        public PuzzleConnectorType ConnectorType { get; }

        public PuzzleSide(PuzzlePiece puzzlePiece, PuzzleConnectorType connectorType)
        {
            PuzzlePiece = puzzlePiece;
            ConnectorType = connectorType;
        }

        public enum PuzzleConnectorType
        {
            Inbound,
            Outbound
        }
    }
}
