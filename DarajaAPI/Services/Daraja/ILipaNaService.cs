using DarajaAPI.Models.Dto;

namespace DarajaAPI.Services.Daraja
{
    public interface ILipaNaService
    {
        Task<LipanaMpesaResultDto> ProcessPayment(LipaNaSimulationRequest request);
    }
}
