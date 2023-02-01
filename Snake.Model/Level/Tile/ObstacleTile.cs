namespace Snake.Model
{
    public class ObstacleTile : ITile
    {
        #region Public Properties

        public Level Level => _level;
        public int X => _x;
        public int Y => _y;

        #endregion

        #region Private Members

        private readonly Level _level;
        private readonly int _x;
        private readonly int _y;

        #endregion

        #region Constructors

        internal ObstacleTile(Level level, int x, int y)
        {
            _level = level;
            _x = x;
            _y = y;
        }

        #endregion

        #region Public Methods

        public void Visit(Snake snake) => snake.Hit();

        public bool IsField() => false;
        public bool IsEgg() => false;
        public bool IsObstacle() => true;
        public bool IsBound() => false;

        #endregion
    }
}
