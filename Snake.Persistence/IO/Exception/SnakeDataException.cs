namespace Snake.Persistence
{
    public class SnakeDataException : Exception
    {
        #region Public Properties

        public override string Message
        {
            get
            {
                return $"An exception has occured while trying to save/load a Snake level: {InnerException?.Message}";
            }
        }

        #endregion

        #region Constructors

        public SnakeDataException(Exception? innerException)
            : base(null, innerException) { }

        #endregion
    }
}
