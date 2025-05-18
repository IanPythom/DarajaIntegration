using DarajaAPI.Helpers;
using Daraja.DbContext;
using Microsoft.EntityFrameworkCore;

namespace DarajaAPI.Services.Daraja
{
    public class AccountService : IAccountService
    {
        private readonly DarajaDbContext _context;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
            DarajaDbContext context,
            ILogger<AccountService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> ValidateAccountAsync(string accountNumber)
        {
            try
            {
                // First validate the account format
                var formatValidation = AccountValidator.ValidateAccountNumber(accountNumber);
                if (!formatValidation.IsValid)
                {
                    _logger.LogWarning("Invalid account format: {AccountNumber}", accountNumber);
                    return false;
                }

                // Then check if account exists in the database
                var accountExists = await _context.Users.AnyAsync(u => u.AccountNumber == accountNumber);

                if (!accountExists)
                {
                    _logger.LogWarning("Account not found: {AccountNumber}", accountNumber);
                }

                return accountExists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Account validation failed for {AccountNumber}", accountNumber);
                return false;
            }
        }
    }
}