using Snake.Persistence;

namespace Snake.Model
{
    public class Level
    {
        #region Public Properties

        public int Dimension { get { return _dimension; } }
        public int ObstacleCount { get; private set; } = 0;

        #endregion

        #region Events & Delegates

        public delegate void TileChangedHandler(object sennder, LevelTileChangedEventArgs e);
        public event TileChangedHandler? TileChanged;

        #endregion

        #region Private Members

        private readonly int _dimension;
        private readonly ITile?[,] _tiles;

        #endregion

        #region Constructors

        public Level(int dimension)
        {
            _dimension = dimension;
            _tiles = new ITile[dimension, dimension];

            SetEmpty();
        }

        #endregion

        #region Public Methods

        public void SetEmpty()
        {
            for (int i = 0; i < _dimension; i++)
                for (int j = 0; j < _dimension; j++)
                    SetEmptyTile(i, j);

            ObstacleCount = 0;
        }

        public void Clear()
        {
            for (int i = 0; i < _dimension; i++)
            {
                for (int j = 0; j < _dimension; j++)
                {
                    ITile? tile = _tiles[i, j];

                    if (tile != null && (tile.IsEgg() || (tile.IsField() && ((FieldTile)tile).IsSnake)))
                        SetEmptyTile(i, j);
                }
            }
        }

        public ITile? Tile(int x, int y) => ((x >= 0 && x < _dimension) && (y >= 0 && y < _dimension)) ? _tiles[x, y] : null;
        public ITile?[] AdjacentTiles(int x, int y) => new ITile?[] { Tile(x - 1, y), Tile(x, y - 1), Tile(x + 1, y), Tile(x, y + 1) };
        public bool IsVisible(ITile tile) => tile.X >= 0 && tile.X < _dimension && tile.Y >= 0 && tile.Y < _dimension;

        public FieldTile SetEmptyTile(int x, int y)
        {
            if (x >= _dimension || y >= _dimension)
                throw new LevelTileOutOfBoundsException(_dimension, x, y);

            ITile? oldTile = _tiles[x, y];

            if (oldTile?.IsObstacle() ?? false)
                ObstacleCount--;

            FieldTile newTile = FieldTile.ConstructEmptyTile(this, x, y);

            _tiles[x, y] = newTile;

            TileChanged?.Invoke(this, new(this, oldTile, newTile));

            return newTile;
        }

        public FieldTile SetSnakeTile(int x, int y)
        {
            if (x >= _dimension || y >= _dimension)
                throw new LevelTileOutOfBoundsException(_dimension, x, y);

            ITile? oldTile = _tiles[x, y];

            if (oldTile?.IsObstacle() ?? false)
                ObstacleCount--;

            FieldTile newTile = FieldTile.ConstructSnakeTile(this, x, y);

            _tiles[x, y] = newTile;

            TileChanged?.Invoke(this, new(this, oldTile, newTile));

            return newTile;
        }

        public ObstacleTile SetObstacleTile(int x, int y)
        {
            if (x >= _dimension || y >= _dimension)
                throw new LevelTileOutOfBoundsException(_dimension, x, y);

            ITile? oldTile = _tiles[x, y];

            if (!(oldTile?.IsObstacle() ?? false))
            {
                ObstacleTile newTile = new(this, x, y);

                _tiles[x, y] = newTile;

                TileChanged?.Invoke(this, new(this, oldTile, newTile));

                ObstacleCount++;

                return newTile;
            }

            return (ObstacleTile)oldTile;
        }

        public EggTile SetEggTile(int x, int y)
        {
            if (x >= _dimension || y >= _dimension)
                throw new LevelTileOutOfBoundsException(_dimension, x, y);

            ITile? oldTile = _tiles[x, y];

            if (oldTile?.IsObstacle() ?? false)
                ObstacleCount--;

            EggTile newTile = new(this, x, y);

            _tiles[x, y] = newTile;

            TileChanged?.Invoke(this, new(this, oldTile, newTile));

            return newTile;
        }

        public EggTile PlaceRandomEgg()
        {
            Random random = new(DateTime.Now.Millisecond);
            int x, y;

            // A ciklusra azért van szükségünk, mert ha véletlen egy olyan mezőre generálunk random egy tojást, amely pont akadály vagy a kígyó rajta van, akkor keresnünk kell egy másik szabad mezőt, ahova a tojást le tudjuk rakni.
            do
            {
                x = random.Next(0, _dimension - 1);
                y = random.Next(0, _dimension - 1);

            } while (!_tiles[x, y]!.IsField() || ((FieldTile)_tiles[x, y]!).IsSnake);

            return SetEggTile(x, y);
        }

        public LevelDesign Design()
        {
            LevelDesign design = new(_dimension);

            for (int i = 0; i < _dimension; i++)
                for (int j = 0; j < _dimension; j++)
                    if (_tiles[i, j]?.IsObstacle() ?? false)
                        design.AddObstacleLocation(i, j);

            return design;
        }

        #endregion

        #region Internal Methods

        internal BoundTile GetBoundTile(int x, int y) => new(this, x, y);

        #endregion

        #region Factory Functions

        public static Level FromDesign(LevelDesign design)
        {
            Level level = new(design.Size);
            design.ForEachObstacleLocation(((int X, int Y) location) => level.SetObstacleTile(location.X, location.Y));
            return level;
        }

        public static Level Random(int size, int obstacleCount)
        {
            Level level = new(size);
            level.SetEmpty();

            int x, y;
            Random random = new(DateTime.Now.Millisecond);

            for (int i = 0; i < obstacleCount; i++)
            {
                x = random.Next(0, level.Dimension - 1);
                y = random.Next(0, level.Dimension - 1);

                level.SetObstacleTile(x, y);
            }

            return level;
        }

        #endregion
    }
}
