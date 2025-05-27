using DarajaAPI.Models.Dto;

namespace DarajaAPI.Services.Daraja
{
    public interface IDarajaRegistrationService
    {
        Task<DarajaResponseDto> RegisterUrlsAsync();
    }
}