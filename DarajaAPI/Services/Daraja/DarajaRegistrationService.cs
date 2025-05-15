using DarajaAPI.Models.Domain;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace DarajaAPI.Services.Daraja
{
    public class DarajaRegistrationService : IDarajaRegistrationService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly DarajaSetting _settings;
        private readonly IDarajaAuthService _authService;
        private readonly ILogger<DarajaRegistrationService> _logger;

        public DarajaRegistrationService(
            IHttpClientFactory clientFactory,
            IOptions<DarajaSetting> settings,
            IDarajaAuthService authService,
            ILogger<DarajaRegistrationService> logger)
        {
            _clientFactory = clientFactory;
            _settings = settings.Value;
            _authService = authService;
            _logger = logger;
        }

        public async Task<string> RegisterUrlsAsync()
        {
            var token = await _authService.GetAccessTokenAsync();
            var client = _clientFactory.CreateClient("mpesa");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new
            {
                ValidationURL = _settings.ValidationURL,
                ConfirmationURL = _settings.ConfirmationURL,
                ResponseType = _settings.ResponseType,
                ShortCode = _settings.ShortCode
            };

            try
            {
                var response = await client.PostAsJsonAsync(_settings.RegisterUrlEndpoint, request);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("URLs registered successfully");
                }
                else
                {
                    _logger.LogError("URL registration failed: {Content}", content);
                }

                return content;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "URL registration critical failure");
                throw;
            }
        }
    }
}
