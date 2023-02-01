namespace Snake.Model
{
    public class SnakeDiedEventArgs: EventArgs
    {
        #region Public Properties

        public Snake Snake { get { return _snake; } }

        #endregion

        #region Private Members

        private readonly Snake _snake;

        #endregion

        #region Constructors

        public SnakeDiedEventArgs(Snake snake)
        {
            _snake = snake;
        }

        #endregion
    }
}
