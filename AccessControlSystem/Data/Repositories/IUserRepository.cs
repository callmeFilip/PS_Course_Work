using AccessControlSystem.Data;
using AccessControlSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AccessControlSystem.Data.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByCardNumberAsync(string cardNumber);
        Task<IEnumerable<User>> GetUsersWithCardsAsync();
    }
}
