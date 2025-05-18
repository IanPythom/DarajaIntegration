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

        [HttpPost]
        [Route("initiate")]
        /// <summary>
        /// Sends STK Push to user’s phone.
        /// </summary>
        /// <remarks>
        /// PRODUCTION ENDPOINT. 
        /// Triggers real payments to Paybill 2211. 
        /// Requires valid consumer credentials and a registered callback URL.
        /// </remarks>
        /// <param name="request">Payment details including phone number and amount</param>
        public async Task<IActionResult> InitiatePayment([FromBody] PaymentRequestDto request)
        {
            try
            {
                var result = await _paymentService.ProcessPaymentAsync(request.PhoneNumber, (decimal)request.Amount, request.ReferenceNumber);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment initiation failed");
                return StatusCode(500, "Payment processing error");
            }
        }

        [HttpPost]
        [Route("simulate-lipa-na")]
        /// <summary>
        /// Simulates a [TESTING ONLY] Lipa Na M-Pesa payment to validate acc no. format no callbacks or saved data
        /// </summary>
        /// <remarks>
        /// FOR DEVELOPMENT/TESTING ONLY. 
        /// Does not process real payments. 
        /// Validates account format and returns mock transaction data.
        /// </remarks>
        /// <param name="request">Test payment simulation request</param>
        public async Task<IActionResult> SimulateLipaNaPayment([FromBody] LipaNaSimulationRequest request)
        {
            var validationResult = AccountValidator.ValidateAccountNumber(request.AccountNumber);

            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    Code = "LNMP001",
                    Message = "Invalid account number format",
                    ExpectedFormat = "174379[yyyyMMddHHmmss]"
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
