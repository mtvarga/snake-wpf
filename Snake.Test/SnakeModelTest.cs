using Moq;
using Snake.Model;
using Snake.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Snake.Test
{
    [TestClass]
    public class SnakeModelTest
    {
        private Level _mockedLevel = null!;
        private Mock<ISnakeDataAccess> _mock = null!;
        private LevelDesign _mockedDesign = null!;

        [TestInitialize]
        public void Initialize()
        {
            _mockedDesign = new LevelDesign(20);
            _mockedDesign.AddObstacleLocation(9, 4);
            _mockedDesign.AddObstacleLocation(9, 8);

            _mockedLevel = Level.FromDesign(_mockedDesign);

            _mock = new Mock<ISnakeDataAccess>();
            _mock.Setup(mock => mock.LoadAsync(It.IsAny<String>()))
                .Returns(() => Task.FromResult(_mockedDesign));

            Game.Instance.Initialize(_mockedLevel);
        }

        [TestMethod]
        public void SnakeModelInitializeEndGameTest()
        {
            Assert.AreEqual(Game.Instance.State, GameState.Initialized);
            Assert.IsNotNull(Game.Instance.Level);
            Assert.IsNotNull(Game.Instance.Snake);
            Assert.AreEqual(Game.Instance.Snake.Level, Game.Instance.Level);

            Game.Instance.End();

            Assert.AreEqual(Game.Instance.State, GameState.Ended);
        }

        [TestMethod]
        public void SnakeModelBeginGameTest()
        {
            Game.Instance.Begin();
            Assert.AreEqual(Game.Instance.State, GameState.Running);
        }

        [TestMethod]
        public void SnakeModelMoveTurnSnakeTest()
        {
            Game.Instance.Begin();
            Game.Instance.MoveSnake(); // jobbra

            int expectedX = 0;
            int expectedY = Convert.ToInt32(Math.Floor(Game.Instance.Level!.Dimension / 2.0)) - 1;

            Assert.AreEqual(Game.Instance.Snake?.Head, Game.Instance.Level?.Tile(expectedX, expectedY));

            Game.Instance.TurnSnake(Direction.Down);
            Game.Instance.MoveSnake();

            expectedX = 0;
            expectedY += 1;

            Assert.AreEqual(Game.Instance.Snake?.Head, Game.Instance.Level?.Tile(expectedX, expectedY));

            Game.Instance.TurnSnake(Direction.Right);
            Game.Instance.MoveSnake();

            Game.Instance.TurnSnake(Direction.Up);
            Game.Instance.MoveSnake();

            expectedX += 1;
            expectedY -= 1;

            Assert.AreEqual(Game.Instance.Snake?.Head, Game.Instance.Level?.Tile(expectedX, expectedY));

            Game.Instance.MoveSnake();
            Game.Instance.TurnSnake(Direction.Left);
            Game.Instance.MoveSnake();

            expectedX -= 1;
            expectedY -= 1;

            Assert.AreEqual(Game.Instance.Snake?.Head, Game.Instance.Level?.Tile(expectedX, expectedY));
        }

        [TestMethod]
        public void SnakeModelTailPassedTest()
        {
            Game.Instance.Begin();
            Game.Instance.MoveSnake(); // (0, Head.Y)

            FieldTile initialHead = Game.Instance.Snake!.Head;

            Assert.IsTrue(initialHead.IsSnake);

            for (int i = 0; i < Game.Instance.Snake.Size; i++)
                Game.Instance.MoveSnake();

            Assert.IsFalse(initialHead.IsSnake);
        }

        [TestMethod]
        public void SnakeModelGrowSnakeTest()
        {
            Game.Instance.Level!.SetEggTile(1, Game.Instance.Snake!.Head.Y);

            Game.Instance.Begin();
            Game.Instance.MoveSnake(); // jobbra, most a (0, Head.Y) helyen van.

            int initialSize = Game.Instance.Snake.Size!;

            Game.Instance.MoveSnake();

            Assert.AreEqual(initialSize + 1, Game.Instance.Snake.Size);
        }

        [TestMethod]
        public void SnakeModelKillSnakeTest()
        {
            Game.Instance.Level!.SetObstacleTile(0, Game.Instance.Snake!.Head.Y + 1);

            // Akadályba ütközés
            Game.Instance.Begin();
            Game.Instance.MoveSnake(); // (0, Head.Y)
            Game.Instance.TurnSnake(Direction.Down);

            Assert.IsTrue(Game.Instance.Snake!.IsAlive);

            Game.Instance.MoveSnake(); // (0, Head.Y + 1)

            Assert.IsFalse(Game.Instance.Snake!.IsAlive);
            Assert.AreEqual(Game.Instance.State, GameState.Ended);

            // Pálya határába ütközés
            Game.Instance.Initialize();
            Game.Instance.Begin();

            Game.Instance.MoveSnake(); // (0, Head.Y)
            Game.Instance.TurnSnake(Direction.Up);
            Game.Instance.MoveSnake(); // (0, Head.Y - 1)
            Game.Instance.TurnSnake(Direction.Left);

            Assert.IsTrue(Game.Instance.Snake!.IsAlive);

            Game.Instance.MoveSnake(); // (-1, Head.Y - 1)

            Assert.IsFalse(Game.Instance.Snake!.IsAlive);

            // Saját magába ütközés
            Game.Instance.Initialize();
            Game.Instance.Begin();

            Game.Instance.MoveSnake(); // (0, Head.Y)
            Game.Instance.MoveSnake(); // (1, Head.Y)
            Game.Instance.TurnSnake(Direction.Up);
            Game.Instance.MoveSnake(); // (1, Head.Y - 1)
            Game.Instance.TurnSnake(Direction.Left);
            Game.Instance.MoveSnake(); // (0, Head.Y - 1)
            Game.Instance.TurnSnake(Direction.Down);

            Assert.IsTrue(Game.Instance.Snake!.IsAlive);

            Game.Instance.MoveSnake(); // (0, Head.Y)

            Assert.IsFalse(Game.Instance.Snake!.IsAlive);
        }

        [TestMethod]
        public async Task SnakeModelLoadTest()
        {
            LevelDesign? design = await _mock.Object.LoadAsync(String.Empty);

            Assert.IsNotNull(design);

            Game.Instance.Initialize(Level.FromDesign(design));

            for (int i = 0; i < Game.Instance.Level!.Dimension; i++)
            {
                for (int j = 0; j < Game.Instance.Level!.Dimension; j++)
                {
                    ITile? tile = Game.Instance.Level.Tile(i, j);
                    Assert.IsNotNull(tile);

                    if (tile.IsObstacle())
                    {
                        bool found = false;

                        design.ForEachObstacleLocation(((int X, int Y) obstacle) =>
                        {
                            if (obstacle.X == i && obstacle.Y == j)
                                found = true;
                        });

                        Assert.IsTrue(found);
                    }
                }
            }

            _mock.Verify(dataAccess => dataAccess.LoadAsync(String.Empty), Times.Once());
        }

        [TestCleanup]
        public void EndGame() => Game.Instance.End();
    }
}