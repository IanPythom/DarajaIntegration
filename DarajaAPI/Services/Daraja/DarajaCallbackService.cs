using Daraja.DbContext;
using DarajaAPI.Enums;
using DarajaAPI.Models.Domain;
using DarajaAPI.Models.Dto;
using DarajaAPI.RepositoryInterface;
using Microsoft.EntityFrameworkCore;

namespace DarajaAPI.Services.Daraja
{
    public class DarajaCallbackService : IDarajaCallbackService
    {
        private readonly DarajaDbContext _context;
        private readonly ILogger<DarajaCallbackService> _logger;
        private readonly IAccountService _accountService;
        private readonly IMpesaTransactionRepository _transactionRepository;

        public DarajaCallbackService(
            DarajaDbContext context,
            ILogger<DarajaCallbackService> logger,
            IAccountService accountService,
            IMpesaTransactionRepository transactionRepository)
        {
            _context = context;
            _logger = logger;
            _accountService = accountService;
            _transactionRepository = transactionRepository;
        }

        public async Task<TransactionResult> HandleConfirmationAsync(MpesaC2B transaction)
        {
            try
            {
                if (await _context.MpesaC2Bs.AnyAsync(t => t.TransID == transaction.TransID))
                {
                    _logger.LogWarning("Duplicate transaction: {TransID}", transaction.TransID);
                    return TransactionResult.Duplicate;
                }

                _context.MpesaC2Bs.Add(transaction);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Transaction confirmed: {TransID}", transaction.TransID);
                return TransactionResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Confirmation handling failed");
                return TransactionResult.Failed;
            }
        }

        // Minimum amount(≥10 KES) and Account format(prefix 2173219)
        public async Task<ValidationResult> HandleValidationAsync(MpesaValidationRequestDto request)
        {
            // 1. Check transaction uniqueness
            if (await _transactionRepository.TransactionExistsAsync(request.TransID))
            {
                _logger.LogWarning("Duplicate transaction validation: {TransID}", request.TransID);
                return ValidationResult.DuplicateTransaction;
            }

            // 2. Validate amount
            if (!decimal.TryParse(request.TransAmount, out var amount) || amount <= 0)
            {
                return ValidationResult.InvalidAmount;
            }

            // 3. Business rule: Minimum payment check
            const decimal minPayment = 10;
            if (amount < minPayment)
            {
                _logger.LogWarning("Amount below minimum: {Amount}", amount);
                return ValidationResult.AmountBelowMinimum;
            }

            // 4. Validate account reference and account format(prefix 2173219)
            var isValidAccount = await _accountService.ValidateAccountAsync(request.BillRefNumber);
            if (!isValidAccount)
            {
                return ValidationResult.InvalidAccount;
            }

            return ValidationResult.Valid;
        }

        public async Task HandleTransactionStatusAsync(TransactionStatusCallbackDto callback)
        {
            try
            {
                var transactionId = callback.Result.TransactionID;
                var status = callback.Result.ResultCode == 0 ? "Completed" : "Failed";

                _logger.LogInformation($"Transaction status update: {transactionId} - {status}");

                // Find and update transaction in database
                var transaction = await _context.MpesaC2Bs
                    .FirstOrDefaultAsync(t => t.TransID == transactionId);

                if (transaction != null)
                {
                    transaction.Status = status;
                    transaction.LastUpdated = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _logger.LogWarning($"Transaction not found: {transactionId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing transaction status");
            }
        }
    }
}
