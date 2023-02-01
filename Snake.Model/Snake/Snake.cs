namespace Snake.Model
{
    public class Snake
    {
        #region Constants

        private const int kDefaultStartingSize = 5;

        #endregion

        #region Public Properties

        public Direction Direction { get; private set; }
        public Level Level { get { return _level; } }
        public FieldTile Head { get; private set; }
        public FieldTile Tail { get { return _tiles.Peek(); } }
        public int Size { get; private set; }
        public bool IsAlive { get; private set; } = true;

        #endregion

        #region Events & Delegates

        public delegate void SnakeMovedHandler(object sender, SnakeMovedEventArgs e);
        public delegate void SnakeGrewHandler(object sender, SnakeGrewEventArgs e);
        public delegate void SnakeDiedHandler(object sender, SnakeDiedEventArgs e);

        public event SnakeMovedHandler? SnakeMoved;
        public event SnakeGrewHandler? SnakeGrew;
        public event SnakeDiedHandler? SnakeDied;

        #endregion

        #region Private Members

        private readonly Level _level;
        private readonly Queue<FieldTile> _tiles;

        private Direction? _oldDirection = null;

        #endregion

        #region Constructors

        internal Snake(Level level, FieldTile head, Direction initialDirection)
        {
            _level = level;
            _tiles = new Queue<FieldTile>();

            Head = head;
            _tiles.Enqueue(Head);

            for (int i = 0; i < kDefaultStartingSize - 1; i++)
            {
                ITile tile = _level.Tile(head.X - i, head.Y) ?? FieldTile.ConstructSnakeTile(level, head.X - i, head.Y);

                if (!tile.IsField() || !((FieldTile)tile).IsSnake)
                    tile = _level.SetSnakeTile(head.X - i, head.Y);                    

                _tiles.Enqueue((FieldTile)tile);
            }

            Size = kDefaultStartingSize;
            Direction = initialDirection;
        }

        #endregion

        #region Internal Methods

        internal void Move()
        {
            FieldTile oldHead = Head, oldTail = _tiles.Peek();

            ITile nextTile = Direction switch
            {
                Direction.Left => _level.Tile(Head.X - 1, Head.Y) ?? _level.GetBoundTile(Head.X - 1, Head.Y),
                Direction.Right => _level.Tile(Head.X + 1, Head.Y) ?? _level.GetBoundTile(Head.X + 1, Head.Y),
                Direction.Up => _level.Tile(Head.X, Head.Y - 1) ?? _level.GetBoundTile(Head.X, Head.Y - 1),
                Direction.Down => _level.Tile(Head.X, Head.Y + 1) ?? _level.GetBoundTile(Head.X, Head.Y + 1),
                _ => Head
            };

            nextTile.Visit(this);

            if (IsAlive)
            {
                SnakeMoved?.Invoke(this, new(this, oldHead, Head, oldTail, _tiles.Peek(), _oldDirection ?? Direction, Direction));

                if (_oldDirection != null)
                    _oldDirection = null;
            }
        }

        internal void Turn(Direction d)
        {
            _oldDirection = Direction;
            Direction = d;
        }

        internal void Kill()
        {
            IsAlive = false;
            SnakeDied?.Invoke(this, new(this));
        }

        internal void Hit(FieldTile t)
        {
            if (t.IsSnake)
            {
                if (t != Tail)
                    Kill();
            }
            else
                t.IsSnake = true;

            Head = t;
            _tiles.Enqueue(Head);

            _tiles.Dequeue().IsSnake = false;
        }

        internal void Hit(EggTile t)
        {
            Head = _level.SetSnakeTile(t.X, t.Y);
            _tiles.Enqueue(Head);

            Size++;

            SnakeGrew?.Invoke(this, new(this, Size - 1, Size));
        }

        internal void Hit() => Kill();

        #endregion
    }
}