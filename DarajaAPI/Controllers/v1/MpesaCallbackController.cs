using Asp.Versioning;
using DarajaAPI.Enums;
using DarajaAPI.Filters;
using DarajaAPI.Models.Domain;
using DarajaAPI.Models.Dto;
using DarajaAPI.Services.Daraja;
using Microsoft.AspNetCore.Mvc;

namespace DarajaAPI.Controllers.v1
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/mpesa")]
    [ServiceFilter(typeof(MpesaIpWhitelistFilter))]
    public class MpesaCallbackController : ControllerBase
    {
        private readonly IDarajaCallbackService _callbackService;
        private readonly ILogger<MpesaCallbackController> _logger;

        public MpesaCallbackController(
            IDarajaCallbackService callbackService,
            ILogger<MpesaCallbackController> logger)
        {
            _callbackService = callbackService;
            _logger = logger;
        }

        [HttpPost]
        [Route("confirmation")]
        /// <summary>
        /// Handles payment confirmation callbacks from M-Pesa
        /// </summary>
        /// <remarks>
        /// PRODUCTION ENDPOINT. 
        /// Called automatically by Safaricom's servers after successful payments. 
        /// Stores transaction details and marks payments as verified.
        /// </remarks>
        public async Task<IActionResult> Confirmation([FromBody] MpesaC2B transaction)
        {
            var result = await _callbackService.HandleConfirmationAsync(transaction);

            return result switch
            {
                TransactionResult.Success => Ok(new { ResultCode = 0, ResultDesc = "Success" }),
                TransactionResult.Duplicate => Ok(new { ResultCode = 0, ResultDesc = "Duplicate" }),
                _ => StatusCode(500, new { ResultCode = 1, ResultDesc = "Internal Error" })
            };
        }

        [HttpPost]
        [Route("validation")]
        /// <summary>
        /// Validates incoming payment requests
        /// </summary>
        /// <remarks>
        /// PRODUCTION ENDPOINT. 
        /// Called by Safaricom before processing payments. 
        /// Verifies account validity and payment amounts.
        /// </remarks>
        public async Task<IActionResult> Validation([FromBody] MpesaValidationRequestDto request)
        {
            var validationResult = await _callbackService.HandleValidationAsync(request);

            return validationResult switch
            {
                ValidationResult.Valid => Ok(new MpesaValidationResponseDto
                { ResultCode = "0", ResultDesc = "Accepted" }),
                ValidationResult.InvalidAmount => Ok(new MpesaValidationResponseDto
                { ResultCode = "C2B00011", ResultDesc = "Invalid Amount" }),
                ValidationResult.AmountBelowMinimum => Ok(new MpesaValidationResponseDto
                { ResultCode = "C2B00012", ResultDesc = "Amount below minimum" }),
                ValidationResult.InvalidAccount => Ok(new MpesaValidationResponseDto
                { ResultCode = "C2B00013", ResultDesc = "Invalid Account" }),
                ValidationResult.DuplicateTransaction => Ok(new MpesaValidationResponseDto
                { ResultCode = "C2B00014", ResultDesc = "Duplicate Transaction" }),
                _ => Ok(new MpesaValidationResponseDto
                { ResultCode = "C2B00099", ResultDesc = "Rejected" })
            };
        }

        [HttpPost]
        [Route("transaction-status")]
        /// <summary>
        /// Handles transaction status callbacks from M-Pesa
        /// </summary>
        /// <remarks>
        /// PRODUCTION ENDPOINT. 
        /// Called by Safaricom with transaction status updates.
        /// Updates transaction records in the database.
        /// </remarks>
        public async Task<IActionResult> TransactionStatus([FromBody] TransactionStatusCallbackDto request)
        {
            try
            {
                await _callbackService.HandleTransactionStatusAsync(request);
                return Ok(new { ResultCode = 0, ResultDesc = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transaction status callback failed");
                return Ok(new { ResultCode = 1, ResultDesc = "Error processing callback" });
            }
        }
    }
}
