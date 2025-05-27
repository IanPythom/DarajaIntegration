using DarajaAPI.Models.Domain;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using DarajaAPI.Models.Dto;
using DarajaAPI.Models.Dto.Config;

namespace DarajaAPI.Services.Daraja
{
    public class DarajaRegistrationService : IDarajaRegistrationService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly DarajaConfig _config;
        private readonly IDarajaAuthService _authService;
        private readonly ILogger<DarajaRegistrationService> _logger;

        public DarajaRegistrationService(IHttpClientFactory clientFactory, IOptions<DarajaConfig> config, IDarajaAuthService authService, ILogger<DarajaRegistrationService> logger)
        {
            _clientFactory = clientFactory;
            _config = config.Value;
            _authService = authService;
            _logger = logger;
        }

        public async Task<DarajaResponseDto> RegisterUrlsAsync()
        {
            var token = await _authService.GetAccessTokenAsync();
            var client = _clientFactory.CreateClient("mpesa");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new
            {
                ValidationURL = _config.Urls.Validation,
                ConfirmationURL = _config.Urls.Confirmation,
                ResponseType = "Completed",
                ShortCode = _config.Credentials.ShortCode
            };

            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Call Safaricom's C2B registration endpoint
                var response = await client.PostAsJsonAsync(_config.Endpoints.C2BRegister, request);
                var responseBody = await response.Content.ReadAsStringAsync();
                var darajaResponse = JsonConvert.DeserializeObject<DarajaResponseDto>(responseBody);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("URLs registered successfully");
                }
                else
                {
                    _logger.LogError("URL registration failed: {Content}", responseBody);
                }

                return darajaResponse;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "URL registration critical failure");
                throw;
            }
        }
    }
}
