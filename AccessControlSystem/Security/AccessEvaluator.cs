using AccessControlSystem.Data;

namespace AccessControlSystem.Security
{
    /// <summary>
    ///   Central place for *every* access‑control rule.
    /// </summary>
    public sealed class AccessEvaluator(IUnitOfWork uow)
    {
        private readonly IUnitOfWork _uow = uow;

        /// <returns>true if the card’s level ≥ reader’s level</returns>
        public async Task<bool> IsAccessAllowedAsync(int cardId, int readerId,
                                                     CancellationToken ct = default)
        {
            var card = await _uow.Cards.GetByIdAsync(cardId);
            var reader = await _uow.CardReaders.GetByIdAsync(readerId);

            if (card is null) return false;        // unknown card -> deny
            if (reader is null) return false;        // unknown reader -> deny

            return card.AccessLevel >= reader.AccessLevel;
        }
    }
}
