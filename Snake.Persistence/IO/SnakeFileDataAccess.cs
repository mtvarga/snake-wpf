namespace Snake.Persistence
{
    public class SnakeFileDataAccess : ISnakeDataAccess
    {
        #region Public Methods

        public async Task<LevelDesign> LoadAsync(string path)
        {
            try
            {
                using StreamReader reader = new(path);

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

        public async Task SaveAsync(string path, LevelDesign design)
        {
            try
            {
                using StreamWriter writer = new(path);

                writer.Write(design.Size);
                await writer.WriteLineAsync(" " + design.ObstacleCount);

                design.ForEachObstacleLocation(async ((int X, int Y) location) => await writer.WriteLineAsync(location.X + " " + location.Y));
            }
            catch (Exception e)
            {
                throw new SnakeDataException(e);
            }
        }

        #endregion
    }
}
