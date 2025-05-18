using Asp.Versioning;
using DarajaAPI.Services.Daraja;
using Microsoft.AspNetCore.Mvc;

namespace DarajaAPI.Controllers.v1
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/mpesa/registration")]
    public class MpesaRegistrationController : ControllerBase
    {
        private readonly IDarajaRegistrationService _registrationService;

        public MpesaRegistrationController(IDarajaRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpPost("urls")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        /// <summary>
        /// Registers validation and confirmation URLs with Safaricom's Daraja API
        /// </summary>
        /// <remarks>
        /// Critical production endpoint. Should be called once during deployment.
        /// Requires valid Daraja credentials. Subsequent calls will fail if URLs are already registered.
        /// </remarks>
        public async Task<IActionResult> RegisterUrls()
        {
            var result = await _registrationService.RegisterUrlsAsync();
            return Ok(result);
        }
    }
}
