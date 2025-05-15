using DarajaAPI.Models.Domain;
using DarajaAPI.Models.Dto;

namespace DarajaAPI.Services.Daraja
{
    public interface ILipaNaService
    {
        Task<LipanaMpesaResult> ProcessPayment(LipaNaSimulationRequest request);
    }
}
