using AccessControlSystem.Models;

namespace AccessControlSystem.Data.Repositories
{
    public interface ICardReaderRepository : IGenericRepository<CardReader>
    {
        Task<IEnumerable<CardReader>> GetByAccessLevelAsync(int minLevel);
        Task<IEnumerable<CardReader>> GetByLocationLikeAsync(string partialLocation);
    }
}