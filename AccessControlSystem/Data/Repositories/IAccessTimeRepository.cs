using AccessControlSystem.Models;

namespace AccessControlSystem.Data.Repositories
{
    public interface IAccessTimeRepository : IGenericRepository<AccessTime>
    {
        Task<IEnumerable<AccessTime>> GetLatestAsync(int take);
        Task<IEnumerable<AccessTime>> GetByCardAsync(int cardId,
                                                     DateTime from,
                                                     DateTime to);
        Task<IEnumerable<AccessTime>> GetByCardReaderAsync(int readerId,
                                                           DateTime from,
                                                           DateTime to);
        Task<IEnumerable<AccessTime>> GetByUserAsync(int userId,
                                                     DateTime from,
                                                     DateTime to);
    }
}