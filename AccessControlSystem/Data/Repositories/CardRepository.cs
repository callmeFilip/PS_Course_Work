using AccessControlSystem.Data;
using AccessControlSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AccessControlSystem.Data.Repositories
{
    internal class CardRepository : GenericRepository<Card>, ICardRepository
    {
        public CardRepository(AccessControlContext ctx) : base(ctx) { }

        public async Task<Card?> GetByNumberAsync(string cardNumber) =>
            await _ctx.Cards.AsNoTracking()
                            .FirstOrDefaultAsync(c => c.CardNumber == cardNumber);

        public async Task<IEnumerable<Card>> GetCardsForUserAsync(int userId) =>
            await _ctx.Cards.AsNoTracking()
                            .Where(c => c.UserId == userId)
                            .ToListAsync();
    }
}