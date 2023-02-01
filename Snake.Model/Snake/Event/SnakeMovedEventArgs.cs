namespace Snake.Model
{
    public class SnakeMovedEventArgs: EventArgs
    {
        #region Public Properties

        public Snake Snake { get { return _snake; } }

        public FieldTile OldHead { get { return _oldHead; } }
        public FieldTile NewHead { get { return _newHead; } }

        public FieldTile OldTail { get { return _oldTail; } }
        public FieldTile NewTail { get { return _newTail; } }

        public Direction OldDirection { get { return _oldDirection; } }
        public Direction NewDirection { get { return _newDirection; } }

        #endregion

        #region Private Members

        private readonly Snake _snake;

        private readonly FieldTile _oldHead;
        private readonly FieldTile _newHead;

        private readonly FieldTile _oldTail;
        private readonly FieldTile _newTail;

        private readonly Direction _oldDirection;
        private readonly Direction _newDirection;

        #endregion

        #region Constructors

        internal SnakeMovedEventArgs(Snake snake, FieldTile oldHead, FieldTile newHead, FieldTile oldTail, FieldTile newTail, Direction oldDirection, Direction newDirection)
        {
            _snake = snake;
            _oldHead = oldHead;
            _newHead = newHead;
            _oldTail = oldTail;
            _newTail = newTail;
            _oldDirection = oldDirection;
            _newDirection = newDirection;
        }

        #endregion
    }
}
