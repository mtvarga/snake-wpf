namespace Snake.Model
{
    public class Game
    {
        #region Singleton Instance

        public static Game Instance { get { return _instance; } }
        private readonly static Game _instance = new();

        #endregion

        #region Public Properties

        public GameState State
        {
            get
            {
                return _state;
            }

            private set
            {
                GameState oldState = _state;
                _state = value;

                StateChanged?.Invoke(this, new(this, oldState, _state));
            }
        }

        public Level? Level { get; private set; }
        public Snake? Snake { get; private set; }

        #endregion

        #region Events & Delegates

        public delegate void StateChangedHandler(object sender, GameStateChangedEventArgs e);
        public event StateChangedHandler? StateChanged;

        #endregion

        #region Private Members

        private GameState _state;

        #endregion

        #region Constructors

        private Game() => _state = GameState.PreInitialized;

        #endregion

        #region Public Methods

        public void Initialize(Level? level = null)
        {
            if (State is GameState.Running)
                throw new IllegalGameOperationException("Attempted to reinitialize a level that is running. Pause or end the level before reinitializing using the Pause() or End() methods.");

            if (level == null && State is GameState.PreInitialized)
                throw new IllegalGameOperationException("Attempted to initialize a game without a level. Call the Initialize() method with a level provided as parameter.");

            if (level != null)
            {
                _state = GameState.PreInitialized;
                Level = level;
            }

            Level?.Clear();
            ResetSnake();

            State = GameState.Initialized;
        }

        public void Begin()
        {
            if (State is not (GameState.Initialized or GameState.Paused))
                throw new IllegalGameOperationException("Attempted to begin a level that has not been fully initialized yet or has already ended. Initialize the level using the Initialize() method, and if the level has already ended, use Clear() to remove the dead Snake from the level, and use Initialize() afterwards to reinitialize the level. If you want to wipe the level completely clear of obstacles, use the SetEmpty() method instead of the Clear() method.");

            State = GameState.Running;
        }

        public void Pause()
        {
            if (State is not GameState.Running)
                throw new IllegalGameOperationException("Attempting to pause a level that is not running. You can only pause a level if it has been started using the Begin() method.");

            State = GameState.Paused;
        }

        public void End() => State = GameState.Ended;

        public void MoveSnake()
        {
            if (State is not GameState.Running)
                throw new IllegalGameOperationException("Attempted to move Snake while level is not running. Start the level with the Begin() method before you want to move the Snake.");

            Snake?.Move();
        }

        public void TurnSnake(Direction d)
        {
            if (State is GameState.PreInitialized or GameState.Ended)
                throw new IllegalGameOperationException("Attempted to turn Snake on a level that has not been initialized yet or has already ended.");

            Snake?.Turn(d);
        }

        #endregion

        #region Private Helper Methods

        private void ResetSnake()
        {
            int middle = Convert.ToInt32(Math.Floor(Level!.Dimension / 2.0)) - 1;
            FieldTile head = FieldTile.ConstructSnakeTile(Level, -1, middle);

            Snake = new(Level, head, Direction.Right);
            Snake.SnakeDied += SnakeDied;
        }

        #endregion

        #region Event Handlers

        private void SnakeDied(object sender, SnakeDiedEventArgs e) => End();

        #endregion
    }

    public enum GameState
    {
        PreInitialized, // nincsen pálya betöltve
        Initialized,    // pálya betöltve és létrehoztunk egy kígyót, a játék készen áll az indulásra
        Running,        // a játék éppen zajlik
        Paused,         // a játék szüneteltetve lett
        Ended           // a játéknak vége

        // Az állapotok egymás után következése:
        //                                        ---------------
        //                                       \|/            |
        // PreInitialized ---> Initialized ---> Running ---> Paused ---> Ended ---> Initialized
        //                                        |            /|\        /|\
        //                                        ---------------          |
        //                                        |                        |
        //                                        --------------------------
    }
}
