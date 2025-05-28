using DarajaAPI.Enums;
using DarajaAPI.Models.Domain;
using DarajaAPI.Models.Dto;

namespace DarajaAPI.Services.Daraja
{
    public interface IDarajaCallbackService
    {
        Task<TransactionResult> HandleConfirmationAsync(MpesaC2B transaction);
        Task<ValidationResult> HandleValidationAsync(MpesaValidationRequestDto request);
        Task HandleTransactionStatusAsync(TransactionStatusCallbackDto callback);
    }
}