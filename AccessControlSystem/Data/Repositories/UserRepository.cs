using AccessControlSystem.Data;
using AccessControlSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AccessControlSystem.Data.Repositories
{
    internal class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AccessControlContext ctx) : base(ctx) { }

        public async Task<User?> GetByCardNumberAsync(string cardNumber) =>
            await _ctx.Users
                      .Include(u => u.Cards)
                      .AsNoTracking()
                      .FirstOrDefaultAsync(u => u.Cards.Any(c => c.CardNumber == cardNumber));

        public async Task<IEnumerable<User>> GetUsersWithCardsAsync() =>
            await _ctx.Users
                      .Include(u => u.Cards)
                      .AsNoTracking()
                      .ToListAsync();
    }
}
