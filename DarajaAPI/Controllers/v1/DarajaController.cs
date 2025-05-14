using Asp.Versioning;
using Daraja.DbContext;
using DarajaAPI.Models.Domain;
using DarajaAPI.Models.Dto;
using DarajaAPI.Services.Daraja;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
        private readonly DarajaDbContext _context;
        private readonly Serilog.ILogger _logger;

        public DarajaController(IHttpClientFactory clientFactory, IOptions<DarajaSetting> settings, DarajaAuthService darajaAuthService, DarajaDbContext context, Serilog.ILogger logger)
        {
            _clientFactory = clientFactory;
            _settings = settings.Value;
            _darajaAuthService = darajaAuthService;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Registers MPESA C2B validation and confirmation URLs with Safaricom's Daraja API
        /// </summary>
        /// <remarks>
        /// Important: This endpoint should be called only ONCE per environment configuration.
        /// <para>
        /// Safaricom's Daraja API allows URL registration only once for a given shortcode. 
        /// Subsequent attempts will fail with "Duplicate notification info" error.
        /// </para>
        /// <para>
        /// Ensure proper configuration in appsettings.json (ValidationURL, ConfirmationURL, ShortCode)
        /// before calling this endpoint. Requires valid Daraja API credentials.
        /// </para>
        /// </remarks>
        /// <returns>
        /// Returns Daraja API response indicating registration success/failure. 
        /// Successful response contains validation and confirmation URL registration details.
        /// </returns>
        /// <response code="200">URLs successfully registered with Daraja API</response>
        /// <response code="400">Invalid request parameters or configuration</response>
        /// <response code="401">Authentication failure with Daraja API</response>
        /// <response code="500">Internal server error or Daraja API communication failure</response>
        [MapToApiVersion("1.0")]
        [HttpPost]
        [Route("registerUrls")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        public async Task<IActionResult> Confirmation([FromBody] dynamic request)
        {
            try
            {
                var callbackData = JsonConvert.DeserializeObject<MpesaC2B>(request.Body.ToString()); // callback data is nested

                _context.MpesaC2Bs.Add(callbackData);
                await _context.SaveChangesAsync();

                _logger.Information($"Raw callback: {request.ToString()}");
                return Ok(new { ResultCode = 0, ResultDesc = "Success" });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to save transaction. Raw request: {Request}", request.ToString());
                return StatusCode(500, "Internal server error");
            }
        }

        [MapToApiVersion("1.0")]
        [HttpPost]
        [Route("validation")]
        //[Authorize(Roles = "Admin")]
        public IActionResult Validation([FromBody] MpesaC2B request)
        {
            // Process the validation request
            // Add your logic here

            return Ok(new
            {
                ResultCode = 0,
                ResultDesc = "Accepted"
            });
        }

        [MapToApiVersion("1.0")]
        [HttpPost]
        [Route("simulate-c2b-payment")]
        public async Task<IActionResult> SimulateC2BPayment([FromBody] C2BRequestDto request)
        {
            var token = await _darajaAuthService.GetAccessTokenAsync();
            var client = _clientFactory.CreateClient("mpesa");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var requestBody = new
            {
                ShortCode = _settings.ShortCode,
                CommandID = "CustomerPayBillOnline", // Ensure uppercase "ID"
                Amount = request.Amount, // Now a string
                Msisdn = request.PhoneNumber,
                BillRefNumber = request.AccountNumber
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_settings.MpesaBaseUrl}mpesa/c2b/v1/simulate", content);

            _logger.Information($"Request to Daraja: {JsonConvert.SerializeObject(requestBody)}");
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
    }
}