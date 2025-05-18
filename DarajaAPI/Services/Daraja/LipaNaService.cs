using DarajaAPI.Models.Dto;

namespace DarajaAPI.Services.Daraja
{
    public class LipaNaService : ILipaNaService
    {
        public async Task<LipanaMpesaResultDto> ProcessPayment(LipaNaSimulationRequest request)
        {
            // Implement actual Lipa Na M-Pesa logic here
            return new LipanaMpesaResultDto
            {
                TransactionCode = "ABC123XYZ",
                Amount = request.Amount
            };
        }
    }
}
