using AccessControlSystem.Data;
using AccessControlSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AccessControlSystem.Data.Repositories
{
    internal class AccessTimeRepository : GenericRepository<AccessTime>,
                                          IAccessTimeRepository
    {
        public AccessTimeRepository(AccessControlContext ctx) : base(ctx) { }

        public async Task<IEnumerable<AccessTime>> GetLatestAsync(int take) =>
            await _ctx.AccessTimes.AsNoTracking()
                                  .OrderByDescending(a => a.Time)
                                  .Take(take)
                                  .ToListAsync();

        public async Task<IEnumerable<AccessTime>> GetByCardAsync(int cardId,
                                                                  DateTime from,
                                                                  DateTime to) =>
            await _ctx.AccessTimes.AsNoTracking()
                                  .Where(a => a.CardId == cardId &&
                                              a.Time >= from && a.Time <= to)
                                  .OrderBy(a => a.Time)
                                  .ToListAsync();

        public async Task<IEnumerable<AccessTime>> GetByCardReaderAsync(int readerId,
                                                                        DateTime from,
                                                                        DateTime to) =>
            await _ctx.AccessTimes.AsNoTracking()
                                  .Where(a => a.CardReaderId == readerId &&
                                              a.Time >= from && a.Time <= to)
                                  .OrderBy(a => a.Time)
                                  .ToListAsync();

        public async Task<IEnumerable<AccessTime>> GetByUserAsync(int userId,
                                                                  DateTime from,
                                                                  DateTime to) =>
            await _ctx.AccessTimes.AsNoTracking()
                                  .Where(a => a.Card.UserId == userId &&
                                              a.Time >= from && a.Time <= to)
                                  .Include(a => a.Card)            // join users via card
                                  .OrderBy(a => a.Time)
                                  .ToListAsync();
    }
}