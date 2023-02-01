namespace Snake.Model
{
    public class LevelTileOutOfBoundsException : Exception
    {
        #region Public Properties

        override public string Message =>
            $"Tile's coordinates were sout of bounds. Expected x and y values between 0 and {_max - 1}. Got: x = {_x}, y = {_y}.";

        #endregion

        #region Private Members

        private readonly int _x, _y, _max;

        #endregion

        #region Constructors

        public LevelTileOutOfBoundsException(int max, int x, int y)
        {
            _x = x;
            _y = y;
            _max = max;
        }

        #endregion
    }
}
