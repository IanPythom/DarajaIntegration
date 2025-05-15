using Asp.Versioning;
using DarajaAPI.Helpers;
using DarajaAPI.Models.Dto;
using DarajaAPI.Services.Daraja;
using Microsoft.AspNetCore.Mvc;

namespace DarajaAPI.Controllers.v1
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/mpesa/payments")]
    public class MpesaPaymentController : ControllerBase
    {
        private readonly IDarajaPaymentService _paymentService;
        private readonly ILipaNaService _lipaNaService;
        private readonly ILogger<MpesaPaymentController> _logger;

        public MpesaPaymentController(
            IDarajaPaymentService paymentService,
            ILipaNaService lipaNaService,
            ILogger<MpesaPaymentController> logger)
        {
            _paymentService = paymentService;
            _lipaNaService = lipaNaService;
            _logger = logger;
        }

        [HttpPost("initiate")]
        public async Task<IActionResult> InitiatePayment([FromBody] PaymentRequestDto request)
        {
            try
            {
                var result = await _paymentService.ProcessPaymentAsync(
                    request.PhoneNumber,
                    (decimal)request.Amount,
                    request.ReferenceNumber);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment initiation failed");
                return StatusCode(500, "Payment processing error");
            }
        }

        [HttpPost("simulate-lipa-na")]
        public async Task<IActionResult> SimulateLipaNaPayment([FromBody] LipaNaSimulationRequest request)
        {
            var validationResult = AccountValidator.ValidateAccountNumber(request.AccountNumber);

            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    Code = "LNMP001",
                    Message = "Invalid account number format",
                    ExpectedFormat = "2173219[yyyyMMddHHmmss]"
                });
            }

            var result = await _lipaNaService.ProcessPayment(request);
            return Ok(new
            {
                result.TransactionCode,
                result.Amount,
                BusinessContact = "0708 374 149",
                BusinessName = "IAN MWENDA GATUMU"
            });
        }
    }
}
