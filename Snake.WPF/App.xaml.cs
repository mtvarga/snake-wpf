using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using Snake.Model;
using Snake.Persistence;
using Snake.WPF.View;
using Snake.WPF.ViewModel;

namespace Snake
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Constants

        private const int kTickInterval = 150;

        #endregion

        #region Private Members

        private SnakeStringDataAccess _builtinDataAccess = new();
        private SnakeFileDataAccess _customDataAccess = new();

        private MainWindow _view = null!;
        private SnakeViewModel _viewModel = null!;

        private DispatcherTimer _gameTimer = null!;
        private DispatcherTimer _snakeTimer = null!;

        private bool _shouldAccelerateSnake = false;

        #endregion

        #region Constructors

        public App()
        {
            Startup += new(App_Startup);
        }

        #endregion

        #region Application Event Handlers

        private void App_Startup(object sender, StartupEventArgs e)
        {
            Game.Instance.StateChanged += Game_StateChanged;

            _viewModel = new();
            _viewModel.LoadBuiltinLevel += ViewModel_LoadBuiltinLevel;
            _viewModel.LoadCustomLevel += ViewModel_LoadCustomLevel;
            _viewModel.LoadRandomLevel += ViewModel_LoadRandomLevel;
            _viewModel.AccelerateSnakeChanged += ViewModel_AccelerateSnakeChanged;
            _viewModel.ExitGame += ViewModel_ExitGame;

            _gameTimer = new()
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            _gameTimer.Tick += GameTimer_Tick;

            _snakeTimer = new()
            {
                Interval = TimeSpan.FromMilliseconds(kTickInterval)
            };

            _snakeTimer.Tick += SnakeTimer_Tick;

            _view = new()
            {
                DataContext = _viewModel
            };

            _view.Closing += View_Closing;
            _view.Show();
        }

        private void View_Closing(object? sender, CancelEventArgs e)
        {
            if (MessageBox.Show("Biztos, hogy ki akarsz lépni?", "Snake", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                e.Cancel = true;
        }
        
        #endregion

        #region Timer Event Handlers

        private void GameTimer_Tick(object? sender, EventArgs e) => _viewModel.IncreaseGameTime();

        private void SnakeTimer_Tick(object? sender, EventArgs e) => _viewModel.MoveSnake();

        #endregion

        #region Model Event Handlers

        private void Game_StateChanged(object sender, GameStateChangedEventArgs e)
        {
            if (e.OldState == GameState.PreInitialized && e.NewState == GameState.Initialized)
                Game.Instance.Snake!.SnakeGrew += Snake_Grew;

            if (e.OldState is GameState.Initialized or GameState.Paused && e.NewState == GameState.Running)
            {
                _gameTimer.Start();

                if (e.OldState == GameState.Initialized)
                    _snakeTimer.Interval = TimeSpan.FromMilliseconds(kTickInterval);

                _snakeTimer.Start();
            }

            if (e.OldState == GameState.Running && e.NewState is GameState.Paused or GameState.Ended)
            {
                _gameTimer.Stop();
                _snakeTimer.Stop();
            }
        }

        private void Snake_Grew(object sender, SnakeGrewEventArgs e)
        {
            if (_shouldAccelerateSnake)
                _snakeTimer.Interval -= TimeSpan.FromMilliseconds(2);
        }

        #endregion

        #region View Model Event Handlers

        private async void ViewModel_LoadBuiltinLevel(object? sender, SnakeViewModel.BuiltinLevelType e) => Game.Instance.Initialize(Level.FromDesign(await _builtinDataAccess.LoadAsync(e switch
        {
            SnakeViewModel.BuiltinLevelType.Easy => WPF.Properties.Resources.Level_Easy,
            SnakeViewModel.BuiltinLevelType.Medium => WPF.Properties.Resources.Level_Medium,
            SnakeViewModel.BuiltinLevelType.Difficult => WPF.Properties.Resources.Level_Difficult,
            _ => string.Empty
        })));

        private async void ViewModel_LoadCustomLevel(object? sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Snake pályadizájnok (*.sld)|*.sld",
                Title = "Pálya betöltése"
            };

            openFileDialog.ShowDialog();

            if (openFileDialog.FileName != "")
                Game.Instance.Initialize(Level.FromDesign(await _customDataAccess.LoadAsync(openFileDialog.FileName)));
        }

        private void ViewModel_LoadRandomLevel(object? sender, EventArgs e) => Game.Instance.Initialize(Level.Random(20, 5));

        private void ViewModel_AccelerateSnakeChanged(object? sender, bool e)
        {
            _shouldAccelerateSnake = e;

            if (!e)
                _snakeTimer.Interval = TimeSpan.FromMilliseconds(kTickInterval);
        }

        private void ViewModel_ExitGame(object? sender, EventArgs e) => _view.Close();

        #endregion
    }
}
