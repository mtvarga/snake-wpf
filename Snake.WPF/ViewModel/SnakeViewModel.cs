using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Snake.Model;

namespace Snake.WPF.ViewModel
{
    public class SnakeViewModel : ViewModelBase
    {
        #region Constants

        private static readonly Color kFieldColorLight = Color.FromRgb(246, 215, 176);
        private static readonly Color kFieldColorDark = Color.FromRgb(242, 210, 169);

        #endregion

        #region Commands

        public DelegateCommand LoadBuiltinLevelCommand { get; private set; }
        public DelegateCommand LoadCustomLevelCommand { get; private set; }
        public DelegateCommand LoadRandomLevelCommand { get; private set; }
        public DelegateCommand AccelerateSnakeCommand { get; private set; }
        public DelegateCommand ExitGameCommand { get; private set; }

        public DelegateCommand TurnSnakeCommand { get; private set; }
        public DelegateCommand PlayPauseCommand { get; private set; }
        public DelegateCommand ResetLevelCommand { get; private set; }

        #endregion

        #region Events

        public EventHandler<BuiltinLevelType>? LoadBuiltinLevel;
        public EventHandler<EventArgs>? LoadCustomLevel;
        public EventHandler<EventArgs>? LoadRandomLevel;
        public EventHandler<bool>? AccelerateSnakeChanged;
        public EventHandler<EventArgs>? ExitGame;

        #endregion

        #region Public Static Properties

        public static bool LevelLoadMenuItemsEnabled => Game.Instance.State != GameState.Running;

        #endregion

        #region Public Properties

        public string HintLabelText => Game.Instance.State switch
        {
            GameState.PreInitialized => "Kérlek tölts be egy pályát a Fájl menüpont alatt!",
            GameState.Initialized => "(Space) Játék elkezdése",
            GameState.Running => "(Space) Játék szüneteltetése        \u2191, \u2193, \u2190, \u2192 Kígyó forgatása",
            GameState.Paused => "\u2191, \u2193, \u2190, \u2192 Játék folytatása (kígyó elforgatása adott irányba)        (R) Újrakezdés",
            GameState.Ended => $"Játék vége. {_eatenEggs} db tojást sikerült megenned.        (Space) Újrakezdés",
            _ => ""
        };

        public string GameTimeLabelText => $"Játékidő: {_elapsedTimeHours}:{_elapsedTimeMinutes % 60:D2}:{_elapsedTimeSeconds % 60:D2}";

        public string EatenEggsLabelText => $"Megevett tojások: {_eatenEggs}";

        public int Dimension => Level?.Dimension ?? 0;

        public ObservableCollection<TileBox> TileBoxes { get; private set; }

        #endregion

        #region Private Static Properties

        private static Level? Level => Game.Instance.Level;

        #endregion

        #region Private Members

        private int _eatenEggs = 0;
        private int _elapsedTimeSeconds = 0;
        private int _elapsedTimeMinutes = 0;
        private int _elapsedTimeHours = 0;

        private readonly Queue<(FieldTile Tile, Direction NewDirection)> _turns = new();
        private Direction? _tailDirection = null;
        private Direction _newDirection = Direction.Right;

        private bool _shouldGrowTexture = false;

        #endregion

        #region Constructors

        public SnakeViewModel()
        {
            LoadBuiltinLevelCommand = new((object? param) =>
            {
                if (param == null || param is not BuiltinLevelType)
                    throw new ArgumentException("A LoadBuiltinLevelCommmand must always have a BuiltinLevelType parameter that indicates which builtin level to load.");

                OnLoadBuiltinLevel((BuiltinLevelType)param);
            });

            LoadCustomLevelCommand = new(_ => OnLoadCustomLevel());
            LoadRandomLevelCommand = new(_ => OnLoadRandomLevel());
            AccelerateSnakeCommand = new((object? param) =>
            {
                if (param == null || param is not bool)
                    throw new ArgumentException("An AccelerateSnakeCommand must always have a boolean parameter which indicates whether the snake should accelerate when it eats an egg.");

                OnAccelerateSnakeChanged((bool)param);
            });

            ExitGameCommand = new(_ => OnExitGame());

            TurnSnakeCommand = new((object? param) =>
            {
                if (param == null || param is not Direction)
                    throw new ArgumentException("A TurnSnakeCommand must always have a Direction parameter which indicates the direction in which the snake should turn.");

                OnTurnSnake((Direction)param);
            });

            PlayPauseCommand = new(_ => OnPlayPause());
            ResetLevelCommand = new(_ => OnResetLevel());

            TileBoxes = new();

            Game.Instance.StateChanged += Game_StateChanged;
        }

        #endregion

        #region Public Methods

        public void MoveSnake()
        {
            Game.Instance.TurnSnake(_newDirection);
            Game.Instance.MoveSnake();
        }

        public void IncreaseGameTime()
        {
            _elapsedTimeSeconds++;

            if (_elapsedTimeSeconds % 60 == 0)
            {
                _elapsedTimeMinutes++;

                if (_elapsedTimeMinutes % 60 == 0)
                    _elapsedTimeHours++;
            }

            OnPropertyChanged(nameof(GameTimeLabelText));
        }

        #endregion

        #region Private Methods

        private void Initialize()
        {
            TileBoxes.Clear();

            if (Level == null)
                return;

            Level.TileChanged += Level_TileChanged;

            for (int i = 0; i < Level.Dimension; i++)
            {
                for (int j = 0; j < Level.Dimension; j++)
                {
                    // Létrehozunk a mezőnek egy gombot
                    TileBox tileBox = new(i, j, (i, j) switch
                    {
                        (int x, int y) when x % 2 == 0 && y % 2 == 0 => kFieldColorLight,
                        (int x, int y) when x % 2 == 1 && y % 2 == 0 => kFieldColorDark,
                        (int x, int y) when x % 2 == 0 && y % 2 == 1 => kFieldColorDark,
                        (int x, int y) when x % 2 == 1 && y % 2 == 1 => kFieldColorLight,
                        _ => Colors.White
                    });

                    TileBoxes.Add(tileBox);
                }
            }

            for (int i = 0; i < Level.Dimension; i++)
                for (int j = 0; j < Level.Dimension; j++)
                    UpdateTileTexture(null, Level.Tile(i, j));
        }

        private void Reset()
        {
            _eatenEggs = 0;
            _elapsedTimeSeconds = 0;
            _elapsedTimeMinutes = 0;
            _elapsedTimeHours = 0;

            OnPropertyChanged(nameof(EatenEggsLabelText));
            OnPropertyChanged(nameof(GameTimeLabelText));

            Game.Instance.Snake!.SnakeMoved += Snake_Moved;
            Game.Instance.Snake!.SnakeGrew += Snake_Grew;

            _turns.Clear();
            _tailDirection = null;
            _newDirection = Game.Instance.Snake!.Direction;

            _shouldGrowTexture = false;
        }

        #endregion

        #region Private Helper Methods

        private void UpdateTileTexture(ITile? oldTile, ITile? newTile)
        {
            if (newTile is not null)
            {
                TileBoxes[Index(newTile.X, newTile.Y)].Texture = newTile switch
                {
                    EggTile => Texture.Egg,
                    ObstacleTile => Texture.WallTextureForNeighbors(Level!.AdjacentTiles(newTile.X, newTile.Y)),
                    _ => null
                };

                if ((oldTile is not null and ObstacleTile && newTile is not ObstacleTile) || newTile is ObstacleTile)
                    UpdateNeighboringObstacleTexture(newTile.X, newTile.Y);
            }
        }

        private void UpdateNeighboringObstacleTexture(int x, int y)
        {
            foreach (ITile? neighbor in Level!.AdjacentTiles(x, y))
                if (neighbor is not null and ObstacleTile)
                    TileBoxes[Index(neighbor.X, neighbor.Y)].Texture = Texture.WallTextureForNeighbors(Level!.AdjacentTiles(neighbor.X, neighbor.Y));
        }

        #endregion

        #region Private Static Helper Functions

        private static int Index(int x, int y) => y * (Level?.Dimension ?? 0) + x;

        private static Direction OppositeDirection(Direction d) => d switch
        {
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            Direction.Left => Direction.Right,
            Direction.Right => Direction.Left,
            _ => Direction.Left
        };

        #endregion

        #region Command Handlers

        private void OnLoadBuiltinLevel(BuiltinLevelType builtinLevel) => LoadBuiltinLevel?.Invoke(this, builtinLevel);

        private void OnLoadCustomLevel() => LoadCustomLevel?.Invoke(this, EventArgs.Empty);

        private void OnLoadRandomLevel() => LoadRandomLevel?.Invoke(this, EventArgs.Empty);

        private void OnAccelerateSnakeChanged(bool accelerateSnake) => AccelerateSnakeChanged?.Invoke(this, accelerateSnake);

        private void OnExitGame() => ExitGame?.Invoke(this, EventArgs.Empty);

        private void OnTurnSnake(Direction d)
        {
            if (Game.Instance.State is GameState.Running or GameState.Paused && d != OppositeDirection(Game.Instance.Snake!.Direction))
            {
                _newDirection = d;

                if (Game.Instance.State == GameState.Paused)
                    Game.Instance.Begin();
            }
        }

        private static void OnPlayPause()
        {
            if (Game.Instance.State == GameState.Initialized)
                Game.Instance.Begin();
            else if (Game.Instance.State == GameState.Running)
                Game.Instance.Pause();
            else if (Game.Instance.State == GameState.Ended)
                Game.Instance.Initialize();
        }

        private static void OnResetLevel()
        {
            if (Game.Instance.State != GameState.Running)
                Game.Instance.Initialize();
        }

        #endregion

        #region Event Handlers

        private void Game_StateChanged(object sender, GameStateChangedEventArgs e)
        {
            if (e.OldState == GameState.PreInitialized && e.NewState == GameState.Initialized)
            {
                OnPropertyChanged(nameof(Dimension));
                Initialize();
            }

            if (e.NewState == GameState.Initialized)
                Reset();

            if (e.OldState == GameState.Initialized && e.NewState == GameState.Running)
                Level!.PlaceRandomEgg();

            OnPropertyChanged(nameof(HintLabelText));
            OnPropertyChanged(nameof(LevelLoadMenuItemsEnabled));
        }

        private void Level_TileChanged(object sender, LevelTileChangedEventArgs e) => UpdateTileTexture(e.OldTile, e.NewTile);

        private void Snake_Moved(object sender, SnakeMovedEventArgs e)
        {
            _tailDirection ??= e.NewDirection;

            if (Level!.IsVisible(e.OldHead))
            {
                if (e.OldDirection == e.NewDirection)
                {
                    if (_shouldGrowTexture)
                    {
                        TileBoxes[Index(e.OldHead.X, e.OldHead.Y)].Texture = Texture.Snake.BodyFat.ForDirection(e.NewDirection);
                        _shouldGrowTexture = false;
                    }
                    else
                        TileBoxes[Index(e.OldHead.X, e.OldHead.Y)].Texture = Texture.Snake.Body.ForDirection(e.NewDirection);
                }
                else
                {
                    TileBoxes[Index(e.OldHead.X, e.OldHead.Y)].Texture = Texture.Snake.BodyTurn.ForDirection(e.OldDirection)?.ForDirection(e.NewDirection);
                    _turns.Enqueue((e.OldHead, e.NewDirection));
                }
            }

            if (Level!.IsVisible(e.NewTail))
            {
                if (_turns.Count > 0 && e.NewTail == _turns.Peek().Tile)
                    _tailDirection = _turns.Dequeue().NewDirection;

                TileBoxes[Index(e.NewTail.X, e.NewTail.Y)].Texture = Texture.Snake.Tail.ForDirection(_tailDirection ?? e.NewDirection);
            }

            if (Level!.IsVisible(e.OldTail))
                TileBoxes[Index(e.OldTail.X, e.OldTail.Y)].Texture = null;

            if (Level!.IsVisible(e.NewHead))
                TileBoxes[Index(e.NewHead.X, e.NewHead.Y)].Texture = Texture.Snake.Head.ForDirection(e.NewDirection);
        }

        private void Snake_Grew(object sender, SnakeGrewEventArgs e)
        {
            _eatenEggs++;
            OnPropertyChanged(nameof(EatenEggsLabelText));

            Game.Instance.Level!.PlaceRandomEgg();
            _shouldGrowTexture = true;
        }

        #endregion

        public enum BuiltinLevelType
        {
            Easy, Medium, Difficult
        }
    }
}
