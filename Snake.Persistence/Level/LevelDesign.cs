namespace Snake.Persistence
{
    public class LevelDesign
    {
        #region Public Properties

        public int Size { get { return _size; } }
        public int ObstacleCount { get; private set; }

        #endregion

        #region Private Members

        private readonly int _size;
        private readonly LinkedList<(int X, int Y)> _obstacleLocations;

        #endregion

        #region Constructors

        public LevelDesign(int size)
        {
            _size = size;
            _obstacleLocations = new();
        }

        #endregion

        #region Public Methods

        public void AddObstacleLocation(int x, int y)
        {
            _obstacleLocations.AddLast((x, y));
            ObstacleCount++;
        }

        public void RemoveObstacleLocation(int x, int y)
        {
            if (_obstacleLocations.Remove((x, y)))
                ObstacleCount--;
        }

        public void ForEachObstacleLocation(Action<(int X, int Y)> action) => _obstacleLocations.ToList().ForEach(action);

        #endregion
    }
}
