using DarajaAPI.Models;
using DarajaAPI.Models.Dto.Config;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace DarajaAPI.Services.Daraja
{
    public class DarajaAuthService : IDarajaAuthService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly DarajaConfig _config;

        public DarajaAuthService(IHttpClientFactory clientFactory, DarajaConfig config)
        {
            _clientFactory = clientFactory;
            _config = config;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var client = _clientFactory.CreateClient("mpesa");

            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_config.Credentials.ConsumerKey}:{_config.Credentials.ConsumerSecret}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            var authUrl = $"{GetBaseUrl()}{_config.Endpoints.Auth}";
            var response = await client.GetAsync(authUrl);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var tokenObject = JsonConvert.DeserializeObject<Token>(json); // Assuming Token is a class with an access_token property

            return tokenObject.access_token;
        }

        private string GetBaseUrl()
        {
            return _config.Environment.Equals("Production", StringComparison.OrdinalIgnoreCase)
                ? _config.BaseUrls.Production
                : _config.BaseUrls.Sandbox;
        }
    }
}
