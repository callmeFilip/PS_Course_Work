using AccessControlSystem.Models;

namespace AccessControlSystem.Data.Repositories
{
    public interface ICardRepository : IGenericRepository<Card>
    {
        Task<Card?> GetByNumberAsync(string cardNumber);
        Task<IEnumerable<Card>> GetCardsForUserAsync(int userId);
        Task<IEnumerable<Card>> GetAllWithOwnerAsync();
    }
}