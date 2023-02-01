namespace Snake.Model
{
    public class IllegalGameOperationException : Exception
    {
        #region Constructors

        public IllegalGameOperationException(string message)
            : base(message) { }

        #endregion
    }
}
