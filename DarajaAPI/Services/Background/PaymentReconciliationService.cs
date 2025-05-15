using DarajaAPI.RepositoryInterface;
using DarajaAPI.Services.Daraja;

namespace DarajaAPI.Services.Background
{
    public class PaymentReconciliationService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<PaymentReconciliationService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(15);

        public PaymentReconciliationService(
            IServiceProvider services,
            ILogger<PaymentReconciliationService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _services.CreateScope();
                var paymentService = scope.ServiceProvider.GetRequiredService<IDarajaPaymentService>();
                var transactionRepo = scope.ServiceProvider.GetRequiredService<IMpesaTransactionRepository>();

                try
                {
                    var pendingTransactions = await transactionRepo.GetUnverifiedTransactionsAsync();

                    foreach (var transaction in pendingTransactions)
                    {
                        var result = await paymentService.VerifyTransactionAsync(transaction.TransID);

                        if (result.IsSuccessful)
                        {
                            transaction.IsVerified = true;
                            transaction.VerificationDate = DateTime.UtcNow;
                            transaction.VerificationResult = result.TransactionStatus;
                            await transactionRepo.UpdateAsync(transaction);
                        }

                        await Task.Delay(1000, stoppingToken); // Rate limit
                    }

                    await transactionRepo.CommitAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Payment reconciliation failed");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
