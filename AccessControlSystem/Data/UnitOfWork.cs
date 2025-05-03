using System.Threading;
using System.Threading.Tasks;
using AccessControlSystem.Data.Repositories;
using AccessControlSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace AccessControlSystem.Data
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        private readonly AccessControlContext _ctx;

        public UnitOfWork(AccessControlContext ctx)
        {
            _ctx = ctx;

            Users = new UserRepository(_ctx);
            Cards = new CardRepository(_ctx);
            CardReaders = new CardReaderRepository(_ctx);
            AccessTimes = new AccessTimeRepository(_ctx);
        }

        public IUserRepository Users { get; }
        public ICardRepository Cards { get; }
        public ICardReaderRepository CardReaders { get; }
        public IAccessTimeRepository AccessTimes { get; }

        public Task<int> CommitAsync(CancellationToken ct = default) =>
            _ctx.SaveChangesAsync(ct);

        public async ValueTask DisposeAsync()
        {
            await _ctx.DisposeAsync();
        }
    }
}