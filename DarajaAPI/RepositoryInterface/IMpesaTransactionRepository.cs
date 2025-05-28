using DarajaAPI.Models.Domain;

namespace DarajaAPI.RepositoryInterface
{
    public interface IMpesaTransactionRepository
    {
        Task<IEnumerable<MpesaC2B>> GetUnverifiedTransactionsAsync();
        Task UpdateAsync(MpesaC2B transaction);
        Task CommitAsync();
        Task<bool> TransactionExistsAsync(string transactionId);
        Task<MpesaC2B> GetByTransactionIdAsync(string transactionId);
    }
}
