namespace Snake.Model
{
    public class SnakeGrewEventArgs: EventArgs
    {
        #region Public Properties

        public Snake Snake { get { return _snake; } }
        public int OldSize { get { return _oldSize; } }
        public int NewSize { get { return _newSize; } }

        #endregion

        #region Private Members

        private readonly Snake _snake;
        private readonly int _oldSize;
        private readonly int _newSize;

        #endregion

        #region Constructors

        public SnakeGrewEventArgs(Snake snake, int oldSize, int newSize)
        {
            _snake = snake;
            _oldSize = oldSize;
            _newSize = newSize;
        }

        #endregion
    }
}
