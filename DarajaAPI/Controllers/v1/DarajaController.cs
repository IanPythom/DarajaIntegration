using Asp.Versioning;
using DarajaAPI.Models.Domain;
using DarajaAPI.Services.Daraja;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;

namespace DarajaAPI.Controllers.v1
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiversion}/[controller]")]
    public class DarajaController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly DarajaSetting _settings;
        private readonly DarajaAuthService _darajaAuthService;

        public DarajaController(IHttpClientFactory clientFactory, IOptions<DarajaSetting> settings, DarajaAuthService darajaAuthService)
        {
            _clientFactory = clientFactory;
            _settings = settings.Value;
            _darajaAuthService = darajaAuthService;
        }

        [MapToApiVersion("1.0")]
        [HttpPost]
        [Route("registerUrls")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterMpesaUrlsAsync()
        {
            var requestBody = new
            {
                ValidationURL = _settings.ValidationURL,
                ConfirmationURL = _settings.ConfirmationURL,
                ResponseType = _settings.ResponseType,
                ShortCode = _settings.ShortCode
            };

            var jsonBody = System.Text.Json.JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var token = await _darajaAuthService.GetAccessTokenAsync();
            var client = _clientFactory.CreateClient("mpesa");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync(_settings.RegisterUrlEndpoint, content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return Ok(responseContent);
            }
            else
            {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
        }

        [MapToApiVersion("1.0")]
        [HttpPost]
        [Route("confirmation")]
        //[Authorize(Roles = "Admin")]
        public IActionResult Confirmation([FromBody] dynamic request)
        {
            // Process the confirmation request
            // Add your logic here

            return Ok(new
            {
                ResultCode = 0,
                ResultDesc = "Success"
            });
        }

        [MapToApiVersion("1.0")]
        [HttpPost]
        [Route("validation")]
        //[Authorize(Roles = "Admin")]
        public IActionResult Validation([FromBody] dynamic request)
        {
            // Process the validation request
            // Add your logic here

            return Ok(new
            {
                ResultCode = 0,
                ResultDesc = "Accepted"
            });
        }
    }
}