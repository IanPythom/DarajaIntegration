using Daraja.DbContext;
using DarajaAPI.Models.Domain;
using DarajaAPI.RepositoryInterface;
using Microsoft.EntityFrameworkCore;

namespace DarajaAPI.Repositories
{
    public class MpesaTransactionRepository : IMpesaTransactionRepository
    {
        private readonly DarajaDbContext _context;

        public MpesaTransactionRepository(DarajaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MpesaC2B>> GetUnverifiedTransactionsAsync()
        {
            return await _context.MpesaC2Bs
                .Where(t => !t.IsVerified && t.CreatedAt > DateTime.UtcNow.AddDays(-7))
                .ToListAsync();
        }

        public async Task UpdateAsync(MpesaC2B transaction)
        {
            _context.Entry(transaction).State = EntityState.Modified;
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> TransactionExistsAsync(string transactionId)
        {
            return await _context.MpesaC2Bs
                .AnyAsync(t => t.TransID == transactionId);
        }
    }
}