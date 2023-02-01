namespace Snake.Persistence
{
    public class SnakeStringDataAccess : ISnakeDataAccess
    {
        #region Public Methods

        public async Task<LevelDesign> LoadAsync(string text)
        {
            try
            {
                using StringReader reader = new(text);

                string line = await reader.ReadLineAsync() ?? "";
                string[] numbers = line.Split(' ');

                int levelSize = int.Parse(numbers[0]);
                int obstacleCount = int.Parse(numbers[1]);

                LevelDesign design = new(levelSize);

                for (int i = 0; i < obstacleCount; i++)
                {
                    line = await reader.ReadLineAsync() ?? "";
                    numbers = line.Split(' ');

                    int x = int.Parse(numbers[0]);
                    int y = int.Parse(numbers[1]);

                    design.AddObstacleLocation(x, y);
                }

                return design;
            }
            catch (Exception e)
            {
                throw new SnakeDataException(e);
            }
        }

        public Task SaveAsync(string path, LevelDesign design) => throw new SnakeDataException(new Exception("Cannot save design using SnakeStringDataAccess. If you want to save a design, use SnakeFileDataAccess instead."));

        #endregion
    }
}
