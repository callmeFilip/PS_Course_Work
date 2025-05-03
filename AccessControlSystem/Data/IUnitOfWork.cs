using System;
using System.Threading;
using System.Threading.Tasks;
using AccessControlSystem.Data.Repositories;

namespace AccessControlSystem.Data
{
    /// <summary>
    ///   A single transactional wrapper around <see cref="AccessControlContext"/>.
    ///   Every repository retrieved from the unit‑of‑work shares the same DbContext
    ///   instance, so <see cref="CommitAsync"/> persists all changes in one go.
    /// </summary>
    public interface IUnitOfWork : IAsyncDisposable
    {
        IUserRepository Users { get; }
        ICardRepository Cards { get; }
        ICardReaderRepository CardReaders { get; }
        IAccessTimeRepository AccessTimes { get; }

        /// <summary>Saves all pending changes to the database.</summary>
        /// <returns>The number of affected rows.</returns>
        Task<int> CommitAsync(CancellationToken ct = default);
    }
}