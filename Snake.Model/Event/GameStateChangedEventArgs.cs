namespace Snake.Model
{
    public class GameStateChangedEventArgs: EventArgs
    {
        #region Public Properties

        public Game Game { get { return _game; } }
        public GameState OldState { get { return _oldState; } }
        public GameState NewState { get { return _newState; } }

        #endregion

        #region Private Members

        private readonly Game _game;
        private readonly GameState _oldState;
        private readonly GameState _newState;

        #endregion

        #region Constructors

        public GameStateChangedEventArgs(Game game, GameState oldState, GameState newState)
        {
            _game = game;
            _oldState = oldState;
            _newState = newState;
        }

        #endregion
    }
}
