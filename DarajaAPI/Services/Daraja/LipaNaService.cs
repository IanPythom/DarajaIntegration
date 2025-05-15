using DarajaAPI.Models.Domain;
using DarajaAPI.Models.Dto;

namespace DarajaAPI.Services.Daraja
{
    public class LipaNaService : ILipaNaService
    {
        public async Task<LipanaMpesaResult> ProcessPayment(LipaNaSimulationRequest request)
        {
            // Implement actual Lipa Na M-Pesa logic here
            return new LipanaMpesaResult
            {
                TransactionCode = "ABC123XYZ",
                Amount = request.Amount
            };
        }
    }
}
