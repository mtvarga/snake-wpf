namespace Snake.Persistence
{
    public interface ISnakeDataAccess
    {
        Task<LevelDesign> LoadAsync(string path);
        Task SaveAsync(string path, LevelDesign design);
    }
}