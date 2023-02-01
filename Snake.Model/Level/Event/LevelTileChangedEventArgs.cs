namespace Snake.Model
{
    public class LevelTileChangedEventArgs : EventArgs
    {
        #region Public Properties

        public Level Level { get { return _level; } }
        public ITile? OldTile { get { return _oldTile; } }
        public ITile? NewTile { get { return _newTile; } }

        #endregion

        #region Private Members

        private readonly Level _level;
        private readonly ITile? _oldTile;
        private readonly ITile? _newTile;

        #endregion

        #region Constructors

        internal LevelTileChangedEventArgs(Level level, ITile? oldTile, ITile? newTile)
        {
            _level = level;
            _oldTile = oldTile;
            _newTile = newTile;
        }

        #endregion
    }
}
