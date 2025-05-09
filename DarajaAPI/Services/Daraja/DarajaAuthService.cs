using DarajaAPI.Models;
using DarajaAPI.Models.Domain;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace DarajaAPI.Services.Daraja
{
    public class DarajaAuthService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly DarajaSetting _settings;

        public DarajaAuthService(IHttpClientFactory clientFactory, IOptions<DarajaSetting> options)
        {
            _clientFactory = clientFactory;
            _settings = options.Value;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var client = _clientFactory.CreateClient("mpesa");

            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.ConsumerKey}:{_settings.ConsumerSecret}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            var response = await client.GetAsync($"{_settings.MpesaBaseUrl}oauth/v1/generate?grant_type=client_credentials");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var tokenObject = JsonConvert.DeserializeObject<Token>(json); // Assuming Token is a class with an access_token property

            return tokenObject.access_token;
        }
    }
}
