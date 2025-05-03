using AccessControlSystem.Data;
using AccessControlSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AccessControlSystem.Data.Repositories
{
    internal class CardReaderRepository : GenericRepository<CardReader>, ICardReaderRepository
    {
        public CardReaderRepository(AccessControlContext ctx) : base(ctx) { }

        public async Task<IEnumerable<CardReader>> GetByAccessLevelAsync(int minLevel) =>
            await _ctx.CardReaders.AsNoTracking()
                                  .Where(r => r.AccessLevel >= minLevel)
                                  .ToListAsync();

        public async Task<IEnumerable<CardReader>> GetByLocationLikeAsync(string partial) =>
            await _ctx.CardReaders.AsNoTracking()
                                  .Where(r => EF.Functions.Like(r.Location, $"%{partial}%"))
                                  .ToListAsync();
    }
}