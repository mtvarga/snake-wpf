namespace Snake.Model
{
    public class FieldTile: ITile
    {
        #region Public Properties

        public Level Level => _level;
        public int X => _x;
        public int Y => _y;
        public bool IsSnake { get; internal set; }

        #endregion

        #region Private Members

        private readonly Level _level;
        private readonly int _x;
        private readonly int _y;

        #endregion

        #region Constructors

        private FieldTile(Level level, int x, int y, bool isSnake)
        {
            _level = level;
            _x = x;
            _y = y;
            IsSnake = isSnake;
        }

        #endregion

        #region Public Methods

        public void Visit(Snake snake) => snake.Hit(this);

        public bool IsField() => true;
        public bool IsEgg() => false;
        public bool IsObstacle() => false;
        public bool IsBound() => false;

        #endregion

        #region Factory Functions

        public static FieldTile ConstructEmptyTile(Level level, int x, int y) => new(level, x, y, false);
        public static FieldTile ConstructSnakeTile(Level level, int x, int y) => new(level, x, y, true);

        #endregion
    }
}
