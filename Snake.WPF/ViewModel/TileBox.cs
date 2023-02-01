using System.Windows.Media;

namespace Snake.WPF.ViewModel
{
    public class TileBox : ViewModelBase
    {
        #region Public Properties

        public int X => _x;
        public int Y => _y;
        public SolidColorBrush Background => new(_backgroundColor);

        public string? ImageSource => _texture?.ImageSource;

        public Texture? Texture
        {
            get => _texture;

            set
            {
                _texture = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        #endregion

        #region Private Members

        private readonly int _x;
        private readonly int _y;
        private readonly Color _backgroundColor;
        
        private Texture? _texture;

        #endregion

        #region Constructors

        public TileBox(int x, int y, Color backgroundColor)
        {
            _x = x;
            _y = y;
            _backgroundColor = backgroundColor;
        }

        #endregion
    }
}
