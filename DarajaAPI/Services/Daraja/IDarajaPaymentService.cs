using DarajaAPI.Models.Dto;
using DarajaAPI.Records;

namespace DarajaAPI.Services.Daraja
{
    public interface IDarajaPaymentService
    {
        Task<object> ProcessPaymentAsync(string phoneNumber, decimal amount, string referenceNumber);
        Task<string> SimulateC2BPaymentAsync(C2BRequestDto request);
        Task<TransactionStatusResult> VerifyTransactionAsync(string transactionId);
    }
}