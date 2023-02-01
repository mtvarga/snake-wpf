using Snake.Model;

namespace Snake.WPF
{
    public class Texture
    {
        #region Public Properties

        public Texture? Base { get { return _base; } }

        public Texture? Up { get; private set; }
        public Texture? Down { get; private set; }
        public Texture? Left { get; private set; }
        public Texture? Right { get; private set; }
        public Texture? Horizontal { get; private set; }
        public Texture? Vertical { get; private set; }
        public Texture? Cross { get; private set; }

        public string ImageSource => "../Resources/" + ResourceName + ".png";
        public string ResourceName { get { return (_base?.ResourceName ?? "") + _postfix; } }

        #endregion

        #region Private Members

        private readonly Texture? _base;

        private readonly string _postfix;

        #endregion

        #region Constructors

        public Texture(string postfix)
            : this(null, postfix) {}

        public Texture(Texture? baseTexture, string postfix)
        {
            _base = baseTexture;
            _postfix = postfix;
        }

        #endregion

        #region Public Methods

        public Texture NewUp(string postfix = "Up")
        {
            Up = new(this, postfix);
            return this;
        }

        public Texture NewDown(string postfix = "Down")
        {
            Down = new(this, postfix);
            return this;
        }

        public Texture NewLeft(string postfix = "Left")
        {
            Left = new(this, postfix);
            return this;
        }

        public Texture NewRight(string postfix = "Right")
        {
            Right = new(this, postfix);
            return this;
        }

        public Texture NewHorizontal(string postfix = "_Horizontal")
        {
            Horizontal = new(this, postfix);
            return this;
        }

        public Texture NewVertical(string postfix = "_Vertical")
        {
            Vertical = new(this, postfix);
            return this;
        }

        public Texture NewCross(string postfix = "_Cross")
        {
            Cross = new(this, postfix);
            return this;
        }

        public Texture? ForDirection(Direction d) => d switch
        {
            Direction.Up => Up,
            Direction.Down => Down,
            Direction.Left => Left,
            Direction.Right => Right,
            _ => this
        };

        #endregion

        #region Static Functions

        public static Texture? WallTextureForNeighbors(ITile?[] neighbors) => (neighbors[0], neighbors[1], neighbors[2], neighbors[3]) switch
        {
            (null or not ObstacleTile, null or not ObstacleTile, null or not ObstacleTile, null or not ObstacleTile) => Wall,

            (ObstacleTile, null or not ObstacleTile, null or not ObstacleTile, null or not ObstacleTile) => Wall.Right,
            (null or not ObstacleTile, ObstacleTile, null or not ObstacleTile, null or not ObstacleTile) => Wall.Down,
            (null or not ObstacleTile, null or not ObstacleTile, ObstacleTile, null or not ObstacleTile) => Wall.Left,
            (null or not ObstacleTile, null or not ObstacleTile, null or not ObstacleTile, ObstacleTile) => Wall.Up,

            (ObstacleTile, ObstacleTile, null or not ObstacleTile, null or not ObstacleTile) => Wall.Right?.Up,
            (null or not ObstacleTile, ObstacleTile, ObstacleTile, null or not ObstacleTile) => Wall.Left?.Up,
            (null or not ObstacleTile, null or not ObstacleTile, ObstacleTile, ObstacleTile) => Wall.Left?.Down,
            (ObstacleTile, null or not ObstacleTile, null or not ObstacleTile, ObstacleTile) => Wall.Right?.Down,
            (ObstacleTile, null or not ObstacleTile, ObstacleTile, null or not ObstacleTile) => Wall.Horizontal,
            (null or not ObstacleTile, ObstacleTile, null or not ObstacleTile, ObstacleTile) => Wall.Vertical,

            (ObstacleTile, ObstacleTile, ObstacleTile, null or not ObstacleTile) => Wall.Horizontal?.Up,
            (ObstacleTile, ObstacleTile, null or not ObstacleTile, ObstacleTile) => Wall.Vertical?.Left,
            (ObstacleTile, null or not ObstacleTile, ObstacleTile, ObstacleTile) => Wall.Horizontal?.Down,
            (null or not ObstacleTile, ObstacleTile, ObstacleTile, ObstacleTile) => Wall.Vertical?.Right,

            (ObstacleTile, ObstacleTile, ObstacleTile, ObstacleTile) => Wall.Cross
        };

        #endregion

        #region Textures

        internal static class Snake
        {
            public static Texture Head = new Texture("Snake_Head_")
                .NewUp()            // Snake_Head_Up
                .NewDown()          // Snake_Head_Down
                .NewLeft()          // Snake_Head_Left
                .NewRight();        // Snake_Head_Right

            public static Texture Body = new Texture("Snake_Body_")
                .NewUp()            // Snake_Body_Up
                .NewDown()          // Snake_Body_Down
                .NewLeft()          // Snake_Body_Left
                .NewRight();        // Snake_Body_Right

            public static Texture BodyTurn = new Texture("Snake_Body_Turn_")
                .NewUp()
                    .Up!
                    .NewLeft()      // Snake_Body_Turn_UpLeft
                    .NewRight()     // Snake_Body_Turn_UpRight
                .Base!
                .NewDown()
                    .Down!
                    .NewLeft()      // Snake_Body_Turn_DownLeft
                    .NewRight()     // Snake_Body_Turn_DownRight
                .Base!
                .NewLeft()
                    .Left!
                    .NewUp()        // Snake_Body_Turn_LeftUp
                    .NewDown()      // Snake_Body_Turn_LeftDown
                .Base!
                .NewRight()
                    .Right!
                    .NewUp()        // Snake_Body_Turn_RightUp
                    .NewDown()      // Snake_Body_Turn_RightDown
                .Base!;

            public static Texture BodyFat = new Texture("Snake_Body_Fat_")
                .NewUp()            // Snake_Body_Fat_Up
                .NewDown()          // Snake_Body_Fat_Down
                .NewLeft()          // Snake_Body_Fat_Left
                .NewRight();        // Snake_Body_Fat_Right

            public static Texture Tail = new Texture("Snake_Tail_")
                .NewUp()            // Snake_Tail_Up
                .NewDown()          // Snake_Tail_Down
                .NewLeft()          // Snake_Tail_Left
                .NewRight();        // Snake_Tail_Right
        }

        public static Texture Wall = new Texture("Wall")    // Wall
            .NewUp("_Up")
            .NewDown("_Down")
            .NewLeft("_Left")
                .Left!                                      // Wall_Left
                .NewUp()                                    // Wall_LeftUp
                .NewDown()                                  // Wall_LeftDown
            .Base!
            .NewRight("_Right")
                .Right!                                     // Wall_Right
                .NewUp()                                    // Wall_RightUp
                .NewDown()                                  // Wall_RightDown
            .Base!
            .NewHorizontal()
                .Horizontal!                                // Wall_Horizontal
                .NewUp()                                    // Wall_HorizontalUp
                .NewDown()                                  // Wall_HorizontalDown
            .Base!
            .NewVertical()
                .Vertical!                                  // Wall_Vertical
                .NewLeft()                                  // Wall_VerticalLeft
                .NewRight()                                 // Wall_VerticalRight
            .Base!
            .NewCross();                                    // Wall_Cross

        public static Texture Egg = new("Egg");             // Egg

        #endregion
    }
}
