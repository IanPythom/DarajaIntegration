using DarajaAPI.Models;
using DarajaAPI.Models.Domain;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace DarajaAPI.Services.Daraja
{
    public class DarajaAuthService : IDarajaAuthService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly DarajaSetting _settings;
        private readonly DarajaConfig _config;

        public DarajaAuthService(IHttpClientFactory clientFactory, IOptions<DarajaSetting> options, DarajaConfig config)
        {
            _clientFactory = clientFactory;
            _settings = options.Value;
            _config = config;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var client = _clientFactory.CreateClient("mpesa");

            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.ConsumerKey}:{_settings.ConsumerSecret}"));
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
