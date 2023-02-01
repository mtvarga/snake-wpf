namespace Snake.Model
{
    public interface ITile
    {
        #region Properties

        Level? Level { get; }
        int X { get; }
        int Y { get; }

        #endregion

        #region Methods

        void Visit(Snake snake);

        bool IsField();
        bool IsEgg();
        bool IsObstacle();
        bool IsBound();

        #endregion
    }
}
